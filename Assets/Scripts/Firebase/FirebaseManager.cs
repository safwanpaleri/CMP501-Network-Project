using System.Collections;
using UnityEngine;
using Firebase.Database;
using UnityEngine.Networking;


//Script that deals with code related to Firebase Database
//Firebase database is used as a DNS holder, where the ip address and DNS will be saved
//The player can fetch the ip using the roomcode and can be used to connect to networking.
public class FirebaseManager : MonoBehaviour
{
    //Cache variables
    private DatabaseReference dbReference;
    [HideInInspector] public string ipaddress;
    string firebaseUrl = "https://network-dns-5e20d-default-rtdb.firebaseio.com/";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //Function for sending data to firebase database
    //used to send the room code and ip-address.
    public void SendData(string ip, string roomcode)
    {
        StartCoroutine(SendData_Coroutine(ip, roomcode));
    }

    //Coroutine function of sending data to firebase.
    private IEnumerator SendData_Coroutine(string ip, string roomcode)
    {
        
        string requestUrl = firebaseUrl + "DNS/" + roomcode + ".json";  // Construct the path to your data (e.g., DNS/roomcode)

        // Create a JSON string to send
        string jsonData = "{\"ip\": \"" + ip + "\"}";

        // Use UnityWebRequest to send data to Firebase
        UnityWebRequest request = new UnityWebRequest(requestUrl, UnityWebRequest.kHttpVerbPUT);  // PUT for overwriting data
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);  // Convert string to byte array
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);  // Attach data to request
        request.downloadHandler = new DownloadHandlerBuffer();  // Set download handler (not used here, but required)

        // Send request and wait for response
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            Debug.Log("Added IP to DNS");
        }
        else
        {
            Debug.LogError("Adding to database failed: " + request.error);
        }
    }

    //Functiob for fetching data from firebase database
    //we will send a room code and its ip address will be returned 
    //if it is present in the database.
    public void GetData(string roomcode)
    {
        StartCoroutine(GetData_Coroutine(roomcode));
    }

    //Coroutine function of fetching data from firebase
    private IEnumerator GetData_Coroutine(string roomcode)
    {
        string requestUrl = firebaseUrl + "DNS/" + roomcode + ".json";  // Construct the path to your data

        // Send GET request
        UnityWebRequest request = UnityWebRequest.Get(requestUrl);

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            // Parse and handle the response
            var str = request.downloadHandler.text.Split("\"");
            Debug.Log("Fetched IP using roomcode: " + str[3]);
            ipaddress = str[3];
        }
        else
        {
            Debug.LogError("Fetching DNS failed: " + request.error);
        }
    }

}
