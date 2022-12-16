using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimeUI : MonoBehaviour
{
    TextMeshProUGUI textMesh;

    public float currentTime = 0.0f;

    void Start()
    {
        textMesh = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        currentTime += Time.deltaTime;
        textMesh.text
            = currentTime.ToString("0.00");
    }
}
