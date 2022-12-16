using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class HighscoreMenu : MonoBehaviour
{
    [SerializeField]
    public TextMeshProUGUI textMeshEasy;
    [SerializeField]
    public TextMeshProUGUI textMeshMedium;
    [SerializeField]
    public TextMeshProUGUI textMeshHard;

    // Start is called before the first frame update
    void Start()
    {
        loadData("easy", textMeshEasy);
    }

    private void loadData(string mode, TextMeshProUGUI textMesh)
    {
        if (PlayerPrefs.HasKey(mode))
        {
            textMesh.text = PlayerPrefs.GetFloat(mode).ToString("0.00");
        } 
        else
        {
            textMesh.text = "NA";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void loadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
