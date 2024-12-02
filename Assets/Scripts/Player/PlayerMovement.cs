using System.Collections;
using LegSystem;
using UnityEngine;
using UnityEngine.InputSystem;
using LimbsPickup;

namespace Player
{
    public enum EPlayerMovementState
    {
        Idle,
        Walking,
        Running,
    }

    public class PlayerMovement : MonoBehaviour
    {
        public static PlayerMovement Instance;
        [Header("Camera")] [SerializeField] private Transform _playerCamera;
        [SerializeField] private float _sensitivity = 10;
        [SerializeField] private CameraController _cameraController;

        [Header("Player settings: ", order = 0)] [SerializeField]
        public CharacterController Controller;

        [SerializeField] public float Gravity = -9.81f;

        [Header("  1. Speed", order = 1)] [SerializeField]
        private float _walkSpeed;

        [SerializeField] private float _speedChangeTime;
        [SerializeField] private float _sprintMultiplier;

        [Header("  2. Jump", order = 2)] [SerializeField]
        private float _jumpHeight;
        
        private PlayerInputActions _playerInputActions;
        private InputAction _moveAction;
        private InputAction _sprintAction;
        private InputAction _lookAction;
        private Vector2 _smoothedInput;

        // public variables
        [HideInInspector] public float Speed;
        [HideInInspector] public Vector3 Velocity;
        [HideInInspector] public Vector3 MoveVector;
        [HideInInspector] public float X, Y;
        [HideInInspector] public EPlayerMovementState PlayerMovementState;

        // private variables
        private float _standHeight;


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
            Instance = this;
            _playerInputActions = new PlayerInputActions();
        }

        private void Start()
        {
            Speed = _walkSpeed;
            _standHeight = transform.localScale.y;
            Cursor.lockState = CursorLockMode.Locked;
            _cameraController = CameraController.Instance;
        }

        private void OnEnable()
        {
            // Initialize input actions
            _moveAction = _playerInputActions.Player.Move;
            _sprintAction = _playerInputActions.Player.Sprint;
            _lookAction = _playerInputActions.Player.Look;
            // Enable input actions
            _moveAction.Enable();
            _sprintAction.Enable();
            _lookAction.Enable();
        }

        private void Update()
        {
            ConstrainPosition();

            // Handle grounded state
            if (Controller.isGrounded && Velocity.y < 0)
            {
                Velocity.y = -2f;
            }

            // Movement
            Vector2 input = _moveAction.ReadValue<Vector2>();

            // smooth input
            _smoothedInput = Vector2.Lerp(_smoothedInput, input, 0.04f);

            X = _smoothedInput.x;
            Y = _smoothedInput.y;

            MoveVector = transform.right * X + transform.forward * Y;
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

            float constraintSpeed = Leg.Instance == null ^ LimbsController.Instance.Leg.enabled == false ? 1 : 2;

            Controller.Move(MoveVector * ((Speed / constraintSpeed) * Time.deltaTime));

            // Apply gravity
            Velocity.y += Gravity * Time.deltaTime;
            Controller.Move(Velocity * Time.deltaTime);

            // Sprint
            if (_sprintAction.IsPressed())
            {
                PlayerMovementState = EPlayerMovementState.Running;
                if (Grounded())
                {
                    AdjustSpeed(_walkSpeed * _sprintMultiplier);
                }
            }
            else
            {
                PlayerMovementState = EPlayerMovementState.Walking;
                AdjustSpeed(_walkSpeed);
            }

            UpdateLook();
        }

        private void UpdateLook()
        {
            _cameraController.UpdateLook(_lookAction, transform);
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

        private IEnumerator ChangeSpeed(float newSpeed)
        {
            float t = 0;
            while (t < _speedChangeTime)
            {
                t += Time.deltaTime;
                float amount = t / _speedChangeTime;
                Speed = Mathf.Lerp(Speed, newSpeed, amount);
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