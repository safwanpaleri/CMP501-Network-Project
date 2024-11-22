using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Firebase;
using Firebase.Database;
using Firebase.Extensions;

public class FirebaseManager : MonoBehaviour
{
    private DatabaseReference dbReference;
    [HideInInspector] public string ipaddress;

    // Start is called before the first frame update
    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbReference = FirebaseDatabase.DefaultInstance.RootReference;
                Debug.Log("Firebase initialized successfully!");
            }
            else
            {
                Debug.LogError($"Could not resolve all Firebase dependencies: {task.Result}");
            }
        });
    }

    public void SendData(string ip, string roomcode)
    {
        StartCoroutine(SendData_Coroutine(ip, roomcode));
    }

    private IEnumerator SendData_Coroutine(string ip, string roomcode)
    {
        var task = dbReference.Child("DNS").Child(roomcode).SetValueAsync(ip);
        yield return new WaitUntil(() => task.IsCompleted);

        if (task.IsCompletedSuccessfully)
            Debug.Log("Added ip to DNS");
        else
            Debug.LogError("Adding to database failed");

    }

    public void GetData(string roomcode)
    {
        StartCoroutine(GetData_Coroutine(roomcode));
    }

    private IEnumerator GetData_Coroutine(string roomcode)
    {
        var task = dbReference.Child("DNS").Child(roomcode).GetValueAsync();

        yield return new WaitUntil(() => task.IsCompleted);
        if (task.IsCompletedSuccessfully)
        {
            Debug.Log("Fetched ip using roomcode: " + task.Result.Value);
            ipaddress = task.Result.Value.ToString();
        }
        else
            Debug.LogError("fetching dns failed: " + task.Result.ToString());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
