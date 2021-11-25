using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class SwimmingSystem : MonoBehaviour
{
    [SerializeField]
    private InputActionReference floatingActionReference = null;
    [SerializeField]
    private ActionBasedContinuousMoveProvider moveProvider = null;
    [SerializeField]
    private XRRig xRRig;
    [SerializeField]
    private float underWaterMoveSpeed = 1.0f;
    [SerializeField]
    private float floatingSpeed = 20.0f;
    [SerializeField]
    private float fallingSlowDown = 10f;

    private Rigidbody rigidbody = null;
    private float oldMoveSpeed = 0.0f;
    private bool isUnderWater = false;
    private bool isFloating = false;

    private void Awake()
    {
        rigidbody = xRRig.gameObject.GetComponent<Rigidbody>();
        floatingActionReference.action.started += StartedFloating;
        floatingActionReference.action.canceled += CanceledFloating;
        oldMoveSpeed = moveProvider.moveSpeed;
    }

    private void Update()
    {
        moveProvider.moveSpeed = isUnderWater ? underWaterMoveSpeed : oldMoveSpeed;
        rigidbody.drag = isUnderWater ? fallingSlowDown : 0f;
    }

    private void FixedUpdate()
    {
        if (isUnderWater && isFloating)
            rigidbody.AddForce(Vector3.up * floatingSpeed);
    }

    private void StartedFloating(InputAction.CallbackContext callbackContext)
    {
        isFloating = true;
    }

    private void CanceledFloating(InputAction.CallbackContext callbackContext)
    {
        isFloating = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Water")
            isUnderWater = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Water")
            isUnderWater = false;
    }
}
