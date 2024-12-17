using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController2 : MonoBehaviour
{
    [HideInInspector] public bool isMoveForward = false, isMoveBackward = false, isKicking = false, isPunching = false, isDefending = false;

    private Animator animator;

    [SerializeField] private Text playerName;
    [SerializeField] private Slider healthSlider;
    [HideInInspector] public int health = 100;

    [HideInInspector] public bool isAttacking;
    [SerializeField] private PlayerMultiplayerController playerMultiplayerController;

    private NetworkClient client;
    private NetworkServer server;
    bool isServer = false;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        client = FindObjectOfType<NetworkClient>();
        server = FindObjectOfType<NetworkServer>();

        if(server != null )
        {
            isServer = server.isServer;
        }
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
        {
            isKicking = true;
            isAttacking = true;
        }
        if (Input.GetKeyUp(KeyCode.J))
        {
            isKicking = false;
            isAttacking = false;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            isDefending = true;
        }
        if (Input.GetKeyUp(KeyCode.K))
        {
            isDefending = false;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            isPunching = true;
            isAttacking = true;
        }
        if (Input.GetKeyUp(KeyCode.L))
        {
            isPunching = false;
            isAttacking = false;
        }
    }

    void HandleAnimation()
    {
        if (isMoveForward)
        {
            MoveForward();
            if (isServer)
                server.SendUDPMessageToClient("MoveForward");
            else
                client.SendUDPMessageToServer("MoveForward");
        }

        if (isMoveBackward)
        {
            MoveBackward();
            if (isServer)
                server.SendUDPMessageToClient("MoveBackward");
            else
                client.SendUDPMessageToServer("MoveBackward");
        }

        if (isKicking)
        {
            Kick();
            if (isServer)
                server.SendUDPMessageToClient("Kick");
            else
                client.SendUDPMessageToServer("Kick");
        }

        if (isPunching)
        {
            Punch();
            if (isServer)
                server.SendUDPMessageToClient("Punch");
            else
                client.SendUDPMessageToServer("Punch");
        }

        if (isDefending)
        {
            Defend();
            if (isServer)
                server.SendUDPMessageToClient("Defend");
            else
                client.SendUDPMessageToServer("Defend");
        }
    }

    public void MoveForward()
    {
        animator.SetTrigger("MoveForward");
    }

    public void MoveBackward()
    {
        animator.SetTrigger("MoveBackward");
    }

    public void Kick()
    {
        animator.SetTrigger("Kick");
    }

    public void Punch()
    {
        animator.SetTrigger("Punch");
    }

    public void Defend()
    {
        animator.SetTrigger("Defend");
    }

    public void Win()
    {
        animator.SetTrigger("Win");
    }

    public void Lose()
    {
        animator.SetTrigger("Lose");
    }


    public void CollisionDetected(GameObject collision)
    {
        if (collision.gameObject.tag == "Opponent")
        {
            if (!isDefending && !isAttacking && playerMultiplayerController.isAttacking)
            {
                if (isServer)
                    server.SendUDPMessageToClient("Hit");
                else
                    client.SendUDPMessageToServer("Hit");

                health -= 5;
                healthSlider.value = health;
                if (health <= 0)
                {
                    if (playerMultiplayerController != null && playerMultiplayerController.isActiveAndEnabled)
                        playerMultiplayerController.Win();

                    Lose();
                }
                //Debug.LogWarning("Opponent Attacked");
            }
            //Debug.LogWarning("Opponent attacking: " + playerMultiplayerController.isAttacking);
            //Debug.LogWarning("collided 4");
        }
        //Debug.LogWarning("collided 3");
    }

    public void GetPredictionMovement()
    {

    }
}
