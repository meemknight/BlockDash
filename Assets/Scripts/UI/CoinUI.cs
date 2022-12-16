using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoinUI : MonoBehaviour
{
    [SerializeField]
    public RectTransform textObj;
    [SerializeField]
    public TimeUI timeUI;
    
    private TextMeshProUGUI textMesh;
    public Player player;

    private int currentCoins = -1;
    public int CurrentCoins
    {
        get => currentCoins;
        set
        {
            currentCoins = value;
            updateText();
        }
    }

    private int neededCoins = -1;
    public int NeededCoins
    {
        get => neededCoins;
        set
        {
            neededCoins = value;
            updateText();
        }
    }

    public void updateText()
    {
        Text = CurrentCoins.ToString() + "/" + NeededCoins.ToString();
    }

    private string text;
    public string Text
    {
        get => text;
        set
        {
            text = value;
            textMesh.SetText(value);
        }
    }

    public void collectCoin()
    {
        CurrentCoins += 1;
        if (CurrentCoins == NeededCoins)
        {
            LevelManager.levelFinished(timeUI.currentTime);
            GameWonUI.coins = currentCoins;
            GameWonUI.time = timeUI.currentTime;

            player.levelManager.loadWonMenu();
            // SceneManager.LoadScene("GameWon");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        textMesh = textObj.GetComponent<TextMeshProUGUI>();
        CurrentCoins = 0;
        NeededCoins = LevelManager.coins;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
