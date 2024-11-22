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
    [HideInInspector] public int health;

    private void Awake()
    {
        networkClient = FindObjectOfType<NetworkClient>();
        networkServer = FindObjectOfType<NetworkServer>();
        animator = GetComponent<Animator>();

        isServer = networkServer.isServer;
        networkClient.playerMultiplayerController = this;
        networkServer.playerMultiplayerController = this;
    }
    public void HandleAnimation(string input)
    {
        animator.SetTrigger(input);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            TakeDamage();
            if (isServer)
                networkServer.SendUDPMessageToClient("Hit");
            else
                networkClient.SendUDPMessageToServer("Hit");
        }
    }

    public void TakeDamage()
    {
        health -= 50;
        healthSlider.value = health;
        if(health < 0)
        {
            if (isServer)
                networkServer.SendUDPMessageToClient("You Lose");
            else
                networkClient.SendUDPMessageToServer("You Lose");
        }
    }
}
