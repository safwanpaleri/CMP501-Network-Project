using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMultiplayerController : MonoBehaviour
{
    private NetworkClient networkClient;
    private NetworkServer networkServer;
    private bool isServer = false;
    private Animator animator;


    [SerializeField] private Text playerName;
    [SerializeField] private Slider healthSlider;
    [HideInInspector] private int health = 100;

    [SerializeField] private PlayerController2 playerController;
    [HideInInspector] public bool isDefending, isAttacking = false;

    private void Awake()
    {
        networkClient = FindObjectOfType<NetworkClient>();
        networkServer = FindObjectOfType<NetworkServer>();
        animator = GetComponent<Animator>();
        if (networkServer != null)
            isServer = networkServer.isServer;
        if(networkClient != null )
            networkClient.playerMultiplayerController = this;
        if(networkServer != null )
            networkServer.playerMultiplayerController = this;
    }
    public void HandleAnimation(string input)
    {
        animator.SetTrigger(input);

        if(input == "Punch" || input == "Kick")
        {
            StartCoroutine(SetAttacking());
        }
    }

    private IEnumerator SetAttacking()
    {
        isAttacking = true;
        yield return new WaitForSeconds(0.5f);
        isAttacking = false;
    }

    public void CollisionDetected(GameObject collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            if (!isDefending && !isAttacking && playerController.isAttacking)
            {
                TakeDamage();
                if (isServer)
                    networkServer.SendUDPMessageToClient("Hit");
                else
                    networkClient.SendUDPMessageToServer("Hit");

                //Debug.LogWarning("Player Attacked");
            }
           // Debug.LogWarning("Player Attacked: " + playerController.isAttacking);
        }
        //Debug.Log("collision: " + collision.name);
    }

    public void Win()
    {
        animator.SetTrigger("Win");
    }

    public void Lose()
    {
        animator.SetTrigger("Lose");
    }

    public void TakeDamage()
    {
        health -= 5;
        healthSlider.value = health;
        if(health < 0)
        {
            Lose();
            playerController.Win();
        }
        Debug.Log("Take Damage");
    }

    public int GetHealth()
    {
        return health;
    }

    public void VerficationAndActions(Vector3 position, int health2)
    {
        var playerPosition = this.gameObject.transform.position;
        //if (playerPosition.x != position.x)
        //{
        //    if (playerPosition.x > position.x)
        //    {
        //        HandleAnimation("MoveBackward");
        //    }

        //    if (playerPosition.x < position.x)
        //    {
        //        HandleAnimation("MoveForward");
        //    }
        //}

        if (health2 != health)
        {
            health = health2;
            healthSlider.value = health;
        }
    }
}
