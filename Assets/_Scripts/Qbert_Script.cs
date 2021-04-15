using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Numerics;
using JetBrains.Annotations;
using UnityEngine;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class Qbert_Script : MonoBehaviour
{

    [SerializeField]
    private float xFactor;
    [SerializeField]
    private float yFactor;

    [SerializeField] 
    private Vector3 SpawnLocaton;

    [SerializeField]
    private float changeInX;

    [SerializeField] 
    public SpriteRenderer spriteRenderer;
    private Movement_Script movementScript;

    public Vector3 position0 = Vector3.zero;
    public Vector3 position1 = Vector3.zero;
    public Vector3 position2 = Vector3.zero;

    public bool isJumping = false;

    [SerializeField] private float BezierCurveFactor;
    private float tParamBezierParameter = 0f;

    // Elevator stuff
    private Vector3 finalPosition;
    private Vector3 startingPosition;
    private Vector3 NormalizedDirection;
    public bool isOnElevator = false;
    [SerializeField]
    private Elevator_Script ElevatorL;

    [SerializeField]
    private Elevator_Script ElevatorR;

    [SerializeField]
    private Collider2D collider;

    private Elevator_Script ElevatorMovingTo;

    public static Vector3 CoilyBlockToDieFrom;

    // Curse Word
    [SerializeField] 
    private GameObject CurseWord;

    // Blocks stuff
    private Blocks BlockMovingTo;

    // Qbert Die
    public static bool Qbert_Dead = false;
    private Vector2 tempPositionForJumpingEffect = Vector2.zero;
    private Vector2 deadPositionForBezierCurve = Vector2.zero;
    private float deadzoneY = -10f;
    private bool deathByFall = false;
    private bool deathByEnemy = false;

    // Sounds
    private SoundManager SoundManagerReference;

    enum SpriteJumpDirection
    {
        BottomLeft,
        BottomRight,
        JumpBottomLeft,
        JumpBottomRight,
        TopLeft,
        TopRight,
        JumpTopLeft,
        JumpTopRight
    }

    private SpriteJumpDirection SpriteSwitchCase;

    [Header("Sprites")]
    public Sprite BottomLeft;
    public Sprite BottomRight;
    public Sprite JumpBottomLeft;
    public Sprite JumpBottomRight;
    public Sprite TopLeft;
    public Sprite TopRight;
    public Sprite JumpTopLeft;
    public Sprite JumpTopRight;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer.sprite = BottomLeft;
        transform.position = SpawnLocaton;

        // Sounds
        SoundManagerReference = GameObject.FindObjectOfType<SoundManager>();
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isOnElevator)
        {
            return;
        }

        if (GameManager.WinTrigerred)
        {
            return;
        }

        if (!isJumping && !Qbert_Dead)
        {
            //Debug.Log("KeyPress Active");
            KeyboardInputCheck();
            return;
        }

        if (tParamBezierParameter < 1.0)
        {
            this.tParamBezierParameter += BezierCurveFactor * Time.deltaTime;
            this.transform.position = Vector3.Lerp(Vector3.Lerp(this.position0, this.position1, tParamBezierParameter), 
                Vector3.Lerp(this.position1, this.position2, tParamBezierParameter), tParamBezierParameter);
        }

        if (isJumping)
        {
            switch (SpriteSwitchCase)
            {
                case SpriteJumpDirection.JumpBottomLeft:
                    spriteRenderer.sprite = JumpBottomLeft;
                    break;

                case SpriteJumpDirection.JumpBottomRight:
                    spriteRenderer.sprite = JumpBottomRight;
                    break;

                case SpriteJumpDirection.JumpTopLeft:
                    spriteRenderer.sprite = JumpTopLeft;
                    break;

                case SpriteJumpDirection.JumpTopRight:
                    spriteRenderer.sprite = JumpTopRight;
                    break;

                default:
                    spriteRenderer.sprite = BottomLeft;
                    break;
            }
        }
       

        if (this.transform.position == this.position2)
        {

            if (BlockMovingTo != null)
            {
                BlockMovingTo.Activate();
                BlockMovingTo = null;
            }
            else if (ElevatorMovingTo != null)
            {
                ElevatorMovingTo.ActivateElevator = true;
                ElevatorMovingTo = null;
            }

            isJumping = false;
            tParamBezierParameter = 0;
            if (SpriteSwitchCase == SpriteJumpDirection.JumpTopLeft)
            {
                spriteRenderer.sprite = TopLeft;
            }
            else if (SpriteSwitchCase == SpriteJumpDirection.JumpTopRight)
            {
                spriteRenderer.sprite = TopRight;
            }
            else if (SpriteSwitchCase == SpriteJumpDirection.JumpBottomLeft)
            {
                spriteRenderer.sprite = BottomLeft;
            }
            else if (SpriteSwitchCase == SpriteJumpDirection.JumpBottomRight)
            {
                spriteRenderer.sprite = BottomRight;
            }

            // If Qbert is dead, get the bezier curve movement to fall down, and reset
            if (Qbert_Dead && deathByFall)
            {
                //Debug.Log("In DeathbyFallQbert");
                float tempCurveFactor = BezierCurveFactor;
                MoveAlongBezierCurve(this.transform.position, deadPositionForBezierCurve);
                if (transform.position.y == deadzoneY)
                {
                    tParamBezierParameter = 1;
                    GameManager.ResetGame = true;
                }
            }
            else if (Qbert_Dead && deathByEnemy)
            {
                //Time.timeScale = 0;
                tParamBezierParameter = 1;
                Debug.Log("In DeathbyEnemyQbert (Update)");
                GameManager.ResetGame = true;
            }
        }

        //this.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
    }

    void KeyboardInputCheck()
    {
        
        if (Input.GetKeyDown(KeyCode.Keypad7) || Input.GetKeyDown(KeyCode.Q))
        {
            //this.transform.position = new Vector3(this.transform.position.x - xFactor  , this.transform.position.y + yFactor, 0);
            Blocks block =
                FindBlockAtPosition(new Vector3(transform.position.x - xFactor, transform.position.y + yFactor, 0f));
            SpriteSwitchCase = SpriteJumpDirection.JumpTopLeft;
            if (block != null)
            {
                //Debug.Log("I move to a block!");
                MoveAlongBezierCurve(this.transform.position,
                    new Vector2(block.transform.position.x, block.transform.position.y + 0.5f));
                BlockMovingTo = block;
                SoundManagerReference.Play_QbertJump_Src();
            }
            else
            {
                //Debug.Log("No Block to move, checking for elevator");
                Elevator_Script Elevator = FindElevatorAtPosition();
                if (Elevator != null)
                {
                    MoveAlongBezierCurve(this.transform.position,
                        new Vector2(Elevator.transform.position.x, Elevator.transform.position.y + 0.2f));
                    ElevatorMovingTo = Elevator;

                    // Activate Coily death here
                    CoilyBlockToDieFrom = new Vector3(this.transform.position.x, this.transform.position.y - 0.2f,0);
                    Coily_Script.CoilyDeath = true;
                    collider.enabled = false;
                }
                else
                {
                    Debug.Log("Die elevator die left ");
                    Qbert_Dead = true;
                    deathByFall = true;
                    tempPositionForJumpingEffect = new Vector2(transform.position.x - xFactor, transform.position.y + yFactor);
                    MoveAlongBezierCurve(transform.position, tempPositionForJumpingEffect);
                    deadPositionForBezierCurve = new Vector2(transform.position.x, deadzoneY);
                    // changing sorting order to make it look more realistic
                    this.spriteRenderer.sortingOrder = -1;
                    SoundManagerReference.Play_QbertFall_Src();
                }
            }
        }
        else if (Input.GetKeyDown(KeyCode.Keypad9) || Input.GetKeyDown(KeyCode.W))
        {
            // Debug.Log("Right Up");
            // this.transform.position = new Vector3(this.transform.position.x + xFactor, this.transform.position.y + yFactor, 0);

            Blocks block =
                FindBlockAtPosition(new Vector3(transform.position.x + xFactor, transform.position.y + yFactor, 0f));
            SpriteSwitchCase = SpriteJumpDirection.JumpTopLeft;
            if (block != null)
            {
                //Debug.Log("I move to a block!");
                MoveAlongBezierCurve(this.transform.position,
                    new Vector2(block.transform.position.x, block.transform.position.y + 0.5f));
                BlockMovingTo = block;
                SoundManagerReference.Play_QbertJump_Src();
            }
            else
            {
                //Debug.Log("No Block to move");
                Elevator_Script Elevator = FindElevatorAtPosition();
                if (Elevator != null)
                {
                    MoveAlongBezierCurve(this.transform.position,
                        new Vector2(Elevator.transform.position.x, Elevator.transform.position.y + 0.2f));
                    ElevatorMovingTo = Elevator;

                    // Activate Coily death here
                    CoilyBlockToDieFrom = new Vector3(this.transform.position.x, this.transform.position.y - 0.2f, 0);
                    Coily_Script.CoilyDeath = true;
                    collider.enabled = false;
                }
                else
                {
                    Debug.Log("Die elevator die Right");
                    Qbert_Dead = true;
                    deathByFall = true;
                    tempPositionForJumpingEffect = new Vector2(transform.position.x + xFactor, transform.position.y + yFactor);
                    MoveAlongBezierCurve(transform.position, tempPositionForJumpingEffect);
                    deadPositionForBezierCurve = new Vector2(transform.position.x, deadzoneY);
                    this.spriteRenderer.sortingOrder = -1;
                    SoundManagerReference.Play_QbertFall_Src();
                }
            }

           //MoveAlongBezierCurve(this.transform.position,
             //   new Vector2(this.transform.position.x + xFactor, this.transform.position.y + yFactor));
            SpriteSwitchCase = SpriteJumpDirection.JumpTopRight;
        }
        else if (Input.GetKeyDown(KeyCode.Keypad3) || Input.GetKeyDown(KeyCode.S))
        {
           Blocks block =
                FindBlockAtPosition(new Vector3(transform.position.x + xFactor, transform.position.y - yFactor, 0f));
            SpriteSwitchCase = SpriteJumpDirection.JumpTopLeft;
            if (block != null)
            {
                //Debug.Log("I move to a block!");
                MoveAlongBezierCurve(this.transform.position,
                    new Vector2(block.transform.position.x, block.transform.position.y + 0.5f));
                BlockMovingTo = block;
                SoundManagerReference.Play_QbertJump_Src();
            }
            else
            {
                Debug.Log("No Block to move Die Qbert");
                Qbert_Dead = true;
                deathByFall = true;
                tempPositionForJumpingEffect = new Vector2(transform.position.x + xFactor, transform.position.y - yFactor);
                MoveAlongBezierCurve(transform.position, tempPositionForJumpingEffect);
                deadPositionForBezierCurve = new Vector2(transform.position.x, deadzoneY);
                SoundManagerReference.Play_QbertFall_Src();
            }


            //MoveAlongBezierCurve(this.transform.position,
               // new Vector2(this.transform.position.x + xFactor, this.transform.position.y - yFactor));
            SpriteSwitchCase = SpriteJumpDirection.JumpBottomRight;
   
        }
        else if (Input.GetKeyDown(KeyCode.Keypad1) || Input.GetKeyDown(KeyCode.A))
        {
            //Debug.Log("Left Down");
            // this.transform.position = new Vector3(this.transform.position.x - xFactor, this.transform.position.y - yFactor, 0);

            Blocks block =
                FindBlockAtPosition(new Vector3(transform.position.x - xFactor, transform.position.y - yFactor, 0f));

            SpriteSwitchCase = SpriteJumpDirection.JumpTopLeft;

            if (block != null)
            {
                //Debug.Log("I move to a block!");
                MoveAlongBezierCurve(this.transform.position,
                    new Vector2(block.transform.position.x, block.transform.position.y + 0.5f ));
                BlockMovingTo = block;
                SoundManagerReference.Play_QbertJump_Src();
            }
            else
            {
                Debug.Log("No Block to move Die Qbert");
                Qbert_Dead = true;
                deathByFall = true;
                tempPositionForJumpingEffect = new Vector2(transform.position.x - xFactor, transform.position.y - yFactor);
                MoveAlongBezierCurve(transform.position, tempPositionForJumpingEffect);
                deadPositionForBezierCurve = new Vector2(transform.position.x, deadzoneY);
                SoundManagerReference.Play_QbertFall_Src();
            }

            //MoveAlongBezierCurve(this.transform.position,
                //new Vector2(this.transform.position.x - xFactor, this.transform.position.y - yFactor));
            SpriteSwitchCase = SpriteJumpDirection.JumpBottomLeft;
        }
    }

    public void MoveAlongBezierCurve(Vector3 startPosition, Vector3 endPosition)
    {
       // Debug.Log("In Move Bezier Curve Function!");
        position0 = startPosition;
        position2 = endPosition;

       // Debug.Log("Position 0 = " + position0);
        //Debug.Log("Position 2 = " + position2);

        position1 = position0 + (position2 - position0) / 2 + Vector3.up * BezierCurveFactor;

        //Debug.Log("Position 1 = " + position1);

        //Vector3 m1 = Vector3.Lerp(position0, position1, BezierCurveFactor * Time.deltaTime);
        //Vector3 m2 = Vector3.Lerp(position1, position2, BezierCurveFactor * Time.deltaTime);
        //this.GetComponent<Transform>().position = Vector3.Lerp(m1, m2, BezierCurveFactor * Time.deltaTime);
        isJumping = true;
    }

    // Elevator stop to Starting Point Translation
    public void PostElevatorMovement()
    {
        finalPosition = new Vector3(0f, 0.5f);

        spriteRenderer.sprite = BottomLeft;
        isOnElevator = false;
        this.transform.position = finalPosition;
        Coily_Script.CoilyDeath = false;
        collider.enabled = true;
    }

    public void CheckAccessForScriptDebugPurposes()
    {
        //Debug.Log("Script Accessed!");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        //Debug.Log("QBERT HAS COLLIDED! with " + collision.collider.name);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Ally"))
        {
            //Debug.Log("Qbert has collided with green ball, now freeze the enemy");
            GameManager.FreezeObjectsGreenBall = true;
            // Score for catching a green ball
            GameManager.CurrentScore += 100;
            SoundManagerReference.Play_GreenBallActivated_Src();
        }

        if (collider.gameObject.CompareTag("Enemy"))
        { 
            Debug.Log("Qbert has collided with an enemy!!!");
            CurseWord.SetActive(true);
            // Qbert is now dead, delay for 1/2 seconds and respawn Qbert at the same block location
            Qbert_Dead = true;
            deathByEnemy = true;
            SoundManagerReference.Play_QbertSwear_Src();
        } 
    }


    // Find block at position
    Blocks FindBlockAtPosition(Vector3 pos)
    {
        //Vector3 newPosition = new Vector3(transform.position.x + changeInX, transform.position.y - yFactor - 0.5f, 0f);
        Vector3 newPosition = new Vector3(pos.x, pos.y - 0.5f, 0f);

        //Debug.Log("Vector new Pos =  " + newPosition);

        Blocks block = GameManager.list_blocks.FirstOrDefault(x => (x.transform.position - newPosition).magnitude < 0.5f);


        return block;
    }

    // Find elevator
    Elevator_Script FindElevatorAtPosition()
    {
        if (transform.position.y == -2.3f)
        {
            //Debug.Log("Elevator Return for R = " + transform.position.x);

            if (Mathf.Approximately(transform.position.x,1.84f))
            {
                
                return ElevatorR;
            }
            else if (Mathf.Approximately(transform.position.x, -2.0f))
            {
                return ElevatorL;
            }
        }
        return null;
    }

    // To reset Qbert
    public void ResetQbert()
    {
        //Debug.Log("In Reset");
        // Reset Qbert parameters
        // Sorting Order
        this.spriteRenderer.sortingOrder = 1;

        Qbert_Dead = false;
        if (deathByFall || (deathByEnemy && Mathf.Approximately(transform.position.y,0.0f)))
        {
            Debug.Log("In DBF/Qbert Spawn reset");
            transform.position = SpawnLocaton;
            position2 = SpawnLocaton;
            Debug.Log("Qbert transform.position = " + transform.position);
        }
        if (deathByEnemy)
        {
            tParamBezierParameter = 0;
        }

        deathByEnemy = false;
        deathByFall = false;
        CurseWord.SetActive(false);
      
    }
}
