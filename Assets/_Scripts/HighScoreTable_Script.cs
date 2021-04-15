using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HighScoreTable_Script : MonoBehaviour
{
    [SerializeField]
    private Transform entryContainer;
    [SerializeField]
    private Transform entryTemplate;

    private List<Transform> highScoreEntryTransformList;
    
    private void Awake()
    {
        //entryContainer = transform.Find("HighScoreContainer");
        //entryTemplate = transform.Find("HighscoreEntryTemplate");
        
        // THIS CODE HAS BEEN SHIFTED TO MAIN MENU SINCE PLAYER IS GOING TO GO TO MENU FIRST AND NOT! LEADERBOARDS!!!!!!!

        //if (!PlayerPrefs.HasKey("highscoreTableX"))
        //{ 
        //    Debug.Log("Key not there.");
        //    Highscores highscores1 = new Highscores();
        //    highscores1.highscoreEntryList = new List<HighScoreEntry>();

        //    HighScoreEntry highscoreEntry = new HighScoreEntry { score = 200, name = "ACC" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);
        //    highscoreEntry = new HighScoreEntry { score = 300, name = "ACC" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);
        //    highscoreEntry = new HighScoreEntry { score = 1475, name = "FBI" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);
        //    highscoreEntry = new HighScoreEntry { score = 900, name = "CSI" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);
        //    highscoreEntry = new HighScoreEntry { score = 150, name = "WHA" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);
        //    highscoreEntry = new HighScoreEntry { score = 1100, name = "THE" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);
        //    highscoreEntry = new HighScoreEntry { score = 1750, name = "QUE" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);
        //    highscoreEntry = new HighScoreEntry { score = 350, name = "SQL" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);
        //    highscoreEntry = new HighScoreEntry { score = 1200, name = "CSS" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);
        //    highscoreEntry = new HighScoreEntry { score = 2000, name = "VKC" };
        //    highscores1.highscoreEntryList.Add(highscoreEntry);

        //    string json = JsonUtility.ToJson(highscores1);

        //    // Save using PlayerPrefs and JSON format
        //    PlayerPrefs.SetString("highscoreTableX", json);
        //    PlayerPrefs.Save();
        //}


        string jsonString = PlayerPrefs.GetString("highscoreTableX");

        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Bubble Sort - optimized
        for (int i = 0; i < highscores.highscoreEntryList.Count; i++)
        {
            for (int j = i + 1; j < highscores.highscoreEntryList.Count; j++)
            {
                if (highscores.highscoreEntryList[j].score > highscores.highscoreEntryList[i].score)
                {
                    HighScoreEntry temp = highscores.highscoreEntryList[i];
                    highscores.highscoreEntryList[i] = highscores.highscoreEntryList[j];
                    highscores.highscoreEntryList[j] = temp;
                }
            }
        }

        int count = 1;
        highScoreEntryTransformList = new List<Transform>();
        foreach (HighScoreEntry highScoreEntry in highscores.highscoreEntryList)
        {
            CreateHighscoreEntryTransform(highScoreEntry, entryContainer, highScoreEntryTransformList, count);
            count++;
            if (count > 10)
            {
                break;
            }
        }
    }

    public void CreateHighscoreEntryTransform(HighScoreEntry highScoreEntry, Transform container, List<Transform> transformList, int count)
    {
        float templateHeight = 35;
        Transform entryTransform = Instantiate(entryTemplate, container);
        RectTransform entryRectTransform = entryTransform.GetComponent<RectTransform>();
        entryRectTransform.anchoredPosition = new Vector2(0, -templateHeight * transformList.Count);
        entryTransform.gameObject.SetActive(true);

        int score = highScoreEntry.score;

        entryTransform.Find("ScoreText").GetComponent<Text>().text = score.ToString();

        string name = highScoreEntry.name;
        entryTransform.Find("NameText").GetComponent<Text>().text = name;

        if (count == 1)
        {
            entryTransform.Find("ScoreText").GetComponent<Text>().color = Color.cyan;
            entryTransform.Find("NameText").GetComponent<Text>().color = Color.cyan;
        }

        transformList.Add(entryTransform);
    }

    public void AddScoreEntry(int score, string name)
    {
        HighScoreEntry highscoreEntry = new HighScoreEntry {score = score, name = name};
        string jsonString = PlayerPrefs.GetString("highscoreTableX");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);
        Debug.Log("highscores obj = " + highscores.highscoreEntryList[0].name);
        highscores.highscoreEntryList.Add(highscoreEntry);

        string json = JsonUtility.ToJson(highscores);

        // Save using PlayerPrefs and JSON format
        PlayerPrefs.SetString("highscoreTableX", json);
        PlayerPrefs.Save();
    }


    public class Highscores
    {
        public List<HighScoreEntry> highscoreEntryList;
    }

    // Class for high score entry
    [System.Serializable]
    public class HighScoreEntry
    {
        public int score;
        public string name;
    }
}
