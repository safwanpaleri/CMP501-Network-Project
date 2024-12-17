using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionDetection : MonoBehaviour
{

    [SerializeField] private PlayerAIController parent;
    [SerializeField] private PlayerAIController2 parent1;
    [SerializeField] private PlayerController parent2;
    [SerializeField] private PlayerController2 parent4;
    [SerializeField] private PlayerMultiplayerController parent3;

    bool sendOnce = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (!sendOnce)
            StartCoroutine(SendCollision(collision.gameObject));

        //Debug.LogWarning("Collision 0");
    }

    private IEnumerator SendCollision(GameObject collidedObject)
    {
        sendOnce = true;
        //Debug.LogWarning("collided 2");
        if (parent != null && parent.isActiveAndEnabled)
        {
            parent.CollisionDetected(collidedObject);
        }
        if (parent1 != null && parent1.isActiveAndEnabled)
        {
            parent1.CollisionDetected(collidedObject);
        }
        if (parent2 != null && parent2.isActiveAndEnabled)
        {
            parent2.CollisionDetected(collidedObject);
        }
        if (parent3 != null && parent3.isActiveAndEnabled)
        {
            parent3.CollisionDetected(collidedObject);
        }
        if (parent4 != null && parent4.isActiveAndEnabled)
        {
            parent4.CollisionDetected(collidedObject);
            //Debug.LogWarning("collided 2");
        }
        yield return new WaitForSeconds(1.0f);
        sendOnce = false;
    }
}
