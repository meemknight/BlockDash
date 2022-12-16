using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public enum GameMode
{
    EASY,
    MEDIUM,
    HARD
}

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    public GameObject[] buttons;
    [SerializeField]
    public GameObject playButton;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void highscore()
    {
        SceneManager.LoadScene("Highscore");
    }

    public void playGame(GameMode mode)
    {
        LevelManager.gameMode = mode;
        SceneManager.LoadScene("Game");
    }

    public void playSelected()
    {
        playButton.SetActive(false);
        foreach (GameObject button in buttons)
        {
            button.SetActive(true);
        }
    }

    public void quit()
    {
        Application.Quit();
    }
}
