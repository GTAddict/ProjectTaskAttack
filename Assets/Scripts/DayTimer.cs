using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DayTimer : MonoBehaviour {

    const int TotalDayTime = 840;
    const int OnTime = 520;
    const int LateLimit = 13;

    [SerializeField]
    private float totalTimeForDay;
    private float rate;
    private int hours = 9;
    private int actualMinutes = 0;
    private Text text;
    private int lateDays = 0;

    [SerializeField]
    private KeyCode goHomeKey = KeyCode.DownArrow;
    private TaskManager taskManager;

    private PropManager props;
    [SerializeField]
    private Animator fadeAnimator;

    [SerializeField]
    private AudioSource ambientAudio;
    [SerializeField]
    private SFXManager sfxManager;
    [SerializeField]
    private float audioFadeDuration;


    private string TimeDisplay {
        get
        {
            string hourString = ((hours%12) == 0) ? "12" : (hours % 12).ToString();
            string minuteString = ((actualMinutes%60) < 10) ? string.Format("0{0}", actualMinutes%60) : (actualMinutes%60).ToString(); 
            string AMorPM = (hours>=12) ? "PM" : "AM";
            return string.Format("{0} : {1} {2}", hourString, minuteString, AMorPM);
        }
    }
    

    // Use this for initialization
    void Start ()
    {
        text = GetComponent<Text>();
        text.text = TimeDisplay;
        taskManager = GameObject.Find("TaskManager").GetComponent<TaskManager>();
        props = GameObject.Find("Screens").GetComponent<PropManager>();
        rate = totalTimeForDay / TotalDayTime;
        StartCoroutine("IncrementClock");
	}

    void Update()
    {
        if (Input.GetKeyDown(goHomeKey))
        {
            StartCoroutine("ResetDay");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            actualMinutes = 0;
            StartCoroutine("ResetDay");
        }
        else if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            actualMinutes = 700;
            StartCoroutine("ResetDay");
        }
    }

    IEnumerator IncrementClock()
    {
        while (actualMinutes <= TotalDayTime)
        {
            text.text = TimeDisplay;
            actualMinutes++;
            if (actualMinutes % 60 == 0)
            {
                hours++;
            }
            if (actualMinutes > OnTime)
            {
                sfxManager.FadeOut(ambientAudio, audioFadeDuration);
            }
            yield return new WaitForSeconds(rate);
        }
        StartCoroutine("ResetDay");
    }

    IEnumerator ResetDay()
    {
        sfxManager.FadeOut(ambientAudio, audioFadeDuration);
        StopCoroutine("IncrementClock");
        fadeAnimator.SetTrigger("Fade");
        int incrementStayedOver = (actualMinutes > OnTime) ? 1 : 0;
        if(actualMinutes >= TotalDayTime)
        {
            incrementStayedOver++;
        }
        lateDays += incrementStayedOver;
        hours = 9;
        actualMinutes = 0;
        text.text = TimeDisplay;
        while (!fadeAnimator.GetCurrentAnimatorStateInfo(0).IsName("FadeIn"))
        {
            yield return new WaitForEndOfFrame();
        }
        taskManager.NewDayInitialize();
        props.Reset(incrementStayedOver);
        if (lateDays <= LateLimit)
        {
            while (!fadeAnimator.GetCurrentAnimatorStateInfo(0).IsName("Invisible"))
            {
                yield return new WaitForEndOfFrame();
            }
            StartCoroutine("IncrementClock");
            sfxManager.FadeIn(ambientAudio, audioFadeDuration);
        }
        else
        {
            SceneManager.LoadScene("OfficeScene");
        }
    }
}
