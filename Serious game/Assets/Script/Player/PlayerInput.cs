using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour {
    private bool touchRight, touchLeft;

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {

        TouchInput();
        KeyboardInput();
    }

    private void TouchInput() {
        touchLeft = touchRight = false;

        for (int i = 0; i < Input.touchCount; i++) {
            if (Input.touches[i].position.x > Screen.width / 2) {
                touchRight = true;
            } else
                touchLeft = true;
        }
    }

    private void KeyboardInput() {
        if (Input.GetKey(KeyCode.A))
            touchLeft = true;
        if (Input.GetKey(KeyCode.D))
            touchRight = true;
    }

    public bool IsTouchingRight {
        get { return touchRight; }
    }

    public bool IsTouchingLeft {
        get { return touchLeft; }
    }
}
