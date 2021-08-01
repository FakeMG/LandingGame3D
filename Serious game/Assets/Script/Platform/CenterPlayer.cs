using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CenterPlayer : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    [SerializeField] public int id;

    private static int platformIDGenerator;
    private Rigidbody playerRB;

    void Start()
    {
        id = platformIDGenerator;
        platformIDGenerator++;
    }

    void OnCollisionStay(Collision collision)
    {
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
