
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

namespace HayWay.Runtime.Components
{
    [RequireComponent(typeof(Collider))]
    public class PlayerController : MonoBehaviour
    {
        public static event Action<PlayerController> OnInvencibleChanged;

        [SerializeField] private StageController m_stage;
        [SerializeField] private float m_moveSpeed;
        [SerializeField] private float m_changeLaneSpeed;
        [SerializeField] private float m_jumpHeight = 2f;
        [SerializeField] private float m_jumpDuration = 1f;

        private PlayerInputActions m_inputActions;
        private Rigidbody m_rigidBody;

        public bool IsDead => isDead;
        public bool IsInvencible => isInvencible;
        public float InvencibleAmount => invencibleElapsedTime / invencibleTime;

        public bool IsJumping => isJumping;
        public bool IsGrounded => !isJumping;
        public Vector3 Position => transform.position;
        public float LanePosition => m_stage.GetLane(currentLaneIndex);

        private bool isJumping = false;
        private bool isDead = false;
        private bool isInvencible = false;

        private float jumpStartedTime = 0;
        private int currentLaneIndex = 0;
        private Vector3 startPlayerPosition;
        private Vector3 position = Vector3.zero;
        private Vector3 jumStartedPosition = Vector2.zero;
        private float invencibleElapsedTime = 0;
        private float invencibleTime = 0;


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
            if (isDead)
            {
                return;
            }

            //Check Jump
            if (isJumping)
            {
                UpdateJump();
            }

            //Check Invencible
            if (isInvencible)
            {
                invencibleElapsedTime += 1 * Time.deltaTime;
                if (invencibleElapsedTime > invencibleTime)
                {
                    invencibleElapsedTime = 0;
                    invencibleTime = 0;
                    isInvencible = false;
                }
            }

            //Update Stage
            m_stage.UpdateStage(this);
        }
        private void FixedUpdate()
        {
            if (isDead)
            {
                return;
            }
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

        private void StartJump()
        {
            jumpStartedTime = Time.time;
            isJumping = true;
            jumStartedPosition = transform.position;
        }
        private void UpdateJump()
        {
            float elapsedTime = Time.time - jumpStartedTime;
            float t = elapsedTime / m_jumpDuration;
            if (t >= 0.99f)
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

        public void SetInvencible(float seconds)
        {
            if (isDead) return;

            invencibleElapsedTime = 0;
            invencibleTime = seconds;
            isInvencible = true;
            OnInvencibleChanged?.Invoke(this);
        }

        public void SetDead(bool dead)
        {
            invencibleElapsedTime = 0;
            invencibleTime = 0;
            isDead = dead;
        }
    }

}