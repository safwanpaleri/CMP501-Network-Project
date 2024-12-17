using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using UnityEngine.SceneManagement;

public class NetworkServer : MonoBehaviour
{
    [HideInInspector] public NetworkServer instance;
    [HideInInspector] public TcpListener tcpListener;
    [HideInInspector] public TcpClient tcpClient;
    private NetworkStream stream;
    [HideInInspector] public bool isServer = false;
    [HideInInspector] public PlayerMultiplayerController playerMultiplayerController;
    [HideInInspector] public PlayerController2 playerController;
    [HideInInspector] public PlayerAIController playerAiController;
    [HideInInspector] public MultiplayerGameManager multiplayerGameManager;


    [HideInInspector] public int port = 7777;
    [HideInInspector] public int port2 = 5001;
    [HideInInspector] public int port3 = 5002;

    [SerializeField] private InputField messageField;
    [SerializeField] private GameObject createServerUI;
    [SerializeField] private GameObject sendMessageUI;
    [SerializeField] private Text codeText;

    [SerializeField] private FirebaseManager firebaseManager;

    [System.Serializable]
    public class MessageData
    {
        public Vector3 playerPosition;
        public int heatlth;
    }

    float TDPMessageTimer = 0;
    float UDPMessageTimer = 0;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public async void StartServer()
    {
        try
        {
            string localIP = "";
            foreach (var address in Dns.GetHostAddresses(Dns.GetHostName()))
            {
                if (address.AddressFamily == AddressFamily.InterNetwork)
                {
                    localIP = address.ToString();
                    break;
                }
            }
            var roomcode = GenerateRoomCode();
            firebaseManager.SendData(localIP, roomcode);
            tcpListener = new TcpListener(IPAddress.Parse(localIP), port);
            tcpListener.Start();
            codeText.text = "Joining Code: \n" + roomcode;
            tcpClient = await tcpListener.AcceptTcpClientAsync();
            
            createServerUI.SetActive(false);
            sendMessageUI.SetActive(true);
            RecieveUDPMessage();
            StartCoroutine(SendTDPMessages());
            StartCoroutine(MessageTimer());
            ReceiveTDPMessage();
            isServer = true;
            SceneManager.LoadScene(1);
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }
    }

