using System.Collections;
using UnityEngine;

public class FuelController : MonoBehaviour {
    [SerializeField] private float maxFuel = 18;
    [SerializeField] private float consumeAmount = 2;
    [SerializeField] private float consumeRate = 0.3f;

    private float remainingFuel;
    private Coroutine consumFuelCoroutine;
    private WaitForSeconds WaitForConsumeFuel;

    private PlayerInput playerInput;

    private void Start() {
        playerInput = GetComponent<PlayerInput>();
        WaitForConsumeFuel = new WaitForSeconds(consumeRate);
        remainingFuel = maxFuel;
    }

    private void Update() {
        ConsumeFuel();
    }

    private void ConsumeFuel() {
        if (playerInput.IsTouchingLeft || playerInput.IsTouchingRight) {
            if (consumFuelCoroutine == null) {
                if (playerInput.IsTouchingLeft && playerInput.IsTouchingRight)
                    consumFuelCoroutine = StartCoroutine(ConsumeFuelCoroutine(consumeAmount * 2));
                else
                    consumFuelCoroutine = StartCoroutine(ConsumeFuelCoroutine(consumeAmount));
            }
        }
    }

    IEnumerator ConsumeFuelCoroutine(float p_consumeAmount) {
        if (remainingFuel > 0)
            remainingFuel -= p_consumeAmount;
        yield return WaitForConsumeFuel;
        consumFuelCoroutine = null;
    }

    public void RefillFuel() {
        remainingFuel = maxFuel;
    }

    public bool IsEmpty() {
        if (remainingFuel <= 0) {
            return true;
        }
        return false;
    }
}