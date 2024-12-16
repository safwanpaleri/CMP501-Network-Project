using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SinglePlayerGameManager : MonoBehaviour
{
    [SerializeField] private Text timer;
    int seconds = 5;
    int minute = 0;
    bool stopTimer = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }

    private IEnumerator TimerCoroutine()
    {
        seconds--;
        if(seconds < 0)
        {
            seconds = 59;
            minute--;
        }
        if (minute < 0)
            stopTimer = true;
        if(minute >= 0)
            timer.text = minute.ToString() +" : " + seconds.ToString();
        yield return new WaitForSeconds(1f);
        if(!stopTimer)
            StartCoroutine(TimerCoroutine());
    }
}
