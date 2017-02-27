using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ScreenManager : MonoBehaviour {

    private static ScreenManager _instance;

    public string startScreen;


    public static ScreenManager Instance {  get { return _instance; } }

    private ArrayList screens;
    private BaseScreen activeScreen;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    void Start()
    {
        screens = new ArrayList(GetComponentsInChildren<BaseScreen>(true));
        OpenScreen("Typing");
    }

    public void OpenScreen(string screenName)
    {
        if (activeScreen != null)
        {
            activeScreen.Close();
        }

        foreach (BaseScreen s in screens)
        {
            if (s.name == screenName)
            {
                activeScreen = s;
                activeScreen.Open();
                break;
            }
        }
    }
}
