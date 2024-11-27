using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public enum EPlayerMovementState
    {
        Idle,
        Walking,
        Running,
        Crouching,
    }

    public class PlayerMovement : MonoBehaviour
    {
        [Header("Camera")] [SerializeField] private Transform _playerCamera;
        [SerializeField] private float _sensitivity = 10;

        [Header("Player settings: ", order = 0)] 
        [SerializeField] public CharacterController Controller;

        [SerializeField] public float Gravity = -9.81f; 

        [FormerlySerializedAs("walkSpeed")]
        [Header("  1. Speed", order = 1)] 
        [SerializeField] private float _walkSpeed;

        [SerializeField] private float _speedChangeTime;
        [SerializeField] private float _sprintMultiplier;

        [FormerlySerializedAs("jumpHeight")]
        [Header("  2. Jump", order = 2)] 
        [SerializeField] private float _jumpHeight;

        [FormerlySerializedAs("crouchHeight")]
        [Header("  3. Crouch", order = 3)] 
        [SerializeField] private float _crouchHeight;
        [SerializeField] private float _crouchDuration;


        private PlayerInputActions _playerInputActions;
        private InputAction _moveAction;
        private InputAction _jumpAction;
        private InputAction _crouchAction;
        private InputAction _sprintAction;
        private InputAction _lookAction;

        // public variables
        [HideInInspector] public float speed;
        [HideInInspector] public Vector3 Velocity;
        [HideInInspector] public Vector3 MoveVector;
        [HideInInspector] public EPlayerMovementState PlayerMovementState;

        // private variables
        private float _standHeight;
        private Vector2 _look;

        // coroutines
        private Coroutine _crouchCoroutine;
        private Coroutine _speedChangeCoroutine;

        // constraints
        private Transform _constraintCenter;
        private float _constraintRadius = 10f;

        // events
        public delegate void PlayerStateDelegate();

        public PlayerStateDelegate OnPlayerStateChange;

        private void Awake()
        {
            _playerInputActions = new PlayerInputActions();
        }

        private void Start()
        {
            speed = _walkSpeed;
            _standHeight = transform.localScale.y;
            Cursor.lockState = CursorLockMode.Locked;
        }

        private void OnEnable()
        {
            // Initialize input actions
            _moveAction = _playerInputActions.Player.Move;
            _jumpAction = _playerInputActions.Player.Jump;
            _crouchAction = _playerInputActions.Player.Crouch;
            _sprintAction = _playerInputActions.Player.Sprint;
            _lookAction = _playerInputActions.Player.Look;
            // Enable input actions
            _moveAction.Enable();
            _jumpAction.Enable();
            _crouchAction.Enable();
            _sprintAction.Enable();
            _lookAction.Enable();
        }

        private void Update()
        {
            ConstrainPosition();
            UpdateLook();

            // Handle grounded state
            if (Controller.isGrounded && Velocity.y < 0)
            {
                Velocity.y = -2f;
            }

            // Movement
            Vector2 input = _moveAction.ReadValue<Vector2>();
            float x = input.x;
            float z = input.y;

            MoveVector = transform.right * x + transform.forward * z;
            if (MoveVector.magnitude < 0.1f)
            {
                PlayerMovementState = EPlayerMovementState.Idle;
            }
            else
            {
                PlayerMovementState = PlayerMovementState == EPlayerMovementState.Running
                    ? EPlayerMovementState.Running
                    : EPlayerMovementState.Walking;
            }

            Controller.Move(MoveVector * speed * Time.deltaTime);

            // Jump
            if (_jumpAction.triggered && Grounded())
            {
                Velocity.y = Mathf.Sqrt(_jumpHeight * -2f * Gravity);
            }

            // Apply gravity
            Velocity.y += Gravity * Time.deltaTime;
            Controller.Move(Velocity * Time.deltaTime);

            // Crouch
            if (_crouchAction.IsPressed())
            {
                PlayerMovementState = EPlayerMovementState.Crouching;
            }

            if (_crouchAction.triggered)
            {
                if (_crouchCoroutine != null) StopCoroutine(_crouchCoroutine);
                _crouchCoroutine = StartCoroutine(Crouch(true));
                AdjustSpeed(_walkSpeed / 2);
            }

            if (_crouchAction.WasReleasedThisFrame())
            {
                if (_crouchCoroutine != null) StopCoroutine(_crouchCoroutine);
                _crouchCoroutine = StartCoroutine(Crouch(false));
                AdjustSpeed(_walkSpeed);
                PlayerMovementState = EPlayerMovementState.Walking;
            }

            // Sprint
            if (_sprintAction.IsPressed() && PlayerMovementState != EPlayerMovementState.Crouching)
            {
                PlayerMovementState = EPlayerMovementState.Running;
                if (Grounded())
                {
                    AdjustSpeed(_walkSpeed * _sprintMultiplier);
                }
            }
            else if (PlayerMovementState != EPlayerMovementState.Crouching)
            {
                PlayerMovementState = EPlayerMovementState.Walking;
                AdjustSpeed(_walkSpeed);
            }
        }

        private void UpdateLook()
        {
            _look += _lookAction.ReadValue<Vector2>() * (_sensitivity * Time.deltaTime);
            _look.y = Mathf.Clamp(_look.y, -90, 90);

            transform.localRotation = Quaternion.Euler(0, _look.x, 0);
            _playerCamera.localRotation = Quaternion.Euler(-_look.y, 0, 0);
        }

        public bool Grounded()
        {
            float radius = transform.localScale.x / 2;
            return Physics.SphereCast(transform.position, radius, Vector3.down, out RaycastHit hit,
                transform.localScale.y);
        }

        private void AdjustSpeed(float newSpeed)
        {
            if (_speedChangeCoroutine != null) StopCoroutine(_speedChangeCoroutine);
            _speedChangeCoroutine = StartCoroutine(ChangeSpeed(newSpeed));
            OnPlayerStateChange?.Invoke();
        }

        private IEnumerator Crouch(bool crouching)
        {
            float t = 0;
            float desiredHeight = crouching ? _crouchHeight : _standHeight;
            while (t < _crouchDuration)
            {
                t += Time.deltaTime;

                float amount = t / _crouchDuration;
                float y = Mathf.Lerp(transform.localScale.y, desiredHeight, amount);
                transform.localScale = new Vector3(transform.localScale.x, y, transform.localScale.z);
                transform.Translate(Vector3.up * 0.01f);
                yield return null;
            }

            _crouchCoroutine = null;
        }

        private IEnumerator ChangeSpeed(float newSpeed)
        {
            float t = 0;
            while (t < _speedChangeTime)
            {
                t += Time.deltaTime;
                float amount = t / _speedChangeTime;
                speed = Mathf.Lerp(speed, newSpeed, amount);
                yield return null;
            }
            _speedChangeCoroutine = null;
        }

        private void ConstrainPosition()
        {
            if (_constraintCenter == null) return;

            Vector3 offset = transform.position - _constraintCenter.position;
            if (!(offset.magnitude > _constraintRadius))
                return;

            Vector3 constrainedPosition = _constraintCenter.position + offset.normalized * _constraintRadius;
            Controller.enabled = false;
            transform.position = constrainedPosition;
            Controller.enabled = true;
        }

        public void SetConstraint(Transform center, float radius)
        {
            _constraintCenter = center;
            _constraintRadius = radius;
        }

        public void RemoveConstraint()
        {
            _constraintCenter = null;
        }
    }
}