using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// stiu ca trebuia facut mai generic si controlat din alt script care reprezenta progress baru asta automat care se asigura ca playerul nu stationeaza prea mult but cmon, it works si nu avem nevoie de alt progress bar sper
public class ProgressBar : MonoBehaviour
{
    [SerializeField]
    int width = 200;
    [SerializeField]
    int height = 10;

    [SerializeField]
    Color backgroundColor = Color.white;
    [SerializeField]
    Color sliderColor = Color.red;

    [SerializeField]
    float fillDuration = 1.0f;
    [SerializeField]
    float fadeDuration = .2f;

    [SerializeField]
    bool beginHidden = true;

    RectTransform rt;
    RectTransform slider;
    CanvasGroup canvasGroup;

    bool fadingIn = false;
    bool fadingOut = false;

    bool filling = false;
    const int fillingOutMultiplier = 2;

    private float fillProgress = 0;
    public float FillProgress
    {
        get => fillProgress;
        set
        {
            fillProgress = Mathf.Clamp(value, 0, fillDuration);
            float perc = fillProgress / fillDuration;

            slider.sizeDelta = new Vector2(width * perc, height);
        }
    }

    private float fadeProgress = 0;
    public float FadeProgress
    {
        get => fillProgress;
        set
        {
            fadeProgress = Mathf.Clamp(value, 0, fadeDuration);
            float perc = fadeProgress / fadeDuration;

            if (fadeProgress == 0.0f) fadingOut = false;
            if (fadeProgress == fadeDuration) fadingIn = false;

            canvasGroup.alpha = perc;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        rt = GetComponent<RectTransform>();
        slider = transform.GetChild(0).GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;

        if (!beginHidden)
        {
            fadeIn();
        }

        rt.sizeDelta = new Vector2(width, height);
        slider.sizeDelta = new Vector2(0, height);

        GetComponent<Image>().color = backgroundColor;
        slider.GetComponent<Image>().color = sliderColor;
    }

    // Update is called once per frame
    void Update()
    {
        if (filling)
        {
            FillProgress += Time.deltaTime;
        } 
        else
        {
            FillProgress -= Time.deltaTime * fillingOutMultiplier;
        }

        if (fadingIn)
        {
            FadeProgress += Time.deltaTime;
        } 
        else if (fadingOut)
        {
            FadeProgress -= Time.deltaTime;
        }
    }

    public void fadeOut()
    {
        fadingOut = true;
        fadingIn = false;
    }

    public void fadeIn()
    {
        fadingIn = true;
        fadingOut = false;
    }

    public void startFilling()
    {
        filling = true;
    }

    public void stopFilling()
    {
        filling = false;
    }

    public bool isFilled()
    {
        return FillProgress == fillDuration;
    }

    public void reset()
    {
        fadingIn = false;
        fadingOut = false;

        filling = false;

        FillProgress = 0.0f;
        FadeProgress = 0.0f;

        if (!beginHidden)
        {
            fadeIn();
        }
    }
}
