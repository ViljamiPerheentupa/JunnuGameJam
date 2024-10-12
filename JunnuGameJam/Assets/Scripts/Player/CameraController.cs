using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public enum CameraState { Locked, Restricted, Free }

public class CameraController : MonoBehaviour
{
    public CameraState cameraState;

    public static CameraController Instance;

    [SerializeField] float mouseSensitivity, flySpeed, fastFlySpeed, smoothTime;

    public float verticalRotationMax;
    public float horizontalRotationMax;

    [SerializeField] GameObject cameraHolder;
    float verticalLookRotation;
    Vector3 smoothMoveVelocity;
    Vector3 moveAmount;

    public bool isAttachedToController = false;
    public GameObject controller;

    float rotationX;
    float rotationY;

    [SerializeField] Camera whiteRenderCam;

    private InputAction _lookAction;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        _lookAction = InputSystem.actions.FindAction("Look");
    }
    private void Update()
    {
        if (cameraState == CameraState.Locked)
        {
            moveAmount = Vector3.zero;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            return;
        }
        if (cameraState == CameraState.Restricted)
        {
            moveAmount = Vector3.zero;
            Look();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            return;
        }
        if(cameraState == CameraState.Free)
        {
            Look();
            Move();
        }

    }

    void Look()
    {
        Vector2 newLookValue = _lookAction.ReadValue<Vector2>();
        rotationX += newLookValue.x * mouseSensitivity;
        if (horizontalRotationMax != 0)
            rotationX = Mathf.Clamp(rotationX, -horizontalRotationMax, horizontalRotationMax);

        rotationY += newLookValue.y * mouseSensitivity;
        if (verticalRotationMax != 0)
            rotationY = Mathf.Clamp(rotationY, -verticalRotationMax, verticalRotationMax);

        if (!isAttachedToController)
            transform.localEulerAngles = new Vector3(-rotationY, rotationX, 0);
        else
        {
            controller.transform.localEulerAngles = Vector3.up * rotationX;
            transform.parent.localEulerAngles = Vector3.left * rotationY;
        }
    }

    public void ResetOrientation()
    {
        rotationX = 0;
        rotationY = 0;
        transform.localEulerAngles = Vector3.zero;
    }

    public void InheritRotation(Transform transform)
    {
        rotationX = transform.localEulerAngles.x;
        rotationY = transform.localEulerAngles.y;
    }

    void Move()
    {
        Vector3 moveDir = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;

        moveAmount = Vector3.SmoothDamp(moveAmount, moveDir * (Input.GetButton("Modifier") ? fastFlySpeed : flySpeed), ref smoothMoveVelocity, smoothTime);
    }

    private void FixedUpdate()
    {
        if (cameraState == CameraState.Free)
            transform.position = transform.position + transform.TransformDirection(moveAmount) * Time.fixedDeltaTime;
    }
    public void StartWhiteRender()
    {
        whiteRenderCam.depth = 2;
    }

    public void EndWhiteRender()
    {
        whiteRenderCam.depth = -1;
    }
}
