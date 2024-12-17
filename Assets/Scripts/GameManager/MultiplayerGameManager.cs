using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MultiplayerGameManager : MonoBehaviour
{
    [SerializeField] private GameObject MobileControls;
    [SerializeField] private Text timerText;

    private int seconds = 0;
    private int minutes = 0;

    [SerializeField] private PlayerController2 playerController;
    [SerializeField] private PlayerMultiplayerController multiplayerController;
    [SerializeField] private PlayerAIController playerAiController;
    [HideInInspector] private NetworkClient client;
    [HideInInspector] private NetworkServer server;

    [SerializeField] private GameObject winCanvas;
    [SerializeField] private GameObject loseCanvas;
    [SerializeField] private GameObject drawCanvas;

    public bool isGameEnded = false;

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
        server = FindAnyObjectByType<NetworkServer>();
        client = FindAnyObjectByType<NetworkClient>();

        if (server != null)
        {
            server.playerController = playerController;
            server.playerAiController = playerAiController;
            server.multiplayerGameManager = this;
            //server.StartTDPVerifications();
            //server.ReceiveTDPMessage();
        }

        if (client != null)
        {
            client.playerController = playerController;
            client.playerAIController = playerAiController;
            client.multiplayerGameManager = this;
            //client.StartTDPVerifications();
            //client.ReceiveTDPMessage();
        }

    }

    public void GameEnded(string result)
    {
        if(result == "Win")
        {
            winCanvas.SetActive(true);
        }
        else if(result == "Lose")
        {
            loseCanvas.SetActive(true);
        }
        else
        {
            drawCanvas.SetActive(true);
        }
        isGameEnded = true;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(multiplayerController.GetHealth() < 0)
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
        if(!isGameEnded)
            StartCoroutine(Timer());
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
