using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerAIController2 : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private Text playerName;
    [SerializeField] private Slider healthSlider;


    public Transform player;
    public float health = 100f;
    public float punchRange = 1.5f;
    public float kickRange = 2.5f;
    public float intenseThreshold = 30f;


    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty difficulty = Difficulty.Medium;

    private float reactionTime = 0.5f;
    private float reactionprobability = 0.5f;
    private PlayerController playerController;

    private string healthState;
    private string distanceState;
    private string playerState;
    private float punchProbability;
    private float kickProbability;
    private float defendProbability;
    private float moveForwardProbability;
    private float moveBackwardProbability;

    private bool isDefending = false;
    [HideInInspector] public bool isAttacking = false;

    private void Start()
    {
        animator = GetComponent<Animator>();
        playerController = player.GetComponent<PlayerController>();
        StartCoroutine(Reaction());
    }

    private IEnumerator Reaction()
    {
        // Get state variables
        healthState = GetHealthStatus();
        distanceState = GetDistanceStatusBetweenPlayerAndOpponent();
        playerState = GetPlayerStatus();

        // Compute probabilities for each action
        punchProbability = PunchingProbability(healthState, distanceState, playerState);
        kickProbability = KickingProbability(healthState, distanceState, playerState);
        defendProbability = DefendingProbability(healthState, distanceState, playerController.isAttacking);
        moveForwardProbability = MoveForwardProbability(healthState, distanceState, playerState);
        moveBackwardProbability = MoveBackwardProbability(healthState, distanceState, playerState);

        // Choose the action with the highest probability
        float maxProbability = Mathf.Max(punchProbability, kickProbability, defendProbability, moveForwardProbability, moveBackwardProbability);
        float actionProbability = UnityEngine.Random.Range(0.0f,1.0f);

        if (actionProbability > reactionprobability)
        {
            if (maxProbability == punchProbability)
                Punch();
            else if (maxProbability == kickProbability)
                Kick();
            else if (maxProbability == moveForwardProbability)
                MoveForward();
            else
                MoveBackward();
        }

        if(playerController.isAttacking)
        {
            if ( actionProbability > defendProbability)
                Defend();
        }

        yield return new WaitForSeconds(reactionTime);
        StartCoroutine(Reaction());
    }

    void Update()
    {
        
    }

    // State Determination
    string GetHealthStatus()
    {
        if (health <= intenseThreshold) return "Low";
        else if (health <= intenseThreshold * 2) return "Medium";
        else return "High";
    }

    string GetDistanceStatusBetweenPlayerAndOpponent()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        if (distance <= punchRange) return "Close";
        else if (distance <= kickRange) return "Medium";
        else return "Far";
    }

    string GetPlayerStatus()
    {
        if (playerController.isAttacking)
            return "Attacking";
        else if (playerController.isDefending)
            return "Defending";
        else
            return "Idle";
    }

    // Bayesian Probability Computation
    float PunchingProbability(string health, string distance, string playerAction)
    {
        if (distance == "Close" && playerAction != "Defending")
            return health == "High" ? 0.8f : health == "Medium" ? 0.5f : 0.3f;
        return 0.1f;
    }

    float KickingProbability(string health, string distance, string playerAction)
    {
        if (distance == "Medium")
            return health == "High" ? 0.7f : health == "Medium" ? 0.4f : 0.2f;
        return 0.1f;
    }

    float DefendingProbability(string health, string distance, bool isPlayerAttacking)
    {
        if (isPlayerAttacking)
            return health == "Low" ? 0.8f : health == "Medium" ? 0.6f : 0.4f;
        return 0.2f;
    }

    float MoveForwardProbability(string health, string distance, string playerAction)
    {
        if (distance == "Far")
            return health == "High" ? 0.8f : health == "Medium" ? 0.5f : 0.3f;
        return 0.2f;
    }

    float MoveBackwardProbability(string health, string distance, string playerAction)
    {
        if (health == "Low" && distance == "Close")
            return 0.9f;
        return 0.2f;
    }

    // Difficulty Adjustment
    void AdjustAccordingToDifficulty(ref float punch, ref float kick, ref float defend, ref float moveForward, ref float moveBackward)
    {
        switch (difficulty)
        {
            case Difficulty.Easy:
                reactionTime = 1.0f;
                reactionprobability = 0.75f;
                break;

            case Difficulty.Medium:
                reactionprobability = 0.5f;
                reactionTime = 0.5f;
                break;

            case Difficulty.Hard:
                reactionprobability = 0.2f;
                reactionTime = 0.2f;
                break;
        }
    }

    // AI Actions
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
                    if(playerController != null)
                        playerController.Win();
                    Lose();
                }
            }
        }
        //Debug.Log("collision: " + collision.name);
    }
}
