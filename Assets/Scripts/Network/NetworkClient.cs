using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using UnityEngine.SceneManagement;


public class NetworkClient : MonoBehaviour
{
    [HideInInspector] public NetworkClient instance;
    private TcpClient tcpClient;
    private NetworkStream stream;

    [HideInInspector] public int port = 7777;
    [HideInInspector] public int port2 = 5001;
    [HideInInspector] public int port3 = 5002;

    private bool isConnected = false;

    [Header("Helper")]
    [SerializeField] private FirebaseManager firebaseManager;
    [HideInInspector] public PlayerMultiplayerController playerMultiplayerController;

    [Header("UI")]
    [SerializeField] private GameObject clientUI;
    [SerializeField] private GameObject JoinUI;
    [SerializeField] private InputField ServerIp;
    [SerializeField] private InputField messageField;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    public async void StartClient()
    {
        try
        {
            firebaseManager.GetData(ServerIp.text);
            await Task.Delay(3000);

            tcpClient = new TcpClient();
            Debug.Log("Connecting to server at " + firebaseManager.ipaddress + " : " + port);
            await tcpClient.ConnectAsync(firebaseManager.ipaddress, port);
            Debug.Log("Connected to server at " + firebaseManager.ipaddress);
            //stream = tcpClient.GetStream();
            isConnected = true;
            RecieveUDPMessage();
            SceneManager.LoadScene(1);
        }
        catch (Exception e)
        {
            Debug.LogError("Error connecting to server: " + e.Message);
        }
    }

    private void StopClient()
    {
        tcpClient.Close();
        stream.Close();
    }

    public async void ReceiveMessage()
    {
        try
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
                        Debug.Log("Message received: " + receivedMessage);
                    }
                    else
                    {
                        Debug.LogWarning("No data received, the server might have closed the connection.");
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
        catch (Exception e)
        {
            Debug.LogError("Error receiving message: " + e.Message);
        }
    }

    public async void RecieveUDPMessage()
    {
        using (UdpClient udpClient = new UdpClient(port2))
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
                    if (playerMultiplayerController != null)
                    {
                        playerMultiplayerController.HandleAnimation(receivedMessage);
                        Debug.Log("received Message: " + receivedMessage);

                        if(receivedMessage == "Hit")
                        {
                            playerMultiplayerController.TakeDamage();
                        }
                    }
                    else
                        Debug.LogError("player multiplayer controller is null");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Error receiving UDP message: " + e.Message);
            }
        }
    }

    public void SendUDPMessageToServer(string message)
    {
        try
        {
            UdpClient udpClient = new UdpClient();
            byte[] data = Encoding.UTF8.GetBytes(message);

            //var clientip = tcpl;
            var clientip = tcpClient.Client.RemoteEndPoint.ToString().Split(":");
            //Debug.Log("ip: " + tcpClient.Client.RemoteEndPoint.ToString() + " & port: " + port3);
            udpClient.Send(data, data.Length, clientip[0], port3);
            Debug.Log("Message Sent");

            udpClient.Close();
        }
        catch (Exception e)
        {
            Debug.LogError("Error sending message: " + e.Message);
        }
    }

    private void FixedUpdate()
    {
        //if (isConnected)
        //    ReceiveMessage();
    }

    private void OnApplicationQuit()
    {
        //StopClient();
    }
}
