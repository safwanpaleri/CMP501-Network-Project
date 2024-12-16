using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildCollisionDetection : MonoBehaviour
{

    [SerializeField] private PlayerAIController parent;
    [SerializeField] private PlayerAIController2 parent1;
    [SerializeField] private PlayerController parent2;

    bool sendOnce = false;
    private void OnCollisionEnter(Collision collision)
    {
        if (!sendOnce)
            StartCoroutine(SendCollision(collision.gameObject));
    }

    private IEnumerator SendCollision(GameObject collidedObject)
    {
        sendOnce = true;
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
        yield return new WaitForSeconds(1.0f);
        sendOnce = false;
    }
}