    public void SendTDPMessageToClient(string message)
    {
        try
        {
            if (tcpClient != null && tcpClient.Connected)
            {
                NetworkStream stream = tcpClient.GetStream();
                if (stream != null && stream.CanWrite)
                {
                    byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
                    stream.Write(messageBytes, 0, messageBytes.Length);
                    Debug.Log("Message sent");
                }
                else
                {
                    Debug.LogError("Cannot write to the stream.");
                }
            }
            else
            {
                Debug.LogError("No connected client.");
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending message: " + e.Message);
        }
    }

    public void SendUDPMessageToClient(string message)
    {
        try
        {
            UdpClient udpClient = new UdpClient();
            byte[] data = Encoding.UTF8.GetBytes(message);

            var clientip = tcpClient.Client.RemoteEndPoint.ToString().Split(":");

            Debug.Log("ip: " + clientip[0] + " & port: " + port2);
            udpClient.Send(data, data.Length, clientip[0], port2);
            Debug.Log("Message Sent");

            udpClient.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending message: " + e.Message);
        }
    }

    public async void RecieveUDPMessage()
    {
        using (UdpClient udpClient = new UdpClient(port3))
        {

            try
            {
                while (true)
                {
                    // Asynchronously wait for a UDP message
                    UdpReceiveResult result = await udpClient.ReceiveAsync();

                    // Decode the received message
                    string receivedMessage = Encoding.UTF8.GetString(result.Buffer);
                    //Debug.Log($"Received message from {result.RemoteEndPoint.Address}:{result.RemoteEndPoint.Port} - {receivedMessage}");
                    if (receivedMessage.Contains("PlayerMovement"))
                    {
                        var messages = receivedMessage.Split(':');
                        if (messages[1] == "MoveForward")
                            playerController.MoveForward();
                        else if (messages[1] == "MoveBackward")
                            playerController.MoveBackward();
                        else if (messages[1] == "Punch")
                            playerController.Punch();
                        else if (messages[1] == "Kick")
                            playerController.Kick();
                        else
                            playerController.Defend();
                    }
                    else
                    {
                        if (playerMultiplayerController != null)
                        {

                            Debug.Log("received Message: " + receivedMessage);

                            if (receivedMessage == "Hit")
                            {
                                playerMultiplayerController.TakeDamage();
                            }
                            else
                            {
                                playerMultiplayerController.HandleAnimation(receivedMessage);
                            }
                        }
                        else
                            Debug.LogError("player multiplayer controller is null");
                    }

                    UDPMessageTimer = 0;
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving UDP message: " + e.Message);
            }
        }
    }

    public void StopServer()
    {
        stream.Close();
        tcpClient.Close();
        tcpListener.Stop();
    }

    private static string GenerateRoomCode()
    {
        var characters = "abcdefghijklmnopqrstuvwxyz012345789";
        char[] result = new char[6];
        for (int i = 0; i < 6; i++)
        {
            result[i] = characters[UnityEngine.Random.Range(0,characters.Length)];
        }
        return new string(result);
    }

    public void StartTDPVerifications()
    {
        StartCoroutine(SendTDPMessages());
    }

    public IEnumerator SendTDPMessages()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            if (playerController != null)
            {
                MessageData messageData = new MessageData();
                messageData.playerPosition = playerController.gameObject.transform.position;
                messageData.heatlth = playerController.health;

                var jsonData = JsonUtility.ToJson(messageData);

                SendTDPMessageToClient(jsonData);
            }
        }
    }

    public async void ReceiveTDPMessage()
    {
        try
        {
            while(true)
            {
                if (tcpClient != null && tcpClient.Connected)
                {
                    NetworkStream stream = tcpClient.GetStream();
                    if (stream != null && stream.CanRead)
                    {
                        byte[] buffer = new byte[1024]; // Adjust size as needed
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);

                        if (bytesRead > 0)
                        {
                            string receivedMessage = System.Text.Encoding.UTF8.GetString(buffer, 0, bytesRead);
                            MessageData messageData;
                            messageData = JsonUtility.FromJson<MessageData>(receivedMessage);
                            playerMultiplayerController.VerficationAndActions(messageData.playerPosition, messageData.heatlth);
                            //Debug.LogWarning("Server TDP Message: " + receivedMessage);
                            TDPMessageTimer = 0;
                        }
                        else
                        {
                            //Debug.LogWarning("No data received, the server might have closed the connection.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Cannot read from the stream.");
                    }
                }
                else
                {
                    Debug.LogError("No connected server.");
                }
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error receiving message: " + e.Message);
        }
    }

    private IEnumerator MessageTimer()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            TDPMessageTimer++;
            UDPMessageTimer++;

            if (TDPMessageTimer > 10)
            {
                Debug.LogWarning("Last Message recieved 10 seconds ago");
                playerMultiplayerController.Lose();
                playerController.Lose();
                multiplayerGameManager.GameEnded("Draw");
                TDPMessageTimer = 0;
            }

            if (UDPMessageTimer > 15)
            {
                Debug.LogWarning("Last Message recieved 10 seconds ago");
                var prediction = playerAiController.GetASingleReaction();
                Debug.LogWarning("Prediction: " + prediction);
                playerMultiplayerController.HandleAnimation(prediction);
                SendUDPMessageToClient("PlayerMovement:" + prediction);
                UDPMessageTimer = 0;
            }

            if (multiplayerGameManager != null && multiplayerGameManager.isGameEnded)
                break;
        }

    }

    private void OnApplicationQuit()
    {
        //StopServer();
    }


}
