using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBehaviour : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] public int id;

    private static int platformIDGenerator;
    private Rigidbody playerRB;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        id = platformIDGenerator;
        platformIDGenerator++;
    }

    void OnCollisionEnter(Collision collision)
    {
        animator.SetTrigger("Squash");
    }

    void OnCollisionStay(Collision collision)
    {
        // Center player
        if (collision.transform.CompareTag("Player"))
        {
            if (collision.transform.GetComponent<PlayerMovement>().isLanded())
            {
                playerRB = collision.rigidbody;

                Vector3 targetPos = new Vector3(gameObject.transform.position.x, playerRB.position.y, playerRB.position.z);
                playerRB.position = Vector3.MoveTowards(playerRB.position, targetPos, moveSpeed * Time.deltaTime);
            }
        }
    }
}
