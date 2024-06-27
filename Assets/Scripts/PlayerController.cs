
using UnityEngine;
using UnityEngine.InputSystem;

namespace HayWay.Runtime.Components
{
    public class PlayerController : MonoBehaviour
    {

        [SerializeField] private StageController m_stage;
        [SerializeField] private float m_moveSpeed;
        [SerializeField] private float m_changeLaneSpeed;
        [SerializeField] private float m_jumpHeight = 2f;
        [SerializeField] private float m_jumpDuration = 1f;

        private PlayerInputActions m_inputActions;
        private Rigidbody m_rigidBody;

        public bool IsJumping => isJumping;
        public bool IsGrounded => !isJumping;
        public Vector3 Position => transform.position;
        public float LanePosition => m_stage.GetLane(currentLaneIndex);

        private bool isJumping = false;
        private float jumpStartedTime = 0;
        private int currentLaneIndex = 0;
        private Vector3 startPlayerPosition;
        private Vector3 position = Vector3.zero;
        private Vector3 jumStartedPosition = Vector2.zero;

        private void Awake()
        {
            m_inputActions = new PlayerInputActions();
            m_rigidBody = gameObject.GetComponent<Rigidbody>();
            m_rigidBody.position = Vector3.zero;
            startPlayerPosition = Position;
        }
        private void OnEnable()
        {
            m_inputActions.Player.Enable();
            m_inputActions.Player.MoveLeft.started += OnMoveLeftInput;
            m_inputActions.Player.MoveRight.started += OnMoveRightInput;
            m_inputActions.Player.Jump.started += OnJumpInput;
        }
        private void OnDisable()
        {
            m_inputActions.Player.Disable();
            m_inputActions.Player.MoveLeft.started -= OnMoveLeftInput;
            m_inputActions.Player.MoveRight.started -= OnMoveRightInput;
            m_inputActions.Player.Jump.started -= OnJumpInput;
        }
        private void Update()
        {
            if (isJumping)
            {
                UpdateJump();
            }

            m_stage.UpdateStage(this);
        }
        private void FixedUpdate()
        {
            position.x = Mathf.Lerp(position.x, LanePosition, m_changeLaneSpeed * Time.fixedDeltaTime);
            position.z += m_moveSpeed * Time.fixedDeltaTime;
            m_rigidBody.MovePosition(position);
        }

        private void OnMoveLeftInput(InputAction.CallbackContext context)
        {
            if (currentLaneIndex <= -1) { return; }
            currentLaneIndex--;
        }
        private void OnMoveRightInput(InputAction.CallbackContext context)
        {
            if (currentLaneIndex >= 1) { return; }
            currentLaneIndex++;
        }
        private void OnJumpInput(InputAction.CallbackContext context)
        {
            if (isJumping) { return; }
            StartJump();
        }

        void StartJump()
        {
            jumpStartedTime = Time.time;
            isJumping = true;
            jumStartedPosition = transform.position;
        }
        void UpdateJump()
        {
            float elapsedTime = Time.time - jumpStartedTime;
            float t = elapsedTime / m_jumpDuration;
            if (t >= 1f)
            {
                isJumping = false;
                position.y = jumStartedPosition.y;
            }
            else
            {
                float height = m_jumpHeight * Mathf.Sin(t * Mathf.PI);
                position.y = jumStartedPosition.y + height;
            }
        }

        public void RefreshPosition()
        {

            position.z = transform.position.z;
         
        }

    }

}