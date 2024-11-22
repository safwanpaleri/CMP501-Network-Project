using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    bool isMoveForward = false, isMoveBackward = false, isKicking = false, isPunching = false, isDefending = false;

    private Animator animator;
    private NetworkServer networkserver;
    private NetworkClient networkClient;

    [HideInInspector] public bool isServer;
    [SerializeField] private Text playerName;
    [SerializeField] private Slider healthSlider;
    [HideInInspector] public int health;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        networkserver = FindObjectOfType<NetworkServer>();
        isServer = networkserver.isServer;
    }

    // Update is called once per frame
    void Update()
    {
        HandleInput();
        HandleAnimation();
    }

    void HandleInput()
    {
        if (Input.GetKey(KeyCode.W))
            isMoveForward = true;

        if (Input.GetKeyUp(KeyCode.W))
            isMoveForward = false;

        if (Input.GetKeyDown(KeyCode.S))
            isMoveBackward = true;

        if (Input.GetKeyUp(KeyCode.S))
            isMoveBackward = false;

        if (Input.GetKeyDown(KeyCode.J))
            isKicking = true;
        if (Input.GetKeyUp(KeyCode.J))
            isKicking = false;

        if (Input.GetKeyDown(KeyCode.K))
            isDefending = true;
        if (Input.GetKeyUp(KeyCode.K))
            isDefending = false;

        if (Input.GetKeyDown(KeyCode.L))
            isPunching = true;
        if (Input.GetKeyUp(KeyCode.L))
            isPunching = false;
    }

    void HandleAnimation()
    {
        if (isMoveForward)
        {
            MoveForward();
        }

        if (isMoveBackward)
        {
            MoveBackward();
        }

        if (isKicking)
        {
            Kick();
        }

        if (isPunching)
        {
            Punch();
        }

        if (isDefending)
        {
            Defend();
        }
    }

    public void MoveForward()
    {
        animator.SetTrigger("MoveForward");
        if (isServer)
            networkserver.SendUDPMessageToClient("MoveForward");
        else
            networkClient.SendUDPMessageToServer("MoveForward");
    }

    public void MoveBackward()
    {
        animator.SetTrigger("MoveBackward");
        if (isServer)
            networkserver.SendUDPMessageToClient("MoveBackward");
        else
            networkClient.SendUDPMessageToServer("MoveBackward");
    }

    public void Kick()
    {
        animator.SetTrigger("Kick");
        if (isServer)
            networkserver.SendUDPMessageToClient("Kick");
        else
            networkClient.SendUDPMessageToServer("kick");
    }

    public void Punch()
    {
        animator.SetTrigger("Punch");
        if (isServer)
            networkserver.SendUDPMessageToClient("Punch");
        else
            networkClient.SendUDPMessageToServer("Punch");
    }

    public void Defend()
    {
        animator.SetTrigger("Defend");
        if (isServer)
            networkserver.SendUDPMessageToClient("Defend");
        else
            networkClient.SendUDPMessageToServer("Defend");
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Opponent")
        {
            if(!isDefending)
            {
                health -= 50;
                healthSlider.value = health;
                if (isServer)
                    networkserver.SendUDPMessageToClient("Hit");
                else
                    networkClient.SendUDPMessageToServer("Hit");
            }
            
        }    
    }
}
