using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseComponent : MonoBehaviour
{
    [SerializeField]
    LevelManager levelManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            levelManager.IsPaused = !levelManager.IsPaused;
            Time.timeScale = (levelManager.IsPaused) ? 0 : 1;
            levelManager.pauseMenu.enabled = levelManager.IsPaused;
        }
    }
}
