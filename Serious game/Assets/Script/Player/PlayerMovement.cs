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
    public CinemachineVirtualCamera virtualCamera;
    public GameObject playerModel;
    private Animator animator;

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

    // Start is called before the first frame update
    void Start()
    {
        animator = playerModel.GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        WaitForSeconds = new WaitForSeconds(consumeRate);
        WaitForIdling = new WaitForSeconds(6);
        remainingFuel = maxFuel;
    }

    void Update()
    {
        if (rb.velocity.y < 0) yVelBeforeLanded = rb.velocity.y;
        if (landed && yVelBeforeLanded < -deadForce) die();
        if (landed) refillFuel();


        //Control camera
        //https://stackoverflow.com/questions/59346229/change-camera-distance-of-cinemachine-in-script
        //
        CinemachineComponentBase componentBase = virtualCamera.GetCinemachineComponent(CinemachineCore.Stage.Body);
        if (componentBase is CinemachineFramingTransposer)
        {
            if (landed)
            {
                (componentBase as CinemachineFramingTransposer).m_DeadZoneWidth = 0;
                (componentBase as CinemachineFramingTransposer).m_XDamping = 3;
            }
            else
            {
                (componentBase as CinemachineFramingTransposer).m_DeadZoneWidth = .7f;
                (componentBase as CinemachineFramingTransposer).m_XDamping = 5;
            }
        }

        animator.SetFloat(yVelHash, rb.velocity.y);
        animator.SetBool(landedHash, landed);
        animator.SetBool(preLandedHash, preValOfLanded);
        if (idlingCoroutine == null & landed) idlingCoroutine = StartCoroutine(startIdling());
        if (!landed) animator.SetBool(idlingHash, false);
    }

    void FixedUpdate()
    {
        preValOfLanded = landed;
        Collider[] collider = Physics.OverlapBox(groundCheck.position, boxSize, Quaternion.identity, whereCanLand);
        if (collider.Length > 0) landed = true;
        else landed = false;
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

            if(touchLeft || touchRight) rb.AddForce(Vector3.up * pushUpForce * Time.deltaTime, ForceMode.VelocityChange);
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
        Destroy(gameObject);
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
        yield return WaitForIdling;
        animator.SetBool(idlingHash, true);
        yield return WaitForIdling;
        idlingCoroutine = null;
        animator.SetBool(idlingHash, false);
    }

    public bool isLanded() => landed;
}
