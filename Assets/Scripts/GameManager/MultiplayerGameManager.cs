using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerGameManager : MonoBehaviour
{
    [SerializeField] private GameObject MobileControls;
    [SerializeField] private Text timerText;

    private int seconds = 0;
    private int minutes = 0;

    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerMultiplayerController multiplayerController;

    private void Awake()
    {
#if !UNITY_ANDROID || !UNITY_IOS

        MobileControls.SetActive(false);
#endif
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Timer());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerController.health < 0 || multiplayerController.health < 0)
        {
            Debug.Log("GameOver");
        }

    }

    private IEnumerator Timer()
    {
        yield return new WaitForSeconds(1f);
        seconds++;
        if (seconds > 59)
        {
            minutes++;
            seconds= 0;
        }

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        StartCoroutine(Timer());
    }
}
