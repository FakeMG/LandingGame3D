using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float horizontalForce;
    [SerializeField] private float launchForce;
    [SerializeField] private float pushUpForce;
    [SerializeField] private float deadForce;
    [Space]
    [SerializeField] private float maxFuel;
    [SerializeField] private float consumeAmount;
    [SerializeField] private float consumeRate;
    [Space]
    [SerializeField] private LayerMask whereCanLand;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 boxSize = new Vector3(.75f, .2f, .8f);
    public Animator animator;

    private bool landed;
    private bool preValOfLanded;
    private bool preValOfAllTouch;

    private float yVelBeforeLanded;
    private float remainingFuel;
    private Coroutine consumFuelCoroutine;
    private WaitForSeconds WaitForSeconds;
    private WaitForSeconds WaitForIdling;
    private Coroutine idlingCoroutine;

    private Rigidbody rb;

    int landedHash = Animator.StringToHash("landed");
    int preLandedHash = Animator.StringToHash("preValOfLanded");
    int yVelHash = Animator.StringToHash("yVel");
    int idlingHash = Animator.StringToHash("idling");

    Quaternion target;

    // Start is called before the first frame update
    void Start()
    {
        //animator = playerModel.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        WaitForSeconds = new WaitForSeconds(consumeRate);
        WaitForIdling = new WaitForSeconds(6);
        remainingFuel = maxFuel;
        target = rb.rotation * Quaternion.Euler(new Vector3(0, 0, 20));
    }

    void Update()
    {
        if (rb.velocity.y < 0) yVelBeforeLanded = rb.velocity.y;
        if (landed && yVelBeforeLanded < -deadForce) die();
        if (landed) refillFuel();

        animator.SetFloat(yVelHash, rb.velocity.y);
        animator.SetBool(landedHash, landed);

        // Khi landed s? có 2 animation (idling, landing) nên ko th? dùng Trigger ?? ch?y landing animation
        // C?n có preValOfLanded ?? landing animation ch? ch?y 1 l?n khi m?i ti?p ??t
        animator.SetBool(preLandedHash, preValOfLanded);

        if (idlingCoroutine == null & landed) idlingCoroutine = StartCoroutine(startIdling());
        if (!landed) animator.SetBool(idlingHash, false);
    }

    void FixedUpdate()
    {
        // Ground check
        preValOfLanded = landed;
        Collider[] collider = Physics.OverlapBox(groundCheck.position, boxSize, Quaternion.identity, whereCanLand);
        if (collider.Length > 0)
        {
            landed = true;
        }
        else
        {
            landed = false;
        }

        //rb.rotation = Quaternion.RotateTowards(rb.rotation, target, 5*Time.deltaTime);
    }

    public void move(bool touchLeft, bool touchRight)
    {
        //Launch
        if(landed && !preValOfAllTouch)
        {
            if (touchLeft || touchRight) rb.velocity = new Vector3(100f, launchForce, 0) * Time.deltaTime;
        }

        //Touch control
        if(!landed && remainingFuel > 0)
        {
            Vector3 force = new Vector3(horizontalForce, 0, 0);
            if (touchLeft && !touchRight) rb.AddForce(-force * Time.deltaTime, ForceMode.VelocityChange);
            if (!touchLeft && touchRight) rb.AddForce(force * Time.deltaTime, ForceMode.VelocityChange);

            if ((touchLeft && !touchRight) || (!touchLeft && touchRight))
            {
                rb.AddForce(Vector3.up * pushUpForce * Time.deltaTime, ForceMode.VelocityChange);
            }

            if (touchLeft && touchRight) rb.AddForce(Vector3.up * 1.5f * pushUpForce * Time.deltaTime, ForceMode.VelocityChange);
        }

        //Consume fuel
        if(touchLeft || touchRight)
        {
            if(consumFuelCoroutine == null)
            {
                if (touchLeft && touchRight) consumFuelCoroutine = StartCoroutine(consumeFuel(consumeAmount*2));
                else consumFuelCoroutine = StartCoroutine(consumeFuel(consumeAmount));
            }
        }

        preValOfAllTouch = touchLeft || touchRight;
    }

    void die()
    {
        gameObject.SetActive(false);
    }

    void refillFuel()
    {
        remainingFuel = maxFuel;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(groundCheck.position, boxSize);
    }

    IEnumerator consumeFuel(float p_consumeAmount)
    {
        if (remainingFuel > 0) remainingFuel -= p_consumeAmount;
        yield return WaitForSeconds;
        consumFuelCoroutine = null;
    }

    IEnumerator startIdling()
    {
        // Khi landed, sau 1 kho?ng th?i gian idling animation s? ch?y
        yield return WaitForIdling;
        animator.SetBool(idlingHash, true);
        // Vì animation ko looping
        // Nên sau khi ch?y ?c 1 kho?ng th?i gian, s? t?t animation ?i ?? có th? ch?y l?i
        yield return WaitForIdling;
        idlingCoroutine = null;
        animator.SetBool(idlingHash, false);
    }

    public bool isLanded() => landed;
}
