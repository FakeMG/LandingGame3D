using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemController : MonoBehaviour {

    [SerializeField] private ParticleSystem rightParticleSystem;
    [SerializeField] private ParticleSystem leftParticleSystem;
    private PlayerInput playerInput;
    private FuelController fuelController;
    private ParticleSystem.EmissionModule leftEmission;
    private ParticleSystem.EmissionModule rightEmission;

    // Start is called before the first frame update
    void Start() {
        playerInput = GetComponent<PlayerInput>();
        fuelController = GetComponent<FuelController>();

        leftEmission = leftParticleSystem.emission;
        rightEmission = rightParticleSystem.emission;
    }

    // Update is called once per frame
    void Update() {
        ControlEmission(leftEmission, playerInput.IsTouchingLeft);
        ControlEmission(rightEmission, playerInput.IsTouchingRight);
    }

    private void ControlEmission(ParticleSystem.EmissionModule emission, bool input) {
        if (input && !fuelController.IsEmpty()) {
            emission.rateOverTime = 50f;
        } else {
            emission.rateOverTime = 0f;
        }
    }
}
