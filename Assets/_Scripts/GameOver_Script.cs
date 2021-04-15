using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameOver_Script : MonoBehaviour
{

    [SerializeField] 
    private GameObject HighScorePanel;

    [SerializeField] 
    private Button OkGameOverButton;

    [SerializeField]
    private Button OKHighScoreButton;

    [SerializeField] 
    private TMP_InputField InputField;

    [SerializeField] private TextMeshProUGUI TMP_GameOver;

    public static bool HighScoreBool = false;

    private string name = "";
    private int score;

    public static string gameStatusForGameOver = "Game Over :(";

    private string jsonString;

    private HighScoreTable_Script.Highscores highscores;
    //[SerializeField] privatestatic HighScoreTable_Script highScoreTable;

    // Start is called before the first frame update
    void Start()
    {
        //name = "TES";

        score = GameManager.CurrentScore;
        Debug.Log("In Game over & Score = " + score);

        TMP_GameOver.text = gameStatusForGameOver;


        jsonString = PlayerPrefs.GetString("highscoreTableX");

        highscores = JsonUtility.FromJson<HighScoreTable_Script.Highscores>(jsonString);

        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    HighScoreTable_Script.HighScoreEntry temp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = temp;
                }
            }
        }

        // Check if the score is greater than any score in Top 10
        for (int i = 9; i >= 0; i--)
        {
            if (score > highscores.highscoreEntryList[i].score)
            {
                Debug.Log("Greater than 1 of the top 10");
                
                HighScoreBool = true;
                break;
            }
        }



        //Debug.Log("After Sorting");
        //for (int i = 0; i < 10; i++)
        //{
        //    Debug.Log("New Slot Entry = " + highscores.highscoreEntryList[i].name + " " + highscores.highscoreEntryList[i].score);
        //}

        if (HighScoreBool)
        {
            HighScorePanel.gameObject.SetActive(true);
            OkGameOverButton.gameObject.SetActive(false);
            InputField.characterLimit = 3;
        }
        else
        {
            HighScorePanel.gameObject.SetActive(false);
            OkGameOverButton.gameObject.SetActive(true);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OkButtonPress()
    {
        name = InputField.text;
        if (HighScoreBool)
        {

            SceneManager.LoadScene("MainMenu");
            if (InputField.text.Length == 0)
            {
                name = "ZZZ";
            }
            else
            {
                name = InputField.text;
            }

            HighScoreTable_Script.HighScoreEntry highscoreEntry = new HighScoreTable_Script.HighScoreEntry {score = score, name = name};
            highscores.highscoreEntryList.Add(highscoreEntry);
            string json = JsonUtility.ToJson(highscores);
            // Save using PlayerPrefs and JSON format
            PlayerPrefs.SetString("highscoreTableX", json);
            PlayerPrefs.Save();
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }
}
