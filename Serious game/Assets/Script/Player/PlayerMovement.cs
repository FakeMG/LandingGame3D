using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    private bool landed;
    private bool preValOfAllTouch;

    private float yVelBeforeLanded;
    private float remainingFuel;
    private Coroutine consumFuelCoroutine;
    private WaitForSeconds WaitForSeconds;

    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        WaitForSeconds = new WaitForSeconds(consumeRate);
        remainingFuel = maxFuel;
    }

    void Update()
    {
        if (rb.velocity.y < 0) yVelBeforeLanded = rb.velocity.y;
        if (landed && yVelBeforeLanded < -deadForce) die();
        if (landed) refillFuel();
    }

    void FixedUpdate()
    {
        Collider[] collider = Physics.OverlapBox(groundCheck.position, boxSize, Quaternion.identity, whereCanLand);
        if (collider.Length > 0) landed = true;
        else landed = false;

        if (landed)
        {
            Vector3 end = new Vector3(gameObject.transform.position.x + 17, Camera.main.transform.position.y, Camera.main.transform.position.z);
            Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, end, 50 * Time.deltaTime);
        }
    }

    public void move(bool touchLeft, bool touchRight)
    {
        //Launch
        if(landed && !preValOfAllTouch)
        {
            if (touchLeft || touchRight) rb.velocity = new Vector3(100f, launchForce, 0) * Time.deltaTime;
        }

        //Control
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

    public bool isLanded() => landed;
}
