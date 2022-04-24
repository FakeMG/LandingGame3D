using UnityEngine;

public class PlatformBehaviour : MonoBehaviour {
    [SerializeField] private float moveSpeed;
    [SerializeField] public int id;

    private static int platformIDGenerator;
    private Animator animator;
    private Rigidbody playerRB;

    void Start() {
        animator = GetComponent<Animator>();
        id = platformIDGenerator;
        platformIDGenerator++;
    }

    private void Update() {
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Squash")) {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1) {
                animator.SetBool("PushDown", false);
            }
        }
    }

    void OnCollisionEnter(Collision collision) {
        animator.SetBool("PushDown", true);

        // Stop player
        if (collision.transform.CompareTag("Player")) {
            playerRB = collision.gameObject.GetComponent<Rigidbody>();
            if (playerRB.GetComponent<PlayerMovement>().isLanded()) {
                playerRB.velocity = Vector3.zero;
            }
        }
    }

    void OnCollisionExit(Collision collision) {
    }

    void OnCollisionStay(Collision collision) {
        // Center player
        if (playerRB != null) {
            if (playerRB.GetComponent<PlayerMovement>().isLanded()) {
                Vector3 targetPos = new Vector3(gameObject.transform.position.x, playerRB.position.y, playerRB.position.z);
                playerRB.position = Vector3.MoveTowards(playerRB.position, targetPos, moveSpeed * Time.deltaTime);
                playerRB.rotation = Quaternion.RotateTowards(playerRB.rotation, Quaternion.Euler(Vector3.zero), moveSpeed * Time.deltaTime);
            }
        }
    }
}
