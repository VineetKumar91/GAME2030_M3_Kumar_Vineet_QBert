using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    // Prefab of Block
    public Blocks prefab_Blocks;
    private Blocks[] prefab_blocksArray;
    
    // List of Blocks for reference
    public static List<Blocks> list_blocks = new List<Blocks>();
    private List<GameObject> list_objectsInGame = new List<GameObject>();

    // Prefab of Red Ball
    public GameObject prefab_RedBall;
    private GameObject RedBall;

    // Prefab of Green Ball (friendly)
    public GameObject prefab_GreenBall;
    private GameObject GreenBall;

    // Prefab of Coily
    public GameObject prefab_Coily;
    private GameObject Coily;

    // Arrows Blinking
    public bool bKeepOnBlinking = true;
    private GameObject[] insideArrows = new GameObject[2];
    private GameObject[] outsideArrows = new GameObject[2];
    public float fblinkArrowInterval = 0.25f;

    int rows = 7; 
    // Start is called before the first frame update

    [SerializeField]
    private float xFactor = 0.96f;
    [SerializeField]
    private float yFactor = -0.7f;


    // Spawn Locations
    [Header("Enemy Spawn Location")]
    [SerializeField]
    private float spawnEnemy_X_1 = 0.48f;
    [SerializeField]
    private float spawnEnemy_X_2 = -0.48f;
    [SerializeField]
    private float spawnEnemy_Y = -0.37f;

    // Red Ball
    [SerializeField] 
    private float delay_spawnRedBall;

    private bool isReadyToSpawnRedBall = true;

    // Green Ball
    [SerializeField]
    private float delay_spawnGreenBall;

    private bool isReadyToSpawnGreenBall = true;

    // Purple Ball / Coily
    [SerializeField]
    private float delay_spawnPurpleBall;

    private bool isReadyToSpawnPurpleBall = true;

    private bool isCoilyStillAlive = false;
    public static bool CoilyJumpedOff = false;
    public  bool CoilyJumpedOffExecuteOnce = false;


    public static int CurrentScore = 0;

    [SerializeField]
    private TextMeshProUGUI scoreTMP;

    // Elevator Count
    public static int ElevatorCount = 2;
    public static int BlocksRemainingCount = 28;

    // Game Reset, and objects getting affected due to Qbert
    public static bool ResetGame = false;
    private bool resetExecuteOnce = false;
    public static bool FreezeObjectsGreenBall = false;
    private bool freezeObjectsExecuteOnce = false;
    // Green ball affect
    [SerializeField] 
    private Camera Maincamera;

    private bool readyToChangeBackgroundColor = false;

    public GameObject QbertLife1;
    public GameObject QbertLife2;
    private int QbertLives = 3;

    [SerializeField] 
    private Qbert_Script QbertScript;

    // Win Trigger
    public static bool WinTrigerred = false;
    [SerializeField] 
    private TextMeshProUGUI BonusTextOnWin;

    // Sounds
    private SoundManager SoundManagerReference;

    void Start()
    {
        // Design Layout 1 by 1
        createBlocks(rows);
        // Get the blinking arrows
        insideArrows[0] = GameObject.Find("InsideArrow_Left");
        insideArrows[1] = GameObject.Find("InsideArrow_Right");
        outsideArrows[0] = GameObject.Find("OutsideArrow_Left");
        outsideArrows[1] = GameObject.Find("OutsideArrow_Right");
        for (int i = 0; i < 2; i++)
        {
            insideArrows[i].SetActive(false);
            outsideArrows[i].SetActive(false);
        }
        StartCoroutine(BlinkArrow());
        CurrentScore = 0;
        BonusTextOnWin.gameObject.SetActive(false);
        WinTrigerred = false;
        CoilyJumpedOff = false;
        // Sounds
        SoundManagerReference = GameObject.FindObjectOfType<SoundManager>();
    }

    // Reset all static variables !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    void Awake()
    {
        list_blocks = new List<Blocks>();
        CurrentScore = 0;
        ElevatorCount = 2;
        BlocksRemainingCount = 28;
        ResetGame = false;
        FreezeObjectsGreenBall = false;
        QbertLives = 3;
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.Escape))
        //{ 
        //    Debug.Log("Escape Key Pressed");
        //    //  QuitGame();         <--- M1 change to M2 requirement
        //}

        // Reset game, by Qbert's Position, Redball, Greenball and Coily reset - score and lives intact
        if (ResetGame && !resetExecuteOnce)
        {
            resetExecuteOnce = true;
            StartCoroutine(ResetAsQbertDead());
        }

        // Specifically for not letting any objects spawn
        if (ResetGame)
        {
            //Debug.Log("In Destroy All Objects Coroutine");
            foreach (GameObject otherobjects in list_objectsInGame)
            {

                UnityEngine.Object.Destroy((UnityEngine.Object)otherobjects);
            }
            this.list_objectsInGame.Clear();
        }

        // Wait for 5 seconds before letting any enemy spaw
        if (CoilyJumpedOff && !CoilyJumpedOffExecuteOnce)
        {
            CoilyJumpedOffExecuteOnce = true;
            isReadyToSpawnPurpleBall = false;
            isReadyToSpawnRedBall = false;
            StopCoroutine("SpawnRedBall");
            //Debug.Log("In Destroy All Objects Coroutine");
            // remove coily
            for (var i = list_objectsInGame.Count - 1; i > -1; i--)
            {
                if (list_objectsInGame[i] == null)
                {
                    list_objectsInGame.RemoveAt(i);
                }
            }

            // remove all enemies
            foreach (GameObject otherobjects in list_objectsInGame)
            {
                if (otherobjects.gameObject.CompareTag("Enemy"))
                {
                    UnityEngine.Object.Destroy((UnityEngine.Object)otherobjects);
                }
            }
            StartCoroutine(CoilyJumpedOffCoRoutine());
        }

        // Causing null pointer error, but runnable
        //if (CoilyJumpedOff)
        //{
        //    // remove all enemies
        //    foreach (GameObject otherobjects in list_objectsInGame)
        //    {
        //        if (otherobjects.gameObject.CompareTag("Enemy"))
        //        {
        //            UnityEngine.Object.Destroy((UnityEngine.Object)otherobjects);
        //        }
        //    }
        //}

        // Greenball has collided, now freeze/unfreeze enemies
        if (FreezeObjectsGreenBall && !freezeObjectsExecuteOnce)
        {
            freezeObjectsExecuteOnce = true;
            StartCoroutine(FreezeObjects());
        }

       // gamemanager is ready to spawn redball
        if (isReadyToSpawnRedBall)
        {
            StartCoroutine(SpawnRedBall());
        }
        
        // gamemanager ready to spawn greenball
        if (isReadyToSpawnGreenBall)
        {
            StartCoroutine(SpawnGreenBall());
        }

        // If coily is dead, then spawn the purple ball and gamemanager is ready
        if (isReadyToSpawnPurpleBall && !isCoilyStillAlive)
        {
            StartCoroutine(SpawnPurpleBall());
            //isCoilyStillAlive = true;
        }

        // Score update
        scoreTMP.text = CurrentScore.ToString();

        // If All blocks have been completed !
        if (BlocksRemainingCount <= 0 && !WinTrigerred)
        {
            StartCoroutine(GameWon());
        }

    }


    // Create the layout for the game (starting with block)
    private void createBlocks(int rows)
    {
        float xPos = 0.0f;
        float yPos = 0.0f;
        // i = 0
        // j = 0
        int k = 0;

        float firstX = 0.0f;
        float firstY = 0.0f;

        // Number of rows for the pyramid
        /*for (int i = rows - 1; i >= 0; i--)
        {
            //Debug.Log(i);
            yPos = yFactor * i;
           // xPos = -0.48f * i;
            xPos = firstX;
            Debug.Log("xPos = " + xPos);
            // Number of columns of blocks in each row
            for (int j = 0; j <= i; j++, k++)
            {
                // Instantiate creates the object during run-time
                // position factor -> +- -0.48, -0.7
                list_blocks.Add(Instantiate(prefab_Blocks, new Vector2(xPos, yPos), Quaternion.identity));
                
                xPos += xFactor;

            }

            firstX = firstX + xFactor;
            //Debug.Log("firstX = " + firstX);
        }*/
        for (int i = 0; i < rows ; i++)
        {
            //Debug.Log(i);
            //yPos = yFactor * i;
            // xPos = -0.48f * i;
            xPos = firstX;
            yPos = firstY;
            //Debug.Log("xPos = " + xPos);
            // Number of columns of blocks in each row
            for (int j = 0; j <= i; j++, k++)
            {
                // Instantiate creates the object during run-time
                // position factor -> +- -0.48, -0.7
                list_blocks.Add(Instantiate(prefab_Blocks, new Vector2(xPos, yPos), Quaternion.identity));

                xPos += xFactor;
                
            }

            firstX -= 0.5f;
            firstY -= yFactor;
            //Debug.Log("firstX = " + firstX);
        }

        foreach (Blocks block in list_blocks)
        {
            block.GetComponent<Animator>().enabled = false;
        }
    }

    // Initialize Red ball, Green ball and Coily
    private void initializeRedBall(float xPos, float yPos)
    {
        RedBall = Instantiate(prefab_RedBall, new Vector2(xPos, yPos), Quaternion.identity);
        list_objectsInGame.Add(RedBall);
    }

    private void initializeGreenBall(float xPos, float yPos)
    {
        GreenBall = Instantiate(prefab_GreenBall, new Vector2(xPos, yPos), Quaternion.identity);
        list_objectsInGame.Add(GreenBall);
    }

    private void initializeCoily(float xPos, float yPos)
    {
        Coily = Instantiate(prefab_Coily, new Vector2(xPos, yPos), Quaternion.identity);
        list_objectsInGame.Add(Coily);
        isCoilyStillAlive = true;
    }

    public IEnumerator SpawnRedBall()
    {
        if (!ResetGame || !FreezeObjectsGreenBall || !CoilyJumpedOff)
        {
            isReadyToSpawnRedBall = false;
            yield return new WaitForSeconds(2);
            //Debug.Log("Spawned");
            if (Random.Range(1, 10) % 2 == 0)
            {
                initializeRedBall(spawnEnemy_X_1, spawnEnemy_Y);
            }
            else
            {
                initializeRedBall(spawnEnemy_X_2, spawnEnemy_Y);
            }
            yield return new WaitForSeconds(delay_spawnRedBall);
            isReadyToSpawnRedBall = true;
            yield break;
        }
    }

    public IEnumerator SpawnGreenBall()
    {
        if (!ResetGame || !FreezeObjectsGreenBall)
        {
            isReadyToSpawnGreenBall = false;
            yield return new WaitForSeconds(6);
            //Debug.Log("Spawned Green ball");
            if (Random.Range(1, 10) % 2 == 0)
            {
                initializeGreenBall(spawnEnemy_X_1, spawnEnemy_Y);
            }
            else
            {
                initializeGreenBall(spawnEnemy_X_2, spawnEnemy_Y);
            }
            yield return new WaitForSeconds(delay_spawnGreenBall);
            isReadyToSpawnGreenBall = true;
        }
    }

    public IEnumerator SpawnPurpleBall()
    {
        if ((!ResetGame || !FreezeObjectsGreenBall))
        {
            Debug.Log("Spawning Purple ball");
            isReadyToSpawnPurpleBall = false;
            yield return new WaitForSeconds(4);
            //Debug.Log("Spawned");
            if (isCoilyStillAlive)
            {
                yield break;
            }
            if (Random.Range(1, 10) % 2 == 0)
            {
                initializeCoily(spawnEnemy_X_1, spawnEnemy_Y);
            }
            else
            {
                initializeCoily(spawnEnemy_X_2, spawnEnemy_Y);
            }
            yield return new WaitForSeconds(delay_spawnPurpleBall);
            isReadyToSpawnPurpleBall = true;
        }
    }

    // Blink Arrow Function
    public IEnumerator BlinkArrow()
    {
        // Use Renderer
        /*SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();
        while (bKeepOnBlinking)
        {
            renderer.enabled = !renderer.enabled;
            yield return new WaitForSeconds(seconds);
        }*/

        //SpriteRenderer renderer = gameObject.GetComponent<SpriteRenderer>();

        while (bKeepOnBlinking)
        {
            //Debug.Log("In Blink Arrow");

            // Animation 1 - Only Outside Arrows are Visible
            yield return new WaitForSeconds(fblinkArrowInterval);
            for (int i = 0; i < 2; i++)
            {
                outsideArrows[i].SetActive(true);
            }
            
            // Animation 2 - Inside Arrows are also Visible
            yield return new WaitForSeconds(fblinkArrowInterval);
            for (int i = 0; i < 2; i++)
            {
                insideArrows[i].SetActive(true);
            }

            // Animation 3 - Both arrows are not visible
            yield return new WaitForSeconds(fblinkArrowInterval);
            for (int i = 0; i < 2; i++)
            {
                outsideArrows[i].SetActive(false);
                insideArrows[i].SetActive(false);
            }
        }
    }

    IEnumerator ResetAsQbertDead()
    {
        StopCoroutine("SpawnGreenBall");
        StopCoroutine("SpawnRedBall");
        StopCoroutine("SpawnPurpleBall");
        yield return new WaitForSeconds(3f);
        QbertScript.ResetQbert();
        ResetGame = false;
        resetExecuteOnce = false;
        if (QbertLives == 3)
        {
            Destroy(QbertLife1);
        }
        else if (QbertLives == 2)
        {
            Destroy(QbertLife2);
        }

        QbertLives--;
        
        // Game Over
        if (QbertLives == 0)
        {
            GameOver_Script.gameStatusForGameOver = "Game Over :(";
            StopAllCoroutines();
            SceneManager.LoadScene(3);
        }

        if (!CoilyJumpedOff)
        {
            isCoilyStillAlive = false;
        }
        
    }

    // When Game has been won, enable animation of the blocks
    private IEnumerator GameWon()
    {
        SoundManagerReference.Play_Victory_Src();
        foreach (Blocks block in list_blocks)
        {
            block.GetComponent<Animator>().enabled = true;
        }
        WinTrigerred = true;    // Making sure this only runs once
        // Stop and remove everything apart from Qbert seen on the screen
        
        foreach (GameObject otherobjects in list_objectsInGame)
        {

            UnityEngine.Object.Destroy((UnityEngine.Object)otherobjects);
        }
        this.list_objectsInGame.Clear();
        
        BonusTextOnWin.gameObject.SetActive(true);
        // Score for Clearing Level
        CurrentScore += (ElevatorCount * 100);
        CurrentScore += 1000;
        
        yield return new WaitForSeconds(3);
        // To Do: Check for high score, and in case of that, redirect to high score scene instead of Main Menu scene
        // Load Main Menu
        GameOver_Script.gameStatusForGameOver = "You Won! :)";
        SceneManager.LoadScene(3);
        StopAllCoroutines();
    }

    private IEnumerator FreezeObjects()
    {
        // Destroy green ball first
        Destroy(GreenBall);
        //readyToChangeBackgroundColor = true;
        InvokeRepeating("ChangeBackgroundColor",0.0f,0.1f);
        yield return new WaitForSeconds(5);
        CancelInvoke("ChangeBackgroundColor");
        Maincamera.backgroundColor = Color.black;
        //readyToChangeBackgroundColor = false;
        
        // Now reset the bool - global greenball bool and execute once bool
        GameManager.FreezeObjectsGreenBall = false;
        freezeObjectsExecuteOnce = false;
    }

    // remove enemies and wait for 5 seconds before spawning
    private IEnumerator CoilyJumpedOffCoRoutine()
    {
        for (var i = list_objectsInGame.Count - 1; i > -1; i--)
        {
            if (list_objectsInGame[i] == null)
            {
                list_objectsInGame.RemoveAt(i);
            }
        }
        yield return new WaitForSeconds(5);
        isReadyToSpawnPurpleBall = true;
        // purple ball
        isCoilyStillAlive = false;
        // static bool
        CoilyJumpedOff = false;
        // execute once 
        CoilyJumpedOffExecuteOnce = false;
        StopCoroutine("CoilyJumpedOffCoRoutine");
        yield break;
    }

    private void ChangeBackgroundColor()
    {
        Color background = new Color(
            Random.Range(0f, 1f),
            Random.Range(0f, 1f),
            Random.Range(0f, 1f)
        );

        Maincamera.backgroundColor = background;
    }

}
