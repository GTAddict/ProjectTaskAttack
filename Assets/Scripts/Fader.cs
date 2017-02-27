using UnityEngine;
using System.Collections;
using System;

public class Fader : MonoBehaviour {

    private Renderer theRenderer;
    private Color originalColor;

	// Use this for initialization
	void Start ()
    {
        if (GetComponent<Renderer>() != null)
        {
            theRenderer = GetComponent<Renderer>();
            originalColor = new Color(
                theRenderer.material.color.r,
                theRenderer.material.color.g,
                theRenderer.material.color.b,
                theRenderer.material.color.a
                );
        }
	}

    public IEnumerator FadeOut(float fadeDuration, Action callback)
    {
       Color finalColor = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        for (float time = 0; time < fadeDuration; time += Time.deltaTime)
        {
            // This check is done inside and not at the root level, because even if the material
            // doesn't exist, we would want the callback to be fired once the time is complete,
            // so we want the time loop to execute
            if (theRenderer != null)
            {
                theRenderer.material.color = Color.Lerp(originalColor, finalColor, time / fadeDuration);
            }

            yield return null;
        }

        if (callback != null)
        {
            callback();
        }
    }

    public IEnumerator FadeIn(float fadeDuration, Action callback)
    {
        Color currentColor = new Color(0, 0, 0, 0);

        if (theRenderer != null)
        {
            currentColor = new Color(
            theRenderer.material.color.r,
            theRenderer.material.color.g,
            theRenderer.material.color.b,
            theRenderer.material.color.a
            );
        }

        for (float time = 0; time < fadeDuration; time += Time.deltaTime)
        {
            // This check is done inside and not at the root level, because even if the material
            // doesn't exist, we would want the callback to be fired once the time is complete,
            // so we want the time loop to execute
            if (theRenderer != null)
            {
                theRenderer.material.color = Color.Lerp(currentColor, originalColor, time / fadeDuration);
            }

            yield return null;
        }

        if (callback != null)
        {
            callback();
        }
    }
}
