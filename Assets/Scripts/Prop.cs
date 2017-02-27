using UnityEngine;
using System.Collections;

public class Prop : MonoBehaviour {

    public Sprite[] images;

    private SpriteRenderer spriteRenderer;
    private int currentIndex;

    // Use this for initialization
    void Awake ()
    {
//         spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
//         spriteRenderer.sortingLayerName = "Props";
//         currentIndex = 0;
//         spriteRenderer.sprite = images[currentIndex];
    }

    public void Initialize()
    {
        if (!GetComponent<SpriteRenderer>())
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }
        else
        {
            spriteRenderer = GetComponent<SpriteRenderer>();//gameObject.AddComponent<SpriteRenderer>();
        }
        spriteRenderer.sortingLayerName = "Props";
        currentIndex = 0;
        spriteRenderer.sprite = images[currentIndex];
    }

    public void TryChangeState()
    {
        if (currentIndex < images.Length - 1)
        {
            spriteRenderer.sprite = images[++currentIndex];
        }
    }
}
