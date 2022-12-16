using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameWonUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI textMesh;

    public static float time = 0;
    public static int coins = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateData()
    {
        textMesh.text = string.Format("Gathered {0} coins and finished the level in {1} seconds", coins, time.ToString("0.00"));
    }

    public void loadMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("MainMenu");
    }
}
