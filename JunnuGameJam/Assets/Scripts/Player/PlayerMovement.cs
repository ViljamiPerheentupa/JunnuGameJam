using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour, IForceInheritance, IChangeMovementData
{
    public static PlayerMovement Instance;

    public MovementData currentData;
    [SerializeField] MovementData defaultData;
    [SerializeField] MovementData sprintData;
    [SerializeField] MovementData slowData;

    [SerializeField] private float staminaPerJump;

    public float gravity;

    [SerializeField] float coyoteTimeMax;
    float coyoteTime;

    private CharacterController characterController;

    bool wishJump;
    bool jumpQueue;

    float jumpQueueTime;
    [SerializeField] float maxJumpQueueTime;

    private Vector2 inputVector;
    private Vector3 playerVelocity;

    [SerializeField] float distancePerStep = 1f;
    float stepDistance;
    int lastStepIndex = -1;
    Vector3 lastPosition;

    bool canMove = true;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _sprintAction;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
        characterController = GetComponent<CharacterController>();
        currentData = defaultData;
        lastPosition = transform.position;
        _moveAction = InputSystem.actions.FindAction("Move");
        _jumpAction = InputSystem.actions.FindAction("Jump");
        _sprintAction = InputSystem.actions.FindAction("Sprint");
    }

    private void Update()
    {
        if (Stamina.Instance.CanRun() && currentData != sprintData)
        {
            currentData = defaultData;
        } else if(!Stamina.Instance.CanRun())
        {
            currentData = slowData;
        }
        //Vector3 oldPosition = transform.position;
        if (canMove)
        {
            CoyoteControl();
            GetInput();
            if (characterController.isGrounded)
            {
                GroundMove();
                FootstepAudio();
            }
            else if (!characterController.isGrounded)
            {
                AirMove();
            }
            if (playerVelocity.magnitude != 0)
                characterController.Move(playerVelocity * Time.deltaTime);
        }
        //movement = transform.position - oldPosition;
        //if(!PV.IsMine)
        //{
        //    LagCompensation();
        //}
    }

    // Lag Compensation without the Photon Transform View component. Kinda laggy, but snappier than the component. If lag becomes a problem during playtesting, I'll see if I need to refine this.
    #region Lag Compensation
    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(transform.position);
    //        stream.SendNext(transform.rotation);
    //        stream.SendNext(movement);
    //    }
    //    else
    //    {
    //        networkPosition = (Vector3)stream.ReceiveNext();
    //        networkRotation = (Quaternion)stream.ReceiveNext();
    //        movement = (Vector3)stream.ReceiveNext();

    //        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
    //        networkPosition += movement * lag;
    //    }
    //}

    //void LagCompensation()
    //{
    //    transform.position = Vector3.MoveTowards(transform.position, networkPosition, Time.deltaTime * currentData.movementSpeed);
    //    transform.rotation = Quaternion.RotateTowards(transform.rotation, networkRotation, Time.deltaTime * 100);
    //}
    #endregion

    void FootstepAudio()
    {
        if (inputVector.magnitude != 0)
        {
            stepDistance += Vector3.Distance(lastPosition, transform.position);
        }
        if (stepDistance >= distancePerStep)
        {
            AudioFW.Instance.PlaySound("Footstep" + StepRandomizer());
            stepDistance = 0;
        }
        lastPosition = transform.position;
    }

    int StepRandomizer()
    {
        int stepIndex = Random.Range(0, 4);
        if(lastStepIndex == -1)
        {
            lastStepIndex = stepIndex;
            return stepIndex;
        } else
        {
            while(stepIndex == lastStepIndex)
            {
                stepIndex = Random.Range(0, 4);
            }
        } return stepIndex;
        
    }


    void Accelerate(Vector3 wishdir, float wishspeed, float accel)
    {
        float currentspeed = Vector3.Dot(playerVelocity, wishdir);
        float addspeed = wishspeed - currentspeed;
        if (addspeed <= 0)
            return;
        float accelspeed = accel * Time.deltaTime * wishspeed;
        if (accelspeed > addspeed)
            accelspeed = addspeed;

        playerVelocity.x += accelspeed * wishdir.x;
        playerVelocity.z += accelspeed * wishdir.z;
    }

    #region Ground Movement
    void GroundMove()
    {
        ApplyFriction(1.0f);

        Vector3 wishdir = new Vector3(inputVector.x, 0, inputVector.y);
        wishdir = transform.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;
        wishspeed *= currentData.movementSpeed;

        Accelerate(wishdir, wishspeed, currentData.runAcceleration);

        if (wishJump)
        {
            Jump();
            if (Stamina.Instance.CanRun())
            {
                Stamina.Instance.ReduceStamina(staminaPerJump);
            }
        } else
        {
            float g = -1f;
            playerVelocity.y += g * Time.deltaTime;
            playerVelocity.y = Mathf.Clamp(playerVelocity.y, -1, Mathf.Infinity);
        }
        void ApplyFriction(float t)
        {
            Vector3 vec = playerVelocity;
            vec.y = 0;
            float speed = vec.magnitude;
            float drop = 0f;

            if (characterController.isGrounded)
            {
                float control = speed < currentData.runDeacceleration ? currentData.runDeacceleration : speed;
                drop = control * currentData.friction * Time.deltaTime * t;
            }

            float newspeed = speed - drop;
            if (newspeed < 0)
                newspeed = 0;
            if (speed > 0)
                newspeed /= speed;

            playerVelocity.x *= newspeed;
            playerVelocity.z *= newspeed;
        }

        void Jump()
        {
            playerVelocity.y = currentData.jumpForce;
            wishJump = false;
        }
    }

    public void AddJumpForce(float amount)
    {
        playerVelocity.y = amount;
    }
    #endregion
    #region Air Movement
    void AirMove()
    {
        Vector3 wishdir = new Vector3(inputVector.x, 0, inputVector.y);
        wishdir = transform.TransformDirection(wishdir);

        float wishspeed = wishdir.magnitude;

        wishspeed *= 7f;

        float wishspeed2 = wishspeed;
        float accel;
        if (Vector3.Dot(playerVelocity, wishdir) < 0)
            accel = currentData.airDeacceleration;
        else
            accel = currentData.airAcceleration;

        if(inputVector.x == 0 && inputVector.y != 0)
        {
            if (wishspeed > currentData.sideStrafeSpeed)
                wishspeed = currentData.sideStrafeSpeed;
            accel = currentData.sideStrafeAcceleration;
        }

        Accelerate(wishdir, wishspeed, accel);

        AirControl(wishdir, wishspeed2);

        // Apply Gravity
        playerVelocity.y += gravity * Time.deltaTime;

        void AirControl(Vector3 wishdir, float wishspeed)
        {
            if (inputVector.x == 0 || wishspeed == 0)
                return;

            float zspeed = playerVelocity.y;
            playerVelocity.y = 0;

            float speed = playerVelocity.magnitude;
            playerVelocity.Normalize();

            float dot = Vector3.Dot(playerVelocity, wishdir);
            float k = 32f;
            k *= currentData.airControl * dot * dot * Time.deltaTime;

            if (dot > 0)
            {
                playerVelocity.x = playerVelocity.x * speed + wishdir.x * k;
                playerVelocity.y = playerVelocity.y * speed + wishdir.y * k;
                playerVelocity.z = playerVelocity.z * speed + wishdir.z * k;

                playerVelocity.Normalize();
            }

            playerVelocity.x *= speed;
            playerVelocity.y = zspeed;
            playerVelocity.z *= speed;
        }
    }
    #endregion
    #region Input
    void GetInput()
    {
        inputVector = _moveAction.ReadValue<Vector2>();
        QueueJump();
        Sprint();

        void QueueJump()
        {
            if (jumpQueue)
            {
                jumpQueueTime += Time.deltaTime;
                if (jumpQueueTime >= maxJumpQueueTime)
                {
                    jumpQueue = false;
                    jumpQueueTime = 0;
                }
            }
            if (isGrounded())
            {
                if (_jumpAction.WasPressedThisFrame() && !jumpQueue)
                {
                    wishJump = true;
                } else if (jumpQueue)
                {
                    wishJump = true;
                    jumpQueue = false;
                }
            }
            if(!isGrounded() && _jumpAction.WasPressedThisFrame())
            {
                jumpQueue = true;
                jumpQueueTime = 0;
            }
        }

        void Sprint()
        {
            if(_sprintAction.IsPressed() && currentData != sprintData && Stamina.Instance.CanRun())
            {
                currentData = sprintData;
                Stamina.Instance.SetStaminaUseState(true);
            } else if((currentData.dataName == "Sprint" && !_sprintAction.IsPressed()) || (currentData.dataName == "Sprint" && !Stamina.Instance.CanRun()))
            {
                Stamina.Instance.SetStaminaUseState(false);
            }
        }
    }
    #endregion
    #region GroundCheck
    bool isGrounded()
    {
        return coyoteTime < coyoteTimeMax;
    }

    void CoyoteControl()
    {
        if (characterController.isGrounded)
        {
            coyoteTime = 0;
        }
        else
        {
            coyoteTime += Time.deltaTime;
        }
    }
    #endregion
    #region Interface Functions
    public void ChangeMovementData(MovementData data)
    {
        currentData = data;
    }
    public void RevertMovementData()
    {
        currentData = defaultData;
    }

    public void ChangeGravity(float _gravity)
    {
        gravity = _gravity;
    }
    public void InheritForce(Vector3 forceVector, float accelerationAmount)
    {
        Accelerate(forceVector, forceVector.sqrMagnitude, accelerationAmount);
    }
    #endregion
    public void DisableMovement()
    {
        canMove = false;
    }

    public void EnableMovement()
    {
        canMove = true;
    }

    public void StopMovement()
    {
        playerVelocity = Vector3.zero;
    }
}
