
using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace HayWay.Runtime.Components
{
    /// <summary>
    /// This component controls all of the player's actions, such as constant movement and jumping. 
    /// - This includes the alive or dead state (<see cref="PlayerController.IsDead"/>). 
    /// Use this state to create screen control, game over, etc.
    /// - This includes also the player's invincible status (<see cref="PlayerController.IsInvencible"/>). 
    /// Use this state to create life control or anything else needed.</para>
    /// 
    /// Important Notes:
    /// - The player foward speed (<see cref="m_StartMoveSpeed"/>), is progressively increased evey time, 
    /// you must control the time necessary for the player to reach the maximum speed allowed. See <see cref="m_MaxSpeedReachTime"/>
    /// 
    /// - Use the <see cref="RefreshPosition"/> funciton, if you teleported the player. 
    /// It will guarantee that the next foward position for foward velocity be the current player position.
    /// - By default, there is no maximum number of lanes that are supplied by the <see cref="StageController.GetLane(int)"/>. 
    /// It accepts any value in the negative to positive range and calculates the lane position based on a pre-established fixed distance.
    /// Therefore, I am controlling the maximum number of lanes right here in the player. 
    /// I use the range -1, 0 and 1, to work in 3 lanes, being: 
    /// > 1 = Left,
    /// > 0 = Middle and 
    /// > 1 = Direct.
    /// 
    /// - The player's Jump is made using a simple sinusoid formula, this is easy to implement at first, 
    /// but needs some love to work with Double Jumps or more. 
    /// A different algorithm needs to be worked on for more complex jumps
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class PlayerController : MonoBehaviour
    {
        public static event Action<PlayerController> OnInvencibleChanged;
        public static event Action<PlayerController> OnDeadChanged;

        [SerializeField] private StageController m_stage;

        [Header("Player Movement")]
        [Tooltip("My initial foward movement speed.")]
        [SerializeField, Range(0, 15)]
        private float m_StartMoveSpeed = 5;

        [Tooltip("Maximum allowed of foward movement speed. The 15 limit is because more of that is very fast")]
        [SerializeField, Range(0, 15)]
        private float m_MaxMoveSpeed = 10;

        [Tooltip("Transition speed between the lanes. Consider this as the speed of horizontal movement (left, right).\n" +
            "- You can take a look in StageController to change the lanes distance")]
        [SerializeField]
        private float m_StartChangeLaneSpeed = 15;
        [Tooltip("This is the time in seconds that the player will take for the reach the maximum foward speed limit.\n" +
            "Remember that the player's speed increases gradually and will reach its maximum limit in this time.")]
        [SerializeField]
        private float m_MaxSpeedReachTime = 120;

        [Header("Player Jump")]
        [Tooltip("Maximum initial jump height.")]
        [SerializeField]
        private float m_StartJumpHeight = 2.5f;
        [Tooltip("This is the time the player's jump will take to complete its sinusoidal movement.")]
        [SerializeField]
        private float m_StartJumpDuration = 0.75f;

        private PlayerInputActions m_inputActions;
        private Rigidbody m_rigidBody;

        /// <summary>
        /// Is Current Dead state of the player ON?
        /// </summary>
        public bool IsDead => isDead;
        /// <summary>
        /// Is Currently Invencible? Use <see cref="InvencibleAmount"/> to know how many time the player will keep this stats
        /// </summary>
        public bool IsInvencible => isInvencible;
        /// <summary>
        /// How many time amount the player will keep this stats if <see cref="IsInvencible"/> true? It work with 0 to 1. 
        /// Use it to calculate the % of the invencible duration.
        /// </summary>
        public float InvencibleAmount => invencibleElapsedTime / invencibleTime;
        /// <summary>
        /// True if the player is jumping currently
        /// </summary>
        public bool IsJumping => isJumping;
        /// <summary>
        /// True if the player is not Jumping currently. 
        /// </summary>
        public bool IsGrounded => !isJumping;
        /// <summary>
        /// Current <see cref="Transform.position"/> this.
        /// </summary>
        public Vector3 Position => transform.position;
        /// <summary>
        /// My Current lane position. You can use <see cref="CurrentLaneIndex"/> to know how is the lane in use
        /// </summary>
        public float CurrentLanePosition => m_stage.GetLane(currentLaneIndex);
        /// <summary>
        /// Current lane index to <see cref="CurrentLanePosition"/>
        /// </summary>
        public int CurrentLaneIndex => currentLaneIndex;
        /// <summary>
        /// Current Foward speed of the player. You can see the <see cref="m_MaxSpeedReachTime"/> to know how many time the player need to 
        /// reach max foward speed. See also <see cref="CurentTraveledDistance"/> to create own math to work with this.
        /// </summary>
        public float CurrentMoveSpeed => currentMoveSpeed;
        public float CurrentChangeLaneSpeed => m_StartChangeLaneSpeed + CurrentMoveSpeedFactor;
        public float CurrentJumpHeight => m_StartJumpHeight;
        public float CurrentJumpDuration => Mathf.Clamp(m_StartJumpDuration / CurrentMoveSpeedFactor, 0.33f, m_StartJumpDuration);
        public float CurentTraveledDistance => travelledDistance;
        public float CurrentMoveSpeedFactor => CurrentMoveSpeed / m_StartMoveSpeed;
       
        private bool isJumping = false;
        private bool isDead = false;
        private bool isInvencible = false;

        private float jumpStartedTime = 0;
        private int currentLaneIndex = 0; //chanded between -1 and 1
        private Vector3 startPlayerPosition;
        private Vector3 position = Vector3.zero;
        private Vector3 jumpStartedPosition = Vector2.zero;
        private float invencibleElapsedTime = 0;
        private float invencibleTime = 0;
        private float travelledDistance = 0;
        [SerializeField, Attributes.ReadOnly]
        private float currentMoveSpeed;
        [SerializeField, Attributes.ReadOnly]
        private float debugJumpDuration = 0;

        private void Awake()
        {
            transform.SetParent(null);
            m_inputActions = new PlayerInputActions();
            m_rigidBody = gameObject.GetComponent<Rigidbody>();
            m_rigidBody.position = Vector3.zero;
            startPlayerPosition = Position;
            currentMoveSpeed = m_StartMoveSpeed;
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
        private void OnDestroy()
        {
            OnInvencibleChanged = null;
            OnDeadChanged = null;
        }
        private void Update()
        {
            debugJumpDuration = CurrentJumpDuration;
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

            //Update traveled distance
            travelledDistance += Time.deltaTime;

            //Update current player speed based in traveled distance and in time to reach max velocity
            currentMoveSpeed = Mathf.Lerp(m_StartMoveSpeed, m_MaxMoveSpeed, travelledDistance / m_MaxSpeedReachTime);
            currentMoveSpeed = Mathf.Min(currentMoveSpeed, m_MaxMoveSpeed);

        }
        private void FixedUpdate()
        {
            if (isDead)
            {
                return;
            }
            position.x = Mathf.Lerp(position.x, CurrentLanePosition, CurrentChangeLaneSpeed * Time.fixedDeltaTime);
            position.z += CurrentMoveSpeed * Time.fixedDeltaTime;
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
            if (isJumping)
            {
                return;
            }
            StartJump();
        }

        private void StartJump()
        {

            jumpStartedTime = Time.time;
            isJumping = true;
            jumpStartedPosition = transform.position;


        }
        private void UpdateJump()
        {
            float elapsedTime = Time.time - jumpStartedTime;
            float t = elapsedTime / CurrentJumpDuration;

            if (t >= 1.0f)
            {

                isJumping = false;
                position.y = jumpStartedPosition.y;

            }
            else
            {
                float height = CurrentJumpHeight * Mathf.Sin(t * Mathf.PI);
                position.y = jumpStartedPosition.y + height;
            }
        }

        /// <summary>
        /// This function wil guarantee that next foward position be the current player possition.
        /// Use this if you teleported the player to some position outside the standard movement cycle.
        /// </summary>
        public void RefreshPosition()
        {
            position.z = transform.position.z;
        }
        /// <summary>
        /// Turn the player invenciby any time. You can use <see cref="IsInvencible"/> and <see cref="InvencibleAmount"/> and <see cref="OnInvencibleChanged"/>,
        /// to work with this.
        /// </summary>
        /// <param name="seconds">Time that player will invencible</param>
        public void SetInvencible(float seconds)
        {
            if (isDead) return;

            invencibleElapsedTime = 0;
            invencibleTime = seconds;
            isInvencible = true;
            OnInvencibleChanged?.Invoke(this);
        }
        /// <summary>
        /// Turn Dead or alive the player. Use the <see cref="IsDead"/> and <see cref="OnDeadChanged"/> events to work with this
        /// </summary>
        /// <param name="dead"></param>
        public void SetDead(bool dead)
        {
            if (dead == isDead) { return; }

            invencibleElapsedTime = 0;
            invencibleTime = 0;
            isDead = dead;
            OnDeadChanged?.Invoke(this);
        }
    }

}