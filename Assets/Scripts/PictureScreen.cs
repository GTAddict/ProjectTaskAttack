using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class PictureScreen : BaseScreen {

    [SerializeField]
    private KeyCode endKey;
    [SerializeField]
    private Animator endAnimator;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        base.Update();

        if (inputEnabled)
        {
            if (Input.GetKeyDown(endKey))
            {
                StartCoroutine("EndGame");
            }
        }
	}

    IEnumerator EndGame()
    {
        endAnimator.SetTrigger("Play");
        while (!endAnimator.GetCurrentAnimatorStateInfo(0).IsName("Finished"))
        {
            yield return new WaitForFixedUpdate();
        }
        SceneManager.LoadScene("EndCredits");
    }
}
