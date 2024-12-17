using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SinglePlayerGameManager : MonoBehaviour
{
    [SerializeField] private Text timer;
    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private GameObject drawCanvas;
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
            timer.text = string.Format("{0:00}:{1:00}", minute, seconds);
        yield return new WaitForSeconds(1f);
        if(!stopTimer)
            StartCoroutine(TimerCoroutine());
    }

    public void GameEnded(string result)
    {
        if (result == "Win")
            winCanvas.SetActive(true);
        else if(result == "Lose")
            loseCanvas.SetActive(true);
        else
            drawCanvas.SetActive(true);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
