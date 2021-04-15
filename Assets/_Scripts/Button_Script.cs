using UnityEngine;
using UnityEngine.SceneManagement;

public class Button_Script : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {   
      
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ChangeScene_StartGame()
    {
        //Debug.Log("Start Game");
        PauseGame_Script.GameIsPaused = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene("Game");
    }

    public void ChangeScene_Leaderboard()
    {
        //Debug.Log("Leaderboard");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Leaderboards");
    }

    public void ChangeScene_Instructions()
    {
        //Debug.Log("Leaderboard");
        Time.timeScale = 1f;
        SceneManager.LoadScene("Instructions");
    }

    public void ChangeScene_MainMenu()
    {
        //Debug.Log("MainMenu");
        Time.timeScale = 1f;
        PauseGame_Script.GameIsPaused = false;
        SceneManager.LoadScene("MainMenu");

    }

    // Quit Game
    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
