using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenBall_Script : BallMovement
{

    [SerializeField] private float xFactor = 0.48f;
    [SerializeField] private float yFactor = -0.7f;

    [SerializeField] private SpriteRenderer spriteRenderer;
    private Sprite currentSprite;

    [Header("Sprites")] public Sprite Idle;
    public Sprite Jump;

    private bool spawnLocationCheck = true;

    public Vector3 position0 = Vector3.zero;
    public Vector3 position1 = Vector3.zero;
    public Vector3 position2 = Vector3.zero;

    public bool isJumping = true;

    [SerializeField] private float BezierCurveFactor;
    private float tParamBezierParameter = 0f;

    private bool killGreenBallExecuteOnce = true;

    // Sounds
    private SoundManager SoundManagerReference;


    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Green Ball Spawned!");
        spriteRenderer.sprite = Idle;

        SoundManagerReference = GameObject.FindObjectOfType<SoundManager>();
    }

    void FixedUpdate()
    {
        // Entry
        if (transform.position.y > -0.38)
        {
            Vector2 tempPos = Vector3.Lerp(transform.position, new Vector3(transform.position.x, -0.38f, 0f),
                Time.deltaTime * 15f);
            transform.position = tempPos;
        }

        // Exit
        if (this.transform.position.y < -4)
        {
            Vector2 tempPos = Vector3.Lerp(transform.position, new Vector3(transform.position.x, -7, 0f),
                Time.deltaTime * 10f);
            transform.position = tempPos;

            // Destroy the ball when its offscreen bottom
            if (this.transform.position.y <= -6 && killGreenBallExecuteOnce)
            {
                StartCoroutine(DestroyGreenBall());
                killGreenBallExecuteOnce = false;
            }
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
            InvokeRepeating("MoveToNextPosition", 0, 3);
        }

        if (tParamBezierParameter < 1.0 && isJumping)
        {
            this.tParamBezierParameter += BezierCurveFactor * Time.deltaTime;
            this.transform.position = Vector3.Lerp(Vector3.Lerp(this.position0, this.position1, tParamBezierParameter),
                Vector3.Lerp(this.position1, this.position2, tParamBezierParameter), tParamBezierParameter);
        }

        if (this.transform.position == this.position2)
        {
            isJumping = false;
            tParamBezierParameter = 0;
            spriteRenderer.sprite = Idle;
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
        spriteRenderer.sprite = Jump;
        isJumping = true;
        //Debug.Log("Check");
        float newXPosition = (Random.Range(1, 10) % 2 == 0) ? xFactor : (xFactor * -1f);
        MoveAlongBezierCurve(this.transform.position,
            new Vector3(this.transform.position.x + newXPosition, this.transform.position.y + yFactor));
        //Debug.Log("Next Position = " + new Vector3(this.transform.position.x + newXPosition, this.transform.position.y + yFactor));
        SoundManagerReference.Play_BallJump_Src();
    }

    IEnumerator DestroyGreenBall()
    {
        yield return new WaitForSeconds(1.0f);
        //Debug.Log("You are out of zone green");
        Destroy(this.gameObject);
    }
}
