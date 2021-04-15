using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey("highscoreTableX"))
        {
            Debug.Log("Key not there.");
            HighScoreTable_Script.Highscores highscores1 = new HighScoreTable_Script.Highscores();
            highscores1.highscoreEntryList = new List<HighScoreTable_Script.HighScoreEntry>();

            HighScoreTable_Script.HighScoreEntry highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 200, name = "ACC" };
            highscores1.highscoreEntryList.Add(highscoreEntry);
            highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 300, name = "ACS" };
            highscores1.highscoreEntryList.Add(highscoreEntry);
            highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 1475, name = "FBI" };
            highscores1.highscoreEntryList.Add(highscoreEntry);
            highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 900, name = "CSI" };
            highscores1.highscoreEntryList.Add(highscoreEntry);
            highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 150, name = "WHA" };
            highscores1.highscoreEntryList.Add(highscoreEntry);
            highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 1100, name = "THE" };
            highscores1.highscoreEntryList.Add(highscoreEntry);
            highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 1750, name = "QUE" };
            highscores1.highscoreEntryList.Add(highscoreEntry);
            highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 350, name = "SQL" };
            highscores1.highscoreEntryList.Add(highscoreEntry);
            highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 1200, name = "CSS" };
            highscores1.highscoreEntryList.Add(highscoreEntry);
            highscoreEntry = new HighScoreTable_Script.HighScoreEntry { score = 2000, name = "VKC" };
            highscores1.highscoreEntryList.Add(highscoreEntry);

            string json = JsonUtility.ToJson(highscores1);

            // Save using PlayerPrefs and JSON format
            PlayerPrefs.SetString("highscoreTableX", json);
            PlayerPrefs.Save();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
