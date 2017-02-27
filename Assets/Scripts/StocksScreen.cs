using UnityEngine;
using System.Collections;

public class StocksScreen : BaseScreen
{

    public KeyCode sellTriggerKey;
    public GameObject TickerObject;

    private int width = 1920;
    private int height = 1080;

    private int triggerHeight = 530;

    private Texture2D texture;

    const int UP = 1;
    const int DOWN = 2;

    private int currentX;
    private int currentY;
    private int currentHeading;

    private bool isShowingHitAnimation = false;
    private float hitAnimationDuration = 0.25f;
    private bool hasCrossedTriggerLine = false;

    [SerializeField]
    private SFXManager sfxManager;
    [SerializeField]
    private AudioClip pressButton;
    [SerializeField]
    private AudioClip wrongPress;
    [SerializeField]
    private AudioClip rightPress;

    void Awake()
    {
        base.Awake();

        texture = new Texture2D(width, height);
        TickerObject.AddComponent<GUITexture>();
        GUITexture guiTexture = TickerObject.GetComponent<GUITexture>();
        guiTexture.texture = texture;
        guiTexture.transform.position = new Vector3(0.497f, 0.66f, 0f);
        TickerObject.transform.localScale = new Vector3(0.525f, 0.48f, 0.0f);

        ClearGraph(Color.clear);
        InitValues();
    }

    override public void Open()
    {
        base.Open();
        TickerObject.SetActive(true);
    }

    override public void Close(bool withAnim)
    {
        TickerObject.SetActive(false);
        base.Close(withAnim);
    }

    void InitValues()
    {
        currentX = 0;
        currentY = Random.Range(0, triggerHeight);
        currentHeading = Random.Range(1, 3);
        hasCrossedTriggerLine = false;
    }

    void ClearGraph(Color color)
    {
        for (int i = 0; i < width; ++i)
        {
            for (int j = 0; j < height; ++j)
            {
                texture.SetPixel(i, j, color);
            }
        }
    }

    void ClearGraphHalves(Color color)
    {
        for (int i = 0; i < width; i += 2)
        {
            for (int j = 0; j < height; j += 2)
            {
                texture.SetPixel(i, j, color);
            }
        }
    }

    void FixedUpdate()
    {
        if (isShowingHitAnimation) return;

        for (int i = 0; i < width; i++)
        {
            for (int j = -5; j < 5; ++j)
            {
                texture.SetPixel(i, triggerHeight + j, Color.red);
            }
        }

        // Do it 10 times, otherwise it's too slow
        for (int i = 0; i < 10; ++i)
        {
            DrawGraph();
        }
        texture.Apply();
    }

    void Update()
    {
        base.Update();

        if (isShowingHitAnimation) return;

        if (Input.GetKeyDown(sellTriggerKey))
        {
            sfxManager.GetComponent<SFXManager>().PlaySound(pressButton);
            if (currentY >= triggerHeight)
            {
                sfxManager.GetComponent<SFXManager>().PlaySound(rightPress);
                taskManager.IncrementTask(TaskType.StockSales);
                ClearGraphHalves(Color.green);
            }
            else
            {
                sfxManager.GetComponent<SFXManager>().PlaySound(wrongPress);
                ClearGraphHalves(Color.red);
            }
            texture.Apply();
            isShowingHitAnimation = true;
            StartCoroutine("SwitchOffHitAnimation", hitAnimationDuration);
        }
    }
    

    void DrawGraph()
    {
        // Set column about to be drawn with clear line
        for (int i = 0; i < height; ++i)
        {
            texture.SetPixel(currentX, i, Color.clear);
        }

        // 10 pixels for visibility
        for (int j = currentY - 5; j < currentY + 5; ++j)
        {
            texture.SetPixel(currentX, j, Color.green);
        }

        int randomNum = Random.Range(0, 100);

        // Basically 1 in 100 chance of changing direction
        if (randomNum == 0)
        {
            if (currentHeading == UP)
            {
                currentHeading = DOWN;
            }
            else
            {
                currentHeading = UP;
            }
        }

        if (currentY >= triggerHeight + 20)
        {
            hasCrossedTriggerLine = true;
        }

        // If the graph has never gone above the trigger point in the
        // first 3/4ths of the width, make sure it does
        if (currentX >= 0.75 * width && !hasCrossedTriggerLine)
        {
            currentHeading = UP;
        }

        if (currentY == height - 1 && currentHeading == UP)
        {
            currentHeading = DOWN;
        }
        else if (currentY == 1 && currentHeading == DOWN)
        {
            currentHeading = UP;
        }

        if (currentHeading == UP)
        {
            currentY += 1;
        }
        else
        {
            currentY -= 1;
        }

        currentX++;

        if (currentX == width)
        {
            currentX = 0;
        }
    }

    void SwitchOffHitAnimation()
    {
        isShowingHitAnimation = false;
        ClearGraph(Color.clear);
        InitValues();
    }
}
