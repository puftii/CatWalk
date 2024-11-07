using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using UnityEngine.Animations.Rigging;
using TMPro;
using System;
using Cinemachine;
using SmallHedge.SoundManager;


#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        public AudioClip[] WaterSplashAudioClips;
        [Range(0, 1)] public float WaterSplashAudioVolume = 0.5f;
        public GameObject WaterSplash;

        public GameObject PoofEffect;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;
        public float AttackTimeout = 0.15f;
        public float AttackSpeed = 1f;
        public float AttackForwardSpeed = 3f;
        public GameObject spine;
        public LayerMask enemyLayers;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        public bool Watered = false;
        public bool Attack = false;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        public LayerMask WaterLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        public CinemachineVirtualCamera CinemachineCamera;

        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player

        private bool _block;

        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;
        private float _attackTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDAttack;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;
        private PlayerCombat _playerCombat;
        private Rigidbody[] _bodies;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void OnEnable()
        {
            EventManager.PlayerDied += Die;
        }

        private void OnDisable()
        {
            EventManager.PlayerDied -= Die;
        }

        void OnDestroy()
        {
            EventManager.PlayerDied -= Die;
        }
        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Start()
        {
            EventManager.PlayerDied += Die;
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
            try
            {
                _bodies = GetComponentsInChildren<Rigidbody>();
                _animator = GetComponent<Animator>();
                _playerCombat = GetComponent<PlayerCombat>();
            }
            catch (ArgumentException e)
            {
                Debug.Log($"Ошибка: {e.Message}");
            }

            foreach (Rigidbody body in _bodies)
            {
                if (body.gameObject.layer != 8)
                    body.isKinematic = true;
            }
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            AssignAnimationIDs();

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
            _attackTimeoutDelta = AttackTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);
            if (_animator == null)
            {
                _animator = GetComponentInChildren<Animator>();
            }
            GoDown();
            JumpAndGravity();
            GroundedCheck();
            Move();
            Fight();
            Block();
            //TimeScaler();
            ChangeCamera();

            /*if(_input.tactical1)
            {
                _animator.SetTrigger("Tactical1");
                _input.tactical1 = false;
            }*/
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDAttack = Animator.StringToHash("Attack");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);
            Watered = Physics.CheckSphere(spherePosition, GroundedRadius, WaterLayers,
                QueryTriggerInteraction.Ignore);
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
                _animator.SetBool("Watered", Watered);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
            {
                //Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Cinemachine will follow this target
            CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {


            // set target speed based on move speed, sprint speed and if sprint is pressed
            bool sprinting = _input.sprint && (_playerCombat.CurrentStamina > 1f);
            float targetSpeed = sprinting ? SprintSpeed : MoveSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (targetSpeed > MoveSpeed && _playerCombat.CurrentStamina > 1f && inputMagnitude > 0.3f)
            {
                _playerCombat.ChangeStamina(-Time.deltaTime * 30f);
                EventManager.OnPlayerWeaponHolstered(true);
            }

            //float inputMagnitude = _input.move.magnitude;
            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }
            //_animationBlend = Mathf.Lerp(_animationBlend, _speed, Time.deltaTime * SpeedChangeRate);
            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            if (_animator.GetCurrentAnimatorStateInfo(0).IsTag("Attack"))
            {
                _controller.Move(targetDirection.normalized * (_speed * AttackSpeed * Time.deltaTime) +
                         new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            }
            else
            {
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }


            // update animator if using character
            if (_hasAnimator)
            {

                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }
        private void GoDown()
        {
            /*if (GetComponent<Animator>().enabled == false)
            {
                transform.position = spine.transform.position;
            }
            freeze = !freeze;
            foreach (Rigidbody body in _bodies)
            {
                body.isKinematic = freeze;
                if (!freeze && body.gameObject.layer != 8)
                {
                    body.velocity = _controller.velocity * 1;
                }
            }

            _controller.enabled = !_controller.enabled;


            GetComponent<Animator>().enabled = !GetComponent<Animator>().enabled;

            Debug.Log("Pizda");*/
            /*if (!GetComponent<Animator>().isActiveAndEnabled && spine != null)
            {
                _input.ragdoll = false;
                return;
            }*/
        }

        private void Block()
        {
             _block = _input.block;
            _animator.SetBool("Block", _block);
        }
        private void TimeScaler()
        {
            if (_input.tactical1)
            {
                //katanRig.GetComponent<Rig>().weight = 1f;

                /*if(Time.timeScale >= 0.7f)
                {
                    Time.timeScale = 0.1f;
                } else
                {
                    Time.timeScale = 1f;
                }*/
            }
            _input.tactical1 = false;
        }
        private void Fight()
        {
            if (_input.attack && _speed <= MoveSpeed)
            {
                EventManager.OnPlayerWeaponHolstered(false);
                _attackTimeoutDelta = AttackTimeout;
                _animator.SetBool(_animIDAttack, true);

            }
            if (_attackTimeoutDelta >= 0.00f)
            {
                _attackTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDAttack, false);
                }
            }
            /*if (!Attack)
            {
                _attackTimeoutDelta = AttackTimeout;
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDAttack, false);
                }
                if (_input.attack)
                {

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDAttack, true);
                    }
                }
                //Attack
                if(_input.attack)
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }

                }

                //Timeout
                if(_attackTimeoutDelta >= 0)
                {

                }
            } else
            {


            }*/

        }

        void AttackDash(float speed)
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            _controller.Move(forward * AttackForwardSpeed * speed * Time.deltaTime);
        }

        public void Dash()
        {
            Vector3 forward = transform.TransformDirection(Vector3.forward);
            _controller.SimpleMove(forward * AttackForwardSpeed * 15);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    //EventManager.OnPlayerWeaponHolstered(true);
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private void Die()
        {
            if (_animator != null)
            {
                _animator.enabled = false;
                transform.position = spine.transform.position;
            }
            foreach (Rigidbody body in _bodies)
            {
                body.isKinematic = false;
                body.linearVelocity = _controller.velocity * 1;
            }

            _controller.enabled = false;
            this.enabled = false;
            Debug.Log("Сдох");
        }

        private void ChangeCamera()
        {
            if (_input.lockTarget)
            {
                Debug.Log("Popa");
                if (CinemachineCamera.LookAt != null)
                {
                    CinemachineCamera.LookAt = null;

                }
                else
                {
                    Collider[] hitEnemies = Physics.OverlapSphere(transform.position, 15f, enemyLayers);
                    
                    if (hitEnemies.Length > 0)
                    {
                        //TPToNearestEnemy(hitEnemies[0].gameObject);
                        /*Vector3 centerHint = new Vector3((hitEnemies[0].transform.position.x + transform.position.x) / 2, (hitEnemies[0].transform.position.y + transform.position.y) / 2, (hitEnemies[0].transform.position.z + transform.position.z) / 2);
                        GameObject center = new GameObject("centerHint");
                        center.transform.position = centerHint;
                        CinemachineCamera.LookAt = hitEnemies[0].transform;*/
                        //CameraLookingAtEnemy(hitEnemies[0].gameObject, center);
                    }
                }

                _input.lockTarget = false;
            }
        }

        IEnumerator CameraLookingAtEnemy(GameObject enemy, GameObject center)
        {
            while (CinemachineCamera.LookAt != null && enemy != null)
            {
                Vector3 centerHint = new Vector3((enemy.transform.position.x + transform.position.x) / 2, (enemy.transform.position.y + transform.position.y) / 2, (enemy.transform.position.z + transform.position.z) / 2);
                center.transform.position = centerHint;
                yield return new WaitForSeconds(0.02f);
            }
            CinemachineCamera.LookAt = null;
            Destroy(center);
            


        }

        public void TPToNearestEnemy(GameObject enemy) 
        {
            GameObject poof = Instantiate(PoofEffect, transform.position, Quaternion.Euler(Vector3.zero));
            transform.position = enemy.transform.position + (enemy.transform.position - transform.position)/ Vector3.Distance(enemy.transform.position, transform.position);
            transform.position = new Vector3(transform.position.x, enemy.transform.position.y, transform.position.z);
            transform.LookAt(enemy.transform.position);
            //transform.rotation = Quaternion.Euler(0, transform.rotation.y, 0);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                SoundManager.PlaySound(SoundType.FOOTSTEP);
            }
        }

        private void OnWaterstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                SoundManager.PlaySound(SoundType.WATERSPLASH);
                /*if (WaterSplashAudioClips.Length > 0)
                {
                    var index = UnityEngine.Random.Range(0, WaterSplashAudioClips.Length);
                    AudioSource.PlayClipAtPoint(WaterSplashAudioClips[index], 
                        transform.TransformPoint(_controller.center), WaterSplashAudioVolume);
                    var position = transform.position;
                    position.y += 1.1f;
                    var rotation = transform.rotation;
                    rotation.z = 90f;
                    Instantiate(WaterSplash, position, WaterSplash.transform.rotation);
                }*/
            }
        }
        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                SoundManager.PlaySound(SoundType.LANDING);
                //AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

    }
}