// Designed by Kinemation, 2022
// Purchased 1/18/2022
// Edited by Nate Bursch

using System.Collections.Generic;
using Demo.Scripts.Runtime;
using Kinemation.FPSFramework.Runtime.Core;
using UnityEngine;


    public class FPSController : MonoBehaviour
    {
        [Header("FPS Framework")]
        [SerializeField] private CoreAnimComponent coreAnimComponent;

        [Header("Video Settings")]
        [SerializeField] private string idk = "LOL";

        [Header("Movement Settings")]
        [SerializeField] private bool toggleCrouch = true;
        [SerializeField] private bool toggleSprint = true;
        [SerializeField] private bool toggleAim = true;
        [Header("Sensitivity Settings")]
        [SerializeField] private float lookSensitivity = 1.2f;
        [SerializeField] private float aimSensitivity = 1f;



        [Header("Keyboard Options")]
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
        [SerializeField] private KeyCode reloadKey = KeyCode.R;
        [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;




        [Header("Character Controls")]
        [SerializeField] private Transform cameraBone;
        [SerializeField] private float crouchHeight;


        [Header("Movement")] 
        [SerializeField] private bool shouldMove;
        [SerializeField] private float walkSpeed = 2f;
        [SerializeField] private float sprintSpeed = 4f;
        [SerializeField] private float crouchSpeed = 1f;
        [SerializeField] private float jumpPower = 1f;
        public float currentSpeed;


        [SerializeField] private CharacterController controller;
        [SerializeField] private Animator animator;

        [SerializeField] private List<Weapon> weapons;
        private RecoilAnimation _recoilAnimation;

        private Vector2 _playerInput;
        private Vector2 _smoothMoveInput;

        private int _index;

        private float _fireTimer = -1f;
        private int _bursts;
        public bool _crouching;
        public bool _sprinting;
        public bool _aiming;
        private bool _reloading;

        private float _lowerCapsuleOffset;
        private CharAnimData _charAnimData;
        private static readonly int Sprint = Animator.StringToHash("sprint");
        private static readonly int Crouch1 = Animator.StringToHash("crouch");
        private static readonly int Moving = Animator.StringToHash("moving");
        private static readonly int MoveX = Animator.StringToHash("moveX");
        private static readonly int MoveY = Animator.StringToHash("moveY");

        private float _gravity = -9.81f;
        [SerializeField] private float gravityMultiplier = 1.0f;
        private float _velocity;

        //axis to camera
        private Camera camera;

        private void Start()
        {
            currentSpeed = walkSpeed;
            Application.targetFrameRate = 120;
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            _lowerCapsuleOffset = controller.center.y - controller.height / 2f;

            coreAnimComponent = GetComponent<CoreAnimComponent>();

            animator = GetComponent<Animator>();
            _recoilAnimation = GetComponent<RecoilAnimation>();

            controller = GetComponent<CharacterController>();

            EquipWeapon();
        }
        
        private void EquipWeapon()
        {
            var gun = weapons[_index];
            
            _bursts = gun.burstAmount;
            _recoilAnimation.Init(gun.recoilData, gun.fireRate, gun.fireMode);
            coreAnimComponent.OnGunEquipped(gun.gunData);

            //animator.Play(gun.poseName);
            gun.gameObject.SetActive(true);
        }
        
        private void ChangeWeapon()
        {
            int newIndex = _index;
            newIndex++;
            if (newIndex > weapons.Count - 1)
            {
                newIndex = 0;
            }

            weapons[_index].gameObject.SetActive(false);
            _index = newIndex;
            
            EquipWeapon();
        }

        public void ToggleAim()
        {
            _aiming = !_aiming;

            if (_aiming)
            {
                _charAnimData.actionState = FPSActionState.Aiming;
            }
            else
            {
                _charAnimData.actionState = FPSActionState.None;
            }
            
            _recoilAnimation.isAiming = _aiming;
        }
        public void AimPressed()
        {
            _aiming = true;
            _charAnimData.actionState = FPSActionState.Aiming;
            _recoilAnimation.isAiming = _aiming;
        }
        public void AimReleased()
        {
            _aiming = false;
            _charAnimData.actionState = FPSActionState.None;
            _recoilAnimation.isAiming = _aiming;
        }
        
        public void ChangeScope()
        {
            coreAnimComponent.OnSightChanged(GetGun().GetScope());
        }

        private void Fire()
        {
            GetGun().OnFire();
            _recoilAnimation.Play();
        }

        private void OnFirePressed()
        {
            Fire();
            _bursts = GetGun().burstAmount - 1;
            _fireTimer = 0f;
        }

        private Weapon GetGun()
        {
            return weapons[_index];
        }

        private void OnFireReleased()
        {
            _recoilAnimation.Stop();
            _fireTimer = -1f;
        }

        private void SprintPressed()
        {
            _sprinting = true;
            if (_charAnimData.poseState == FPSPoseState.Crouching)
            {
                Uncrouch();
            }
            
            _charAnimData.movementState = FPSMovementState.Sprinting;
            _charAnimData.actionState = FPSActionState.None;

            currentSpeed = sprintSpeed;
            animator.SetBool(Sprint, true);
        }
        
        private void SprintReleased()
        {

            _sprinting = false;
            if (_charAnimData.poseState == FPSPoseState.Crouching)
            {
                return;
            }
            
            _charAnimData.movementState = FPSMovementState.Walking;

            currentSpeed = walkSpeed;
            animator.SetBool(Sprint, false);
        }
        private void ToggleCrouch()
        {
            _crouching = !_crouching;
            if (_crouching)
            {
                Crouch();
            }
            else
            {
                Uncrouch();
            }
        }
        private void Crouch()
        {
            _crouching = true;
            var height = controller.height;
            height *= crouchHeight;
            controller.height = height;
            controller.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
            currentSpeed = crouchSpeed;
            
            _charAnimData.poseState = FPSPoseState.Crouching;
            animator.SetBool(Crouch1, true);

            
        }

        private void Uncrouch()
        {
            _crouching = false;
            var height = controller.height;
            height /= crouchHeight;
            controller.height = height;
            controller.center = new Vector3(0f, _lowerCapsuleOffset + height / 2f, 0f);
            currentSpeed = walkSpeed;

            _charAnimData.poseState = FPSPoseState.Standing;
            animator.SetBool(Crouch1, false);
           

        }
        private void JumpPressed()
        {
            if (controller.isGrounded)
            {
                _velocity = jumpPower;
            }
        }

        private void ProcessActionInput()
        {
            _charAnimData.leanDirection = 0;

            if (toggleSprint)
            {
                if (Input.GetKeyDown(sprintKey) && Input.GetKey(KeyCode.W))
                {
                    if (_sprinting)
                    {
                        SprintReleased();
                    }
                    else
                    {
                        SprintPressed();
                    }
                }
                if (Input.GetKeyUp(KeyCode.W))
                {
                    SprintReleased();
                }
            }
            else
            {
                if (Input.GetKeyDown(sprintKey) && Input.GetKey(KeyCode.W))
                {
                    SprintPressed();
                }
                //cant sprint sideways or backwords
                if (Input.GetKeyUp(KeyCode.W))
                {
                    SprintReleased();
                }

                if (Input.GetKeyUp(sprintKey))
                {
                    SprintReleased();
                }
            }

            
            if (Input.GetKeyDown(KeyCode.F))
            {
                ChangeWeapon();
            }

            //jump
            if (Input.GetKeyDown(jumpKey))
            {
                JumpPressed();
            }

            if (_charAnimData.movementState == FPSMovementState.Sprinting)
            {
                return;
            }

            if (_charAnimData.actionState != FPSActionState.Ready)
            {
                if (Input.GetKey(KeyCode.Q))
                {
                    _charAnimData.leanDirection = 1;
                }
                else if (Input.GetKey(KeyCode.E))
                {
                    _charAnimData.leanDirection = -1;
                }

                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    OnFirePressed();
                }
            
                if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    OnFireReleased();
                }

                ///togle aim
                if (toggleAim)
                {
                    if (Input.GetKeyDown(KeyCode.Mouse1))
                    {

                        ToggleAim();
                    }
                }
                else
                {
                    //means we are not toggle aiming
                    if (Input.GetKey(KeyCode.Mouse1))
                    {

                        AimPressed();
                    }
                    if (Input.GetKeyUp(KeyCode.Mouse1))
                    {
                        AimReleased();
                    }
                }

            
                if (Input.GetKeyDown(KeyCode.V))
                {
                    ChangeScope();
                }
            
                if (Input.GetKeyDown(KeyCode.B) && _aiming)
                {
                    if (_charAnimData.actionState == FPSActionState.PointAiming)
                    {
                        _charAnimData.actionState = FPSActionState.Aiming;
                    }
                    else
                    {
                        _charAnimData.actionState = FPSActionState.PointAiming;
                    }
                }
            }
            if (toggleCrouch)
            {
                if (Input.GetKeyDown(crouchKey))
                {
                    ToggleCrouch();
                }
            }
            else
            {
                if (Input.GetKeyDown(crouchKey))
                {
                    Crouch();
                }
                if (Input.GetKeyUp(crouchKey))
                {
                    Uncrouch();
                }
            }


            if (Input.GetKeyDown(KeyCode.H))
            {
                if (_charAnimData.actionState == FPSActionState.Ready)
                {
                    _charAnimData.actionState = FPSActionState.None;
                }
                else
                {
                    _charAnimData.actionState = FPSActionState.Ready;
                    OnFireReleased();
                }
            }
        }

        private void ProcessLookInput()
        {
            float deltaMouseX = Input.GetAxis("Mouse X") * (_aiming ? aimSensitivity : lookSensitivity);
            float deltaMouseY = -Input.GetAxis("Mouse Y") * (_aiming ? aimSensitivity : lookSensitivity);

            _playerInput.x += deltaMouseX;
            _playerInput.y += deltaMouseY;
            
            _playerInput.x = Mathf.Clamp(_playerInput.x, -90f, 90f);
            _playerInput.y = Mathf.Clamp(_playerInput.y, -90f, 90f);

            _charAnimData.deltaAimInput = new Vector2(deltaMouseX, deltaMouseY);

            if (shouldMove)
            {
                transform.Rotate(Vector3.up * deltaMouseX);
            }
        }

        private void UpdateFiring()
        {
            if (_recoilAnimation.fireMode != FireMode.Semi && _fireTimer >= 60f / GetGun().fireRate)
            {
                Fire();

                if (_recoilAnimation.fireMode == FireMode.Burst)
                {
                    _bursts--;

                    if (_bursts == 0)
                    {
                        _fireTimer = -1f;
                        OnFireReleased();
                    }
                    else
                    {
                        _fireTimer = 0f;
                    }
                }
                else
                {
                    _fireTimer = 0f;
                }
            }

            if (_fireTimer >= 0f)
            {
                _fireTimer += Time.deltaTime;
            }

            _charAnimData.recoilAnim = new LocRot(_recoilAnimation.OutLoc,
                Quaternion.Euler(_recoilAnimation.OutRot));
        }

        private void UpdateMovement()
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            
            _charAnimData.moveInput = new Vector2(moveX, moveY);

            _smoothMoveInput.x = CoreToolkitLib.Glerp(_smoothMoveInput.x, moveX, 7f);
            _smoothMoveInput.y = CoreToolkitLib.Glerp(_smoothMoveInput.y, moveY, 7f);

            bool moving = Mathf.Abs(moveX) >= 0.4f || Mathf.Abs(moveY) >= 0.4f;
            
            animator.SetBool(Moving, moving);    
            animator.SetFloat(MoveX, _smoothMoveInput.x);
            animator.SetFloat(MoveY, _smoothMoveInput.y);
                
            Vector3 move = transform.right * moveX + transform.forward * moveY;
            //Debug.Log(move);

            //controller.Move(move * currentSpeed * Time.deltaTime);
            controller.Move(new Vector3(move.x*currentSpeed*Time.deltaTime,
                _velocity*walkSpeed*Time.deltaTime,
                move.z*currentSpeed*Time.deltaTime));

        }

        private void UpdateAnimValues()
        {
            coreAnimComponent.SetCharData(_charAnimData);
        }
        private void ApplyGravity()
        {
            if (controller.isGrounded && _velocity < 0.0f)
            {
                _velocity = -2.0f;
            }
            else
            {
                _velocity += _gravity * gravityMultiplier * Time.deltaTime;
            }

            //_direction.y = _velocity;
        }
        private void Update()
        {
            ApplyGravity();
            ProcessActionInput();
            ProcessLookInput();
            UpdateFiring();
            UpdateMovement();
            UpdateAnimValues();
        }

        private void UpdateCameraRotation()
        {
            var rootBone = coreAnimComponent.GetRootBone();
            cameraBone.rotation =
                rootBone.rotation * Quaternion.Euler(_playerInput.y, shouldMove ? 0f : _playerInput.x, 0f);
        }
        
        private void LateUpdate()
        {
            UpdateCameraRotation();
        }
    }
