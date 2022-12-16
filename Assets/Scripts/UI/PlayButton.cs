using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    [SerializeField]
    public GameMode gameMode;
    [SerializeField]
    public MainMenu mainMenu;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Button>().onClick.AddListener(delegate { mainMenu.playGame(gameMode); });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
