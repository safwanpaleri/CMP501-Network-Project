using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public bool isMoveForward = false, isMoveBackward = false, isKicking = false, isPunching = false, isDefending = false;

    private Animator animator;
   
    [SerializeField] private Text playerName;
    [SerializeField] private Slider healthSlider;
    [HideInInspector] private int health = 100;

    [HideInInspector] public bool isAttacking;
    [SerializeField] private PlayerAIController opponentController;
    [SerializeField] private PlayerAIController2 opponentController2;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
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
            if (!isDefending && !isAttacking && (opponentController.isAttacking || opponentController2.isAttacking))
            {
                health -= 10;
                healthSlider.value = health;
                if(health <= 0)
                {
                    if(opponentController != null && opponentController.isActiveAndEnabled)
                        opponentController.Win();
                    if(opponentController2 != null && opponentController2.isActiveAndEnabled)
                        opponentController2.Win();
                    Lose();
                }

            }

        }
        //Debug.Log(collision.gameObject.name + " : " + health);
    }
}
