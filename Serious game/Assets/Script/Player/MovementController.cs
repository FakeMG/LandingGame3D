using System.Collections;
using UnityEngine;

public class MovementController : MonoBehaviour {
    [SerializeField] private float horizontalForce = 12;
    [SerializeField] private float launchForce = 550;
    [SerializeField] private float pushUpForce = 12;
    [SerializeField] private float deadForce = 8;
    [Space]
    [SerializeField] private LayerMask whereCanLand;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 boxSize = new Vector3(.75f, .2f, .8f);
    public Animator animator;

    private FuelController fuelController;
    private PlayerInput playerInput;

    private bool dead = false;
    private bool landed;
    private bool preValOfLanded;
    private bool preValOfAllTouch;

    private WaitForSeconds WaitForIdling;
    private Coroutine idlingCoroutine;

    private Rigidbody rb;

    int landedHash = Animator.StringToHash("landed");
    int preLandedHash = Animator.StringToHash("preValOfLanded");
    int yVelHash = Animator.StringToHash("yVel");
    int idlingHash = Animator.StringToHash("idling");

    Quaternion target;

    // Start is called before the first frame update
    void Start() {
        //animator = playerModel.GetComponent<Animator>();
        fuelController = GetComponent<FuelController>();
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        WaitForIdling = new WaitForSeconds(6);
        target = rb.rotation * Quaternion.Euler(new Vector3(0, 0, -40));
    }

    private void Update() {
        if (LandAndHitGroundTooHard()) {
            Die();
        }
        if (landed) {
            fuelController.RefillFuel();
        }

        animator.SetFloat(yVelHash, rb.velocity.y);
        animator.SetBool(landedHash, landed);

        // Khi landed sẽ có 2 animation (idling, landing) nên ko thể dùng Trigger để chạy landing animation
        // Cần có preValOfLanded để landing animation chỉ chạy 1 lần khi mới tiếp đất
        animator.SetBool(preLandedHash, preValOfLanded);

        if (idlingCoroutine == null & landed) {
            idlingCoroutine = StartCoroutine(StartIdlingCoroutine());
        }
        if (!landed) {
            animator.SetBool(idlingHash, false);
            StopCoroutine(StartIdlingCoroutine());
        }
    }

    private bool LandAndHitGroundTooHard() {
        return landed && rb.velocity.y < -deadForce;
    }

    private void Die() {
        dead = true;
        rb.constraints = RigidbodyConstraints.None;
    }

    private void FixedUpdate() {
        CheckGround();

        if (!dead) {
            Move();
        }

    }

    private void CheckGround() {
        preValOfLanded = landed;
        Collider[] collider = Physics.OverlapBox(groundCheck.position, boxSize, Quaternion.identity, whereCanLand);
        if (collider.Length > 0) {
            landed = true;
        } else {
            landed = false;
        }
    }

    private void Move() {
        LaunchControl();

        FlyControl();

        RotatePlayer();
    }

    private void LaunchControl() {
        //Ko launch khi giữ nút từ trước
        if (landed && !preValOfAllTouch) {
            if (playerInput.IsTouchingLeft || playerInput.IsTouchingRight) {
                rb.velocity = new Vector3(100f, launchForce, 0) * Time.deltaTime;
            }
        }

        preValOfAllTouch = playerInput.IsTouchingLeft || playerInput.IsTouchingRight;
    }

    private void FlyControl() {
        if (!landed && !fuelController.IsEmpty()) {
            FlyLeftRightControl();

            SingleWingPushUp();

            DoubleWingsPushUp();
        }
    }

    private void FlyLeftRightControl() {
        if (playerInput.IsTouchingLeft && !playerInput.IsTouchingRight)
            rb.AddForce(Vector3.left * horizontalForce * Time.deltaTime, ForceMode.VelocityChange);
        if (!playerInput.IsTouchingLeft && playerInput.IsTouchingRight)
            rb.AddForce(Vector3.right * horizontalForce * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void SingleWingPushUp() {
        if ((playerInput.IsTouchingLeft && !playerInput.IsTouchingRight) || (!playerInput.IsTouchingLeft && playerInput.IsTouchingRight)) {
            rb.AddForce(Vector3.up * pushUpForce * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    private void DoubleWingsPushUp() {
        if (playerInput.IsTouchingLeft && playerInput.IsTouchingRight)
            rb.AddForce(Vector3.up * 1.5f * pushUpForce * Time.deltaTime, ForceMode.VelocityChange);
    }

    private void RotatePlayer() {
        if (playerInput.IsTouchingRight && !playerInput.IsTouchingLeft) {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, target, 20 * Time.deltaTime);
        } else if (!playerInput.IsTouchingRight && playerInput.IsTouchingLeft) {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.Inverse(target), 20 * Time.deltaTime);
        } else if (playerInput.IsTouchingRight && playerInput.IsTouchingLeft) {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.Euler(Vector3.zero), 20 * Time.deltaTime);
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawCube(groundCheck.position, boxSize);
    }

    IEnumerator StartIdlingCoroutine() {
        // Khi landed, sau 1 khoảng thời gian idling animation sẽ chạy
        yield return WaitForIdling;
        animator.SetBool(idlingHash, true);

        // Vì animation ko looping
        // Nên sau khi chạy đc 1 khoảng thời gian, sẽ tắt animation đi để có thể chạy lại
        yield return WaitForIdling;
        idlingCoroutine = null;
        animator.SetBool(idlingHash, false);
    }

    public bool isLanded() => landed;
    public bool isDead() => dead;
}
