using UnityEngine;
using System.Collections;
using System;

public class BaseScreen : MonoBehaviour {

    [System.Serializable]
    public class KeyCodeTransitionMap
    {
		public string screenName;
		public KeyCode keyCode;
    }

    public KeyCodeTransitionMap[] keyTransitionBindings;
    public float fadeDuration = 0.5f;

    protected bool inputEnabled = false;
    [SerializeField]
    protected TaskManager taskManager;

    // Use this for initialization
    protected void Awake ()
    {
        gameObject.AddComponent<Fader>();
        Transform[] recursiveChildren = GetComponentsInChildren<Transform>(true);
        foreach (Transform t in recursiveChildren)
        {
            t.gameObject.AddComponent<Fader>();
        }

        Close(false);
	}

    protected void Update()
    {
        if (inputEnabled)
        {
            foreach (KeyCodeTransitionMap element in keyTransitionBindings)
            {
                if (Input.GetKeyDown(element.keyCode))
                {
                    ScreenManager.Instance.OpenScreen(element.screenName);
                    return;
                }
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }
        }
    }

    virtual public void Open()
    {
        gameObject.SetActive(true);

        StartCoroutine(GetComponent<Fader>().FadeIn(fadeDuration, OnFadeInComplete));

        // DO NOT store off the children for later use. Components MAY get
        // enabled and disabled, and we don't want to fade them in/out
        Fader[] recursiveChildren = GetComponentsInChildren<Fader>();
        foreach (Fader fader in recursiveChildren)
        {
            StartCoroutine(fader.FadeIn(fadeDuration, null));
        }
    }

    virtual protected void OnFadeInComplete()
    {
        inputEnabled = true;
    }

    virtual public void Close(bool withAnimation = true)
    {
        inputEnabled = false;

        if (!withAnimation)
        {
            OnFadeOutComplete();
            return;
        }

        StartCoroutine(GetComponent<Fader>().FadeOut(fadeDuration, OnFadeOutComplete));

        // DO NOT store off the children for later use. Components MAY get
        // enabled and disabled, and we don't want to fade them in/out
        Fader[] recursiveChildren = GetComponentsInChildren<Fader>();
        foreach (Fader fader in recursiveChildren)
        {
            StartCoroutine(fader.FadeOut(fadeDuration, null));
        }
    }

    virtual protected void OnFadeOutComplete()
    {
        gameObject.SetActive(false);
    }

   
}
