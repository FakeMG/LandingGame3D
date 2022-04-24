using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    [SerializeField] private float horizontalForce = 12;
    [SerializeField] private float launchForce = 550;
    [SerializeField] private float pushUpForce = 12;
    [SerializeField] private float deadForce = 8;
    [Space]
    [SerializeField] private float maxFuel = 18;
    [SerializeField] private float consumeAmount = 2;
    [SerializeField] private float consumeRate = 0.3f;
    [Space]
    [SerializeField] private LayerMask whereCanLand;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 boxSize = new Vector3(.75f, .2f, .8f);
    public Animator animator;

    private bool dead = false;
    private bool landed;
    private bool preValOfLanded;
    private bool preValOfAllTouch;

    private float yVelBeforeLanded;
    private float remainingFuel;
    private Coroutine consumFuelCoroutine;
    private WaitForSeconds WaitForConsumeFuel;
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
        rb = GetComponent<Rigidbody>();
        WaitForConsumeFuel = new WaitForSeconds(consumeRate);
        WaitForIdling = new WaitForSeconds(6);
        remainingFuel = maxFuel;
        target = rb.rotation * Quaternion.Euler(new Vector3(0, 0, -40));
    }

    void Update() {
        if (rb.velocity.y < 0) {
            yVelBeforeLanded = rb.velocity.y;
        }
        if (landed && yVelBeforeLanded < -deadForce) {
            die();
        }
        if (landed) {
            refillFuel();
        }

        animator.SetFloat(yVelHash, rb.velocity.y);
        animator.SetBool(landedHash, landed);

        // Khi landed sẽ có 2 animation (idling, landing) nên ko thể dùng Trigger để chạy landing animation
        // Cần có preValOfLanded để landing animation chỉ chạy 1 lần khi mới tiếp đất
        animator.SetBool(preLandedHash, preValOfLanded);

        if (idlingCoroutine == null & landed) {
            idlingCoroutine = StartCoroutine(startIdling());
        }
        if (!landed) {
            animator.SetBool(idlingHash, false);
            StopCoroutine(startIdling());
        }
    }

    void FixedUpdate() {
        checkGround();
    }

    // Dùng trong "PlayerInput.cs"
    public void move(bool touchLeft, bool touchRight) {
        launchControl(touchLeft, touchRight);

        movementControl(touchLeft, touchRight);


        //Consume fuel
        if (touchLeft || touchRight) {
            if (consumFuelCoroutine == null) {
                if (touchLeft && touchRight) consumFuelCoroutine = StartCoroutine(consumeFuel(consumeAmount * 2));
                else consumFuelCoroutine = StartCoroutine(consumeFuel(consumeAmount));
            }
        }

        rotatePlayer(touchRight, touchLeft);

        preValOfAllTouch = touchLeft || touchRight;
    }

    void die() {
        dead = true;
        rb.constraints = RigidbodyConstraints.None;
    }

    void refillFuel() {
        remainingFuel = maxFuel;
    }

    void launchControl(bool touchLeft, bool touchRight) {
        //Ko launch khi giữ nút từ trước
        if (landed && !preValOfAllTouch) {
            if (touchLeft || touchRight) rb.velocity = new Vector3(100f, launchForce, 0) * Time.deltaTime;
        }
    }

    void movementControl(bool touchLeft, bool touchRight) {
        if (!landed && remainingFuel > 0) {
            Vector3 force = new Vector3(horizontalForce, 0, 0);
            if (touchLeft && !touchRight) rb.AddForce(-force * Time.deltaTime, ForceMode.VelocityChange);
            if (!touchLeft && touchRight) rb.AddForce(force * Time.deltaTime, ForceMode.VelocityChange);

            if ((touchLeft && !touchRight) || (!touchLeft && touchRight)) {
                rb.AddForce(Vector3.up * pushUpForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            if (touchLeft && touchRight) rb.AddForce(Vector3.up * 1.5f * pushUpForce * Time.deltaTime, ForceMode.VelocityChange);
        }
    }

    void rotatePlayer(bool touchRight, bool touchLeft) {
        if (touchRight && !touchLeft) {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, target, 20 * Time.deltaTime);
        } else if (!touchRight && touchLeft) {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.Inverse(target), 20 * Time.deltaTime);
        } else if (touchRight && touchLeft) {
            rb.rotation = Quaternion.RotateTowards(rb.rotation, Quaternion.Euler(Vector3.zero), 20 * Time.deltaTime);
        }
    }

    void checkGround() {
        preValOfLanded = landed;
        Collider[] collider = Physics.OverlapBox(groundCheck.position, boxSize, Quaternion.identity, whereCanLand);
        if (collider.Length > 0) {
            landed = true;
        } else {
            landed = false;
        }
    }

    private void OnDrawGizmos() {
        Gizmos.DrawCube(groundCheck.position, boxSize);
    }

    IEnumerator consumeFuel(float p_consumeAmount) {
        if (remainingFuel > 0) remainingFuel -= p_consumeAmount;
        yield return WaitForConsumeFuel;
        consumFuelCoroutine = null;
    }

    IEnumerator startIdling() {
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
