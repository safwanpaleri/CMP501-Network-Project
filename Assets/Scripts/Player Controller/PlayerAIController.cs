using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAIController : MonoBehaviour
{
    private Animator animator;
    private PlayerController2 playerController;
    [SerializeField] private Text playerName;
    [SerializeField] private Slider healthSlider;
    [HideInInspector] public bool isDefending = false;
    [HideInInspector] public bool isAttacking = false;

    public Transform player;
    public float health = 100f;
    public float punchRange = 1.5f;
    public float kickRange = 2.5f;
    public float lowHealthThreshold = 30f;
    public float moveSpeed = 2f;

    private string playerAction = "idle"; 

    // Difficulty levels
    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty difficulty = Difficulty.Medium;

    float distance;
    float distanceClose;
    float distanceMedium;
    float distanceFar;

    float healthLow;
    float healthHigh;

    // Fuzzy rules for actions
    float punchConfidence;
    float kickConfidence;
    float defendConfidence;
    float moveForwardConfidence;
    float moveBackwardConfidence;
    float rand;

    private float reactionTime = 0.2f;
    private float probabilty = 0.5f;


    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = player.GetComponent<PlayerController2>();
        //StartCoroutine(Reaction());
    }

    void Update()
    {
        
        
    }

    private IEnumerator Reaction()
    {
        distance = Vector3.Distance(transform.position, player.position);
        //Debug.LogWarning(distance);
        distanceClose = FuzzifyClose(distance);
        distanceMedium = FuzzifyMedium(distance);
        distanceFar = FuzzifyFar(distance);

        healthLow = FuzzifyLowHealth(health);
        healthHigh = FuzzifyHighHealth(health);

        // Fuzzy rules for actions
        punchConfidence = FuzzyAnd(distanceClose, 1 - healthLow);
        kickConfidence = FuzzyAnd(distanceMedium, 1 - healthLow);
        defendConfidence = FuzzyAnd(PlayerIsAttacking(), 1);
        moveForwardConfidence = FuzzyAnd(distanceFar, healthHigh);
        moveBackwardConfidence = FuzzyAnd(distanceClose, healthLow);

        // Adjust decisions based on difficulty
        //AdjustForDifficulty(ref punchConfidence, ref kickConfidence, ref defendConfidence, ref moveForwardConfidence, ref moveBackwardConfidence);
        
        if (playerController.isAttacking)
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if (rand > probabilty)
                Defend();
        }
        // Choose action based on the highest confidence
        if (punchConfidence > kickConfidence && punchConfidence > defendConfidence &&
            punchConfidence > moveForwardConfidence && punchConfidence > moveBackwardConfidence)
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if(rand > probabilty)
                Punch();
            
        }
        else if (kickConfidence > defendConfidence && kickConfidence > moveForwardConfidence &&
                 kickConfidence > moveBackwardConfidence)
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if (rand > probabilty)
                Kick();
        }
        else if (moveForwardConfidence > moveBackwardConfidence)
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if (rand > probabilty)
                MoveForward();
        }
        else
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if (rand > probabilty)
                MoveBackward();
        }

        yield return new WaitForSeconds(reactionTime);

        StartCoroutine(Reaction());
    }

    // Fuzzy membership functions
    float FuzzifyClose(float distance)
    {
        return Mathf.Clamp01(1 - distance / punchRange);
    }

    float FuzzifyMedium(float distance)
    {
        return Mathf.Clamp01((distance - punchRange) / (kickRange - punchRange));
    }

    float FuzzifyFar(float distance)
    {
        return Mathf.Clamp01((distance - kickRange) / kickRange);
    }

    float FuzzifyLowHealth(float currentHealth)
    {
        return Mathf.Clamp01(1 - currentHealth / lowHealthThreshold);
    }

    float FuzzifyHighHealth(float currentHealth)
    {
        return Mathf.Clamp01(currentHealth / lowHealthThreshold);
    }

    float PlayerIsAttacking()
    {
        return playerAction == "punch" || playerAction == "kick" ? 1.0f : 0.0f;
    }

    // Fuzzy AND operator
    float FuzzyAnd(float a, float b)
    {
        return Mathf.Min(a, b);
    }

    // Difficulty adjustment
    void AdjustForDifficulty(ref float punch, ref float kick, ref float defend, ref float moveForward, ref float moveBackward)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                reactionTime = 1.0f;
                punch *= 0.7f; // Less aggressive
                kick *= 0.7f;
                defend *= 0.5f; // Less likely to defend
                moveForward *= 1.2f; // More likely to move forward
                moveBackward *= 0.8f; // Less likely to retreat
                probabilty = 0.7f;
                break;

            case Difficulty.Medium:
                // No change for medium
                reactionTime = 0.5f;
                probabilty = 0.5f;
             
                break;

            case Difficulty.Hard:
                reactionTime = 0.2f;
                punch *= 1.2f; // More aggressive
                kick *= 1.2f;
                defend *= 1.5f; // More likely to defend
                moveForward *= 0.8f; // Less likely to move forward unnecessarily
                moveBackward *= 1.5f; // More likely to retreat when low health
                probabilty = 0.2f;
                break;
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
        isAttacking = true;
        animator.SetTrigger("Kick");
        StartCoroutine(StopAttacking());
    }

    public void Punch()
    {
        isAttacking = true;
        animator.SetTrigger("Punch");
        StartCoroutine(StopAttacking());
    }

    public void Defend()
    {
        isDefending = true;
        animator.SetTrigger("Defend");
        StartCoroutine(StopDefending());

    }

    private IEnumerator StopAttacking()
    {
        yield return new WaitForSeconds(0.25f);
        isAttacking = false;
    }

    private IEnumerator StopDefending()
    {
        yield return new WaitForSeconds(0.18f);
        isDefending = false;
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
        if (collision.gameObject.tag == "Player")
        {
            if (!isDefending && !isAttacking && playerController.isAttacking)
            {
                health -= 5;
                healthSlider.value = health;
                if (health <= 0)
                {
                    if (playerController != null)
                        playerController.Win();
                    Lose();
                }
                
            }
           
        }
    }

    public string GetASingleReaction()
    {
        distance = Vector3.Distance(transform.position, player.position);
        //Debug.LogWarning(distance);
        distanceClose = FuzzifyClose(distance);
        distanceMedium = FuzzifyMedium(distance);
        distanceFar = FuzzifyFar(distance);

        healthLow = FuzzifyLowHealth(health);
        healthHigh = FuzzifyHighHealth(health);

        // Fuzzy rules for actions
        punchConfidence = FuzzyAnd(distanceClose, 1 - healthLow);
        kickConfidence = FuzzyAnd(distanceMedium, 1 - healthLow);
        defendConfidence = FuzzyAnd(PlayerIsAttacking(), 1);
        moveForwardConfidence = FuzzyAnd(distanceFar, healthHigh);
        moveBackwardConfidence = FuzzyAnd(distanceClose, healthLow);

        // Adjust decisions based on difficulty
        //AdjustForDifficulty(ref punchConfidence, ref kickConfidence, ref defendConfidence, ref moveForwardConfidence, ref moveBackwardConfidence);

        if (playerController.isAttacking)
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if (rand > probabilty)
                return "Defend";
        }
        // Choose action based on the highest confidence
        else if (punchConfidence > kickConfidence && punchConfidence > defendConfidence &&
            punchConfidence > moveForwardConfidence && punchConfidence > moveBackwardConfidence)
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if (rand > probabilty)
                return "Punch";

        }
        else if (kickConfidence > defendConfidence && kickConfidence > moveForwardConfidence &&
                 kickConfidence > moveBackwardConfidence)
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if (rand > probabilty)
                return "Kick";
        }
        else if (moveForwardConfidence > moveBackwardConfidence)
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if (rand > probabilty)
                return "MoveForward";
        }
        else
        {
            rand = (UnityEngine.Random.Range(0.0f, 1.0f));
            if (rand > probabilty)
               return "MoveBackward";
        }

        return "idle";
    }
}
