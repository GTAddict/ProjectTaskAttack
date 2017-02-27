using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TypingScreen : BaseScreen {

    public string WorkFocussedWords;
    public string FamilyFocussedWords;
    public char wordSeparator;

    public float moveDownDuration = 8.0f; 
    public float finalizeDuration = 3.0f;

    public Text startRange;
    public Text endRange;
    public Canvas canvas;

    private float startX, endX;
    private float startY, endY;

    private string[] parsedWorkWords;
    private string[] parsedFamilyWords;

    private ArrayList activeWords;
    private string currentWordBeingBuilt;
    private Text objectBeingCompared;
    private string currentObjectString;

    [SerializeField]
    private SFXManager sfxManager;
    [SerializeField]
    private AudioClip error;
    [SerializeField]
    private AudioClip type;

	// Use this for initialization
	void Awake ()
    {
        base.Awake();

        parsedWorkWords = WorkFocussedWords.ToUpper().Split(wordSeparator);
        parsedFamilyWords = FamilyFocussedWords.ToUpper().Split(wordSeparator);

        startX  = startRange.transform.position.x;
        startY  = startRange.transform.position.y;
        endX    = endRange.transform.position.x;
        endY    = endRange.transform.position.y;

        startRange.enabled = false;
        endRange.enabled = false;

        activeWords = new ArrayList();
    }

    void SpawnRandomWord(string[] wordArray)
    {
        float spawnX = Random.Range(startX, endX);
        float spawnY = startY;

        Text spawnedText = (Text) Instantiate(startRange, canvas.transform, false);
        spawnedText.text = wordArray[Random.Range(0, wordArray.Length)];
        spawnedText.transform.position = new Vector3(spawnX, spawnY, 0);
        spawnedText.enabled = true;

        activeWords.Add(spawnedText);

        StartCoroutine(MoveDownwards(spawnedText, moveDownDuration));
    }

    IEnumerator MoveDownwards(Text text, float duration)
    {
        if (text)
        {
            Vector3 startPosition = text.transform.position;
            Vector3 endPosition = new Vector3(startPosition.x, endY, startPosition.z);
            float time = 0;

            while (text && activeWords.Contains(text) && text.transform.position.y > endPosition.y)
            {
                text.transform.position = Vector3.Lerp(startPosition, endPosition, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            StartCoroutine(Finalize(text, finalizeDuration));
        }
    }

    IEnumerator Finalize(Text text, float duration)
    {
        if (text)
        {
            Vector3 startScale = text.transform.localScale;
            Vector3 endScale = startScale * 1.2f;
            float time = 0;

            while (text && activeWords.Contains(text) && text.transform.localScale.y < endScale.y)
            {
                text.transform.localScale = Vector3.Lerp(startScale, endScale, time / duration);
                time += Time.deltaTime;
                yield return null;
            }

            if (activeWords.Contains(text))
            {
                OnWordFail(text, false);
            }
        }
    }

    void OnWordSuccess(Text text)
    {
        taskManager.IncrementTask(TaskType.Typing);
        text.color = Color.green;
        WordFinalize(text);
    }

    void OnWordFail(Text text, bool canContinue)
    {
        text.color = Color.red;

        if (!canContinue)
        {
            WordFinalize(text);
        }
    }

    void WordFinalize(Text text)
    {
        currentWordBeingBuilt = null;

        activeWords.Remove(text);
        if (text == objectBeingCompared)
        {
            objectBeingCompared = null;
        }
        StartCoroutine(DestroyAfter(text, 0.5f));
        SpawnRandomWord(parsedFamilyWords);
    }

    IEnumerator DestroyAfter(Text t, float delay)
    {
        yield return new WaitForSeconds(delay);

        Destroy(t);
    }
	
	// Update is called once per frame
	void Update ()
    {
        base.Update();

        if (Input.anyKeyDown)
        {
            for (KeyCode key = KeyCode.A; key < KeyCode.Z; ++key)
            {
                if (Input.GetKeyDown(key))
                {
                    sfxManager.GetComponent<SFXManager>().PlaySound(type);
                    currentWordBeingBuilt += key.ToString();
                }
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                sfxManager.GetComponent<SFXManager>().PlaySound(type);
                currentWordBeingBuilt += " ";
            }
        }

        if (currentWordBeingBuilt != null && currentWordBeingBuilt.Length > 0)
        {
            if (objectBeingCompared == null)
            {
                bool found = false;
                foreach (Text text in activeWords)
                {
                    if (text.text.Substring(0, currentWordBeingBuilt.Length).Equals(currentWordBeingBuilt))
                    {
                        found = true;
                        objectBeingCompared = text;
                        currentObjectString = objectBeingCompared.text;
                        break;
                    }
                }
                if (found == false)
                {
                    currentWordBeingBuilt = "";
                }
            }
            else if (currentObjectString.Substring(0, currentWordBeingBuilt.Length).Equals(currentWordBeingBuilt))
            {
               if (UpdateCurrentWord())
                {
                    OnWordSuccess(objectBeingCompared);
                }
            }
            else
            {
                currentWordBeingBuilt = currentWordBeingBuilt.Remove(currentWordBeingBuilt.Length - 1);
                OnWordFail(objectBeingCompared, true);
                sfxManager.GetComponent<SFXManager>().PlaySound(error);
            }
        }
    }

    bool UpdateCurrentWord()
    {
        int totalLen = currentObjectString.Length;
        if (currentWordBeingBuilt.Length < totalLen)
        {
            objectBeingCompared.text
                = "<color=#FFA500>"
                + currentWordBeingBuilt
                + "</color>"
                + currentObjectString.Substring(currentWordBeingBuilt.Length, totalLen - currentWordBeingBuilt.Length);

            return false;
        }
        else if (currentWordBeingBuilt.Length == totalLen)
        {
            objectBeingCompared.text = currentObjectString;
            return true;
        }

        return false;
    }


    override protected void OnFadeInComplete()
    {
        base.OnFadeInComplete();
        SpawnRandomWord(parsedFamilyWords);
    }

    public override void Close(bool withAnimation = true)
    {
        base.Close(withAnimation);

        if (activeWords != null)
        {
            foreach (Text text in activeWords)
            {
                Destroy(text);
            }

            activeWords.Clear();
        }

        currentWordBeingBuilt = null;
        objectBeingCompared = null;
        currentObjectString = null;

    }
}
