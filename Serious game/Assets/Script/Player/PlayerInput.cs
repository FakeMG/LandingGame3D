using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    private PlayerMovement controller;
    private bool touchRight, touchLeft;

    // Start is called before the first frame update
    void Start() {
        controller = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    void Update() {
        touchLeft = touchRight = false;

        //touch input
        for (int i = 0; i < Input.touchCount; i++) {

            Debug.Log(Input.touches[i].position.x);
            if (Input.touches[i].position.x > Screen.width / 2) touchRight = true;
            else touchLeft = true;
        }

        //keyboard input
        if (Input.GetKey(KeyCode.A)) touchLeft = true;
        if (Input.GetKey(KeyCode.D)) touchRight = true;
    }

    private void FixedUpdate() {
        if (!controller.isDead()) {
            controller.move(touchLeft, touchRight);
        }
    }
}
