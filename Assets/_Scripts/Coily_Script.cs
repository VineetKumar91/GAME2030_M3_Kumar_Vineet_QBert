using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Coily_Script : BallMovement
{

    [SerializeField]
    private float xFactor = 0.48f;
    [SerializeField]
    private float yFactor = -0.7f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Sprite currentSprite;

    [Header("Sprites For Coily Ball")]
    public Sprite Idle;
    public Sprite Jump;

    [Header("Sprites For Coily")]
    public Sprite BottomLeft;
    public Sprite BottomRight;
    public Sprite JumpBottomLeft;
    public Sprite JumpBottomRight;
    public Sprite TopLeft;
    public Sprite TopRight;
    public Sprite JumpTopLeft;
    public Sprite JumpTopRight;

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

    [Header("Qbert")]
    [SerializeField]
    private GameObject Qbert;
    private Qbert_Script QbertScript;

    private bool spawnLocationCheck = true;

    public Vector3 position0 = Vector3.zero;
    public Vector3 position1 = Vector3.zero;
    public Vector3 position2 = Vector3.zero;

    public bool isJumping = true;

    [SerializeField] private float BezierCurveFactor;
    private float tParamBezierParameter = 0f;

    private bool isCoilyHatched = false;

    public static bool CoilyDeath = false;
    private bool killCoilyExecuteOnce = true;

    // Animation correction for bezier curve -
    private bool doAnimation = true;

    // Sounds
    private SoundManager SoundManagerReference;
 

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Coily Started");
        spriteRenderer.sprite = Idle;
        QbertScript = GameObject.FindGameObjectWithTag("Qbert").GetComponent<Qbert_Script>();
        if (Qbert == null)
        {
            Qbert = GameObject.Find("Qbert");
        }
        //QbertScript.CheckAccessForScriptDebugPurposes();
        //Qbert.gameObject.GetComponent<Transform>().position = new Vector2(10f, 10f);

        // Sounds
        SoundManagerReference = GameObject.FindObjectOfType<SoundManager>();
        CoilyDeath = false;

    }

    void FixedUpdate()
    {
        // Entry
        if (transform.position.y > -0.38 && !isCoilyHatched)
        {
            Vector2 tempPos = Vector3.Lerp(transform.position, new Vector3(transform.position.x, -0.38f, 0f),
                Time.deltaTime * 15f);
            transform.position = tempPos;
        }

        // Destroy Coily when its offscreen bottom
        if (this.transform.position.y <= -7 && killCoilyExecuteOnce)
        {
            StartCoroutine(DestroyCoily());
            killCoilyExecuteOnce = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.position.y <= -0.37f && spawnLocationCheck)
        {
            spawnLocationCheck = false;
            isJumping = false;
            // Nice function call way.
            InvokeRepeating("MoveToNextPosition", 0, 1.5f);
        }

        if (tParamBezierParameter < 1.0 && isJumping)
        {
            //Debug.Log("In Bezier Curve and isJumping is true");
            this.tParamBezierParameter += BezierCurveFactor * Time.deltaTime;
            this.transform.position = Vector3.Lerp(Vector3.Lerp(this.position0, this.position1, tParamBezierParameter),
                Vector3.Lerp(this.position1, this.position2, tParamBezierParameter), tParamBezierParameter);
        }

        if (this.transform.position == this.position2 && !isCoilyHatched)
        {
            isJumping = false;
            tParamBezierParameter = 0;
            spriteRenderer.sprite = Idle;
        }

        if (this.transform.position.y <= -3.87 && !isCoilyHatched)
        {
            CoilyHatch();
        }

        if (isJumping && isCoilyHatched)
        {
            switch (SpriteSwitchCase)
            {
                case SpriteJumpDirection.JumpBottomLeft:
                    //Debug.Log("Here in resting of hatched coily");
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
                    
                    break;
            }
        }

        if (this.transform.position == this.position2 && isCoilyHatched)
        {
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
        }

    }

    public void MoveAlongBezierCurve(Vector3 startPosition, Vector3 endPosition)
    {
        position0 = startPosition;
        position2 = endPosition;
        position1 = position0 + (position2 - position0) / 2 + Vector3.up * BezierCurveFactor;

    }

    public void MoveToNextPosition()
    {
        //Debug.Log("Move to next Position Call");
        if (!GameManager.FreezeObjectsGreenBall)
        {
            spriteRenderer.sprite = Jump;
            isJumping = true;
            //Debug.Log("Check");
            float newXPosition = (Random.Range(1, 10) % 2 == 0) ? xFactor : (xFactor * -1f);
            MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x + newXPosition, this.transform.position.y + yFactor));
            SoundManagerReference.Play_BallJump_Src();
            //Debug.Log("Next Position = " + new Vector3(this.transform.position.x + newXPosition, this.transform.position.y + yFactor));
        }

    }

    public void CoilyHatch()
    {
        CancelInvoke("MoveToNextPosition");
        isJumping = false;
        isCoilyHatched = true;
        InvokeRepeating("CoilyChase", 0, 1.5f);
    }

    // Make Coily chase Qbert
    public void CoilyChase()
    {
        float newXPosition = 0f;
        float newYPosition = 0f;
        if (CoilyDeath && !GameManager.FreezeObjectsGreenBall)
        {
            // if coily death is active and not frozen state
            // make coily chase to the block before elevator and jump off it
            // similar to qbert's jump down probably
            //Debug.Log("Coily death state activated pos = " + Qbert_Script.CoilyBlockToDieFrom);
            if (Mathf.Abs(Qbert_Script.CoilyBlockToDieFrom.y - transform.position.y) < 0.15)
            {
                if (Mathf.Abs(Qbert_Script.CoilyBlockToDieFrom.x - transform.position.x) < 0.15)
                {
                    Debug.Log("Coily is at the death location");
                    //Debug.Log("Coily is moving to death position.");
                    if (transform.position.x > 0)
                    {
                        // isJumping = true;
                        // Debug.Log("Coily is at the death location +x" + new Vector3(this.transform.position.x + xFactor, -10f));
                        // MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x + xFactor, -10f));

                        newXPosition = xFactor;
                        newYPosition = -10f;
                        SpriteSwitchCase = SpriteJumpDirection.JumpTopRight;
                    }
                    else
                    {
                        // isJumping = true;
                        // Debug.Log("Coily is at the death location -x"+ new Vector3(this.transform.position.x - xFactor, -10f));
                        // MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x - xFactor, -10f));
                        // SoundManagerReference.Play_CoilyJump_Src();

                        newXPosition = xFactor * -1;
                        newYPosition = -10f;
                        SpriteSwitchCase = SpriteJumpDirection.JumpBottomLeft;
                    }
                    this.spriteRenderer.sortingOrder = -1;
                    isJumping = true;
                    //Debug.Log("Coily is moving to death position.");
                    MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x + newXPosition, this.transform.position.y + newYPosition));
                    SoundManagerReference.Play_CoilyFall_Src();
                    CancelInvoke("CoilyChase");
                }
                else        // Slightly buggy but to make sure coily doesn't stop when y matches
                {
                    if (transform.position.x > 0)
                    {
                        // isJumping = true;
                        // Debug.Log("Coily is at the death location +x" + new Vector3(this.transform.position.x + xFactor, -10f));
                        // MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x + xFactor, -10f));
                        Debug.Log("In ++");
                        newXPosition = xFactor;
                        newYPosition = yFactor;
                        if (this.transform.position.y + newYPosition > this.transform.position.y)
                        {
                            SpriteSwitchCase = SpriteJumpDirection.JumpTopRight;
                        }
                        else
                        {
                            SpriteSwitchCase = SpriteJumpDirection.JumpBottomRight;
                        }
                        
                    }
                    else
                    {
                        // isJumping = true;
                        // Debug.Log("Coily is at the death location -x"+ new Vector3(this.transform.position.x - xFactor, -10f));
                        // MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x - xFactor, -10f));
                        // SoundManagerReference.Play_CoilyJump_Src();

                        newXPosition = xFactor * -1;
                        newYPosition = -yFactor;
                        Debug.Log("In --");
                        if (this.transform.position.y + newYPosition > this.transform.position.y)
                        {
                            SpriteSwitchCase = SpriteJumpDirection.JumpTopLeft;
                        }
                        else
                        {
                            SpriteSwitchCase = SpriteJumpDirection.JumpBottomLeft;
                        }
                    }
                    isJumping = true;
                    //Debug.Log("Coily is moving to death position.");
                    MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x + newXPosition, this.transform.position.y + newYPosition));
                    SoundManagerReference.Play_CoilyJump_Src();
                }
            }
            else if (Qbert_Script.CoilyBlockToDieFrom.y >= this.transform.position.y)
            {
                // y is a negative factor, hence to make is positive  multiplying by -1
                newYPosition = (yFactor * -1);
                //Debug.Log("Moving Up");
                if (Qbert_Script.CoilyBlockToDieFrom.x >= this.transform.position.x)
                {
                    //Debug.Log("Moving Right Now");
                    //spriteRenderer.sprite = TopRight;
                    newXPosition = xFactor;
                    SpriteSwitchCase = SpriteJumpDirection.JumpTopRight;
                }
                else
                {
                    //Debug.Log("Moving Left Now");
                    newXPosition = xFactor * -1f;
                    SpriteSwitchCase = SpriteJumpDirection.JumpTopLeft;
                }
                isJumping = true;
                //Debug.Log("Coily is moving to death position.");
                MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x + newXPosition, this.transform.position.y + newYPosition));
                SoundManagerReference.Play_CoilyJump_Src();
            }
            else
            {
                newYPosition = (yFactor);
                //Debug.Log("Moving Downwards");
                if (Qbert_Script.CoilyBlockToDieFrom.x >= this.transform.position.x)
                {
                    //Debug.Log("Moving Right Now");
                    newXPosition = xFactor;
                    SpriteSwitchCase = SpriteJumpDirection.JumpBottomRight;
                }
                else
                {
                    //Debug.Log("Moving Left Now");
                    newXPosition = xFactor * -1;
                    SpriteSwitchCase = SpriteJumpDirection.JumpBottomLeft;
                }
                isJumping = true;
                //Debug.Log("Coily is moving to death position.");
                MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x + newXPosition, this.transform.position.y + newYPosition));
                SoundManagerReference.Play_CoilyJump_Src();
            }
            
        }
        else if (!GameManager.FreezeObjectsGreenBall)
        {
            if (Qbert.transform.position.y >= this.transform.position.y)
            {
                // y is a negative factor, hence to make is positive  multiplying by -1
                newYPosition = (yFactor * -1);
                //Debug.Log("Moving Up");
                if (Qbert.transform.position.x >= this.transform.position.x)
                {
                    //Debug.Log("Moving Right Now");
                    //spriteRenderer.sprite = TopRight;
                    newXPosition = xFactor;
                    SpriteSwitchCase = SpriteJumpDirection.JumpTopRight;
                }
                else
                {
                    //Debug.Log("Moving Left Now");
                    newXPosition = xFactor * -1f;
                    SpriteSwitchCase = SpriteJumpDirection.JumpTopLeft;
                }
            }
            else
            {
                newYPosition = (yFactor);
                //Debug.Log("Moving Downwards");
                if (Qbert.transform.position.x >= this.transform.position.x)
                {
                    //Debug.Log("Moving Right Now");
                    newXPosition = xFactor;
                    SpriteSwitchCase = SpriteJumpDirection.JumpBottomRight;
                }
                else
                {
                    //Debug.Log("Moving Left Now");
                    newXPosition = xFactor * -1;
                    SpriteSwitchCase = SpriteJumpDirection.JumpBottomLeft;
                }
            }
            Blocks block =
                FindBlockAtPosition(new Vector3(transform.position.x + newXPosition, transform.position.y + newYPosition, 0f));
            if (block == null)
            {
                //Debug.Log("Coily is going to off pyramid position.");
                return;
            }
            isJumping = true;
            //Debug.Log("Coily is in pyramid position.");
            MoveAlongBezierCurve(this.transform.position, new Vector3(this.transform.position.x + newXPosition, this.transform.position.y + newYPosition));
            SoundManagerReference.Play_CoilyJump_Src();
        }
        
    }

    IEnumerator DestroyCoily()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("Coily is destroyed !!!!!!!!!!");
        StopAllCoroutines();
        GameManager.CurrentScore += 500;
        GameManager.CoilyJumpedOff = true;
        Destroy(this.gameObject);
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
}

//  if block == null then set bezier curve to qbert position x, -10 on y
//  player has a variable for old position
//  if that position is not vector3.zero then coily will go to the old position
//  old position set when using elevator
//  so coily follows old position instead of position while on elevator