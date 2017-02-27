using UnityEngine;
using System.Collections;

public class SFXManager : MonoBehaviour {

    public void PlaySound(AudioClip sound)
    {
        GetComponent<AudioSource>().PlayOneShot(sound);
    }

    public void FadeIn(AudioSource source, float duration)
    {
        StartCoroutine(FadeInImp(source, duration));
    }

    public void FadeOut(AudioSource source, float duration)
    {
        StartCoroutine(FadeOutImp(source, duration));
    }

    private IEnumerator FadeOutImp(AudioSource source, float duration)
    {
        float startVolume = source.volume;

        while (source.volume > 0)
        {
            source.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }
    }

    private IEnumerator FadeInImp(AudioSource source, float duration)
    {
        while (source.volume < 1)
        {
            source.volume += Time.deltaTime / duration;
            yield return null;
        }
    }
}
