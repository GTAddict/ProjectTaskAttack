using UnityEngine;
using System.Collections;
using System;

public class FileSortScreen : BaseScreen
{
    [System.Serializable]
    public class FolderKeyMap
    {
        public GameObject folder;
        public GameObject file;
        public GameObject wrongFile;
        public KeyCode key;
    }

    public FolderKeyMap[] folderKeyMap;
    public GameObject spawnPoint;
    private int numFolders;
    private int currentIndex;
    private GameObject currentGameObject;
    private GameObject currentInvalidFileGameObject;
    private GameObject mainFoldersObject;

    public float folderAnimDuration = 0.5f;
    public float wrongAnimDisplayDuration = 0.25f;

    [SerializeField]
    private SFXManager sfxManager;
    [SerializeField]
    private AudioClip sortFile;
    [SerializeField]
    private AudioClip wrongFile;

    void Awake()
    {
        base.Awake();
        numFolders = folderKeyMap.Length;
    }

    void Update()
    {
        base.Update();

        if (inputEnabled)
        {
            if (Input.anyKeyDown)
            {
                DestroyInvalidGO();

                if (Input.GetKeyDown(folderKeyMap[currentIndex].key))
                {
                    sfxManager.GetComponent<SFXManager>().PlaySound(sortFile);
                    FolderAnimation(currentGameObject);
                    taskManager.IncrementTask(TaskType.FileSort);
                    GenerateRandom();
                }
                else
                {
                    sfxManager.GetComponent<SFXManager>().PlaySound(wrongFile);
                    InvalidAnimation();
                }
            }
        }
    }

    public override void Open()
    {
        base.Open();
        if (currentGameObject == null)
        {
            GenerateRandom();
        }
    }

    public override void Close(bool withAnimation = true)
    {
        if (currentInvalidFileGameObject != null)
        {
            DestroyInvalidGO();
        }
        
        base.Close(withAnimation);
    }

    private void GenerateRandom()
    {
        currentIndex = UnityEngine.Random.Range(0, numFolders);
        currentGameObject = (GameObject) Instantiate(folderKeyMap[currentIndex].file, spawnPoint.transform);
        currentGameObject.transform.position = spawnPoint.transform.position;

    }

    private void FolderAnimation(GameObject g)
    {
        StartCoroutine(
            MoveToPosition(
                g,
                folderKeyMap[currentIndex].folder.transform.position,
                folderAnimDuration,
                OnFolderAnimationComplete
                ));
    }

    private void OnFolderAnimationComplete(GameObject g)
    {
        Destroy(g);
    }

    private void InvalidAnimation()
    {
        currentInvalidFileGameObject = Instantiate(folderKeyMap[currentIndex].wrongFile);
        currentInvalidFileGameObject.transform.position = spawnPoint.transform.position;

        Invoke("DestroyInvalidGO", wrongAnimDisplayDuration);
    }

    void DestroyInvalidGO()
    {
        Destroy(currentInvalidFileGameObject);
        currentInvalidFileGameObject = null;
    }

    IEnumerator MoveToPosition(GameObject g, Vector3 newPosition, float duration, Action<GameObject> callback)
    {
        Vector3 currentPosition = g.transform.position;

        for (float time = 0; time < duration; time += Time.deltaTime)
        {
            g.transform.position = Vector3.Lerp(currentPosition, newPosition, time / duration);
            yield return null;
        }

        if (callback != null)
        {
            callback(g);
        }
    }
}