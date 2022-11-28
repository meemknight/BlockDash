using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LifeUI : MonoBehaviour
{
    Image image;
    RectTransform rectTransform;

    [SerializeField]
    Player player;

    private int lives;
    public int Lives
    {
        get => lives;
        set
        {
            if (value >= 0)
            {
                lives = value;
                rectTransform.sizeDelta = new Vector2(image.sprite.texture.width * value, rectTransform.rect.height);
            }
            if (value <= 0)
            {
                player.kill();
            }
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        image = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
