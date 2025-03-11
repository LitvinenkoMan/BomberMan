using Runtime.MonoBehaviours;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetworkBehaviours
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerMovement : NetworkBehaviour, InputActions.IPlayerMapActions
    {
        [SerializeField, Tooltip("Used to take main player values (speed, amount of bombs, health and ex.)")]
        private BaseBomberParameters BomberParameters;
        [SerializeField, Tooltip("Used for Animations")]
        private PawnAnimationHandler PawnAnimation;
        
        [SerializeField] 
        private float GravityAcceleration = -9.8f;
        [SerializeField]
        private bool IsGravityOn = true;
        [SerializeField]
        private float GravityMultiplyer = 0.0001f;

        
        private CharacterController _controller;
        private InputActions _input;

        private Vector3 _moveDirection;
        private float _velocity;
        private bool canMove;

        private const float CONSTANTSPEEDDEVIDER = 1.25f;

        void Start()
        {
            canMove = true;
        }

        public override void OnNetworkSpawn()
        {
            if (_input == null)
            {
                _input = new InputActions();
            }
            _input.PlayerMap.AddCallbacks(this);
            _input.Enable();

            if (!_controller)
            {
                _controller = GetComponent<CharacterController>();
            }
            _velocity = 0;

            _controller.enabled = true;
            _moveDirection = Vector3.zero;
        }

        private void FixedUpdate()
        {
            if (IsOwner)
            {
                if (IsGravityOn)
                {
                    ApplyGravity();
                }
                
                MoveController();
            }
        }

        private void MoveController()
        {
            if (_moveDirection.x != 0 || _moveDirection.z != 0)
            {
                PawnAnimation.PlayMoveAnimation();
            }
            else PawnAnimation.PlayIdleAnimation();
            
            _controller.Move(_moveDirection);
        }
        
        private void ApplyGravity()
        {
            if (!_controller.isGrounded)
            {
                _velocity += GravityAcceleration * GravityMultiplyer;
                _moveDirection += new Vector3(0, _velocity, 0); 
            }
            else _velocity = 0;
        }

        public void SetAbilityToMove(bool canIt)
        {
            canMove = canIt;
        }

        [ClientRpc]
        public void SetAbilityToMoveClientRpc(bool canIt)
        {
            SetAbilityToMove(canIt);
        }

        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 inputValue = context.ReadValue<Vector2>();
            _moveDirection = new Vector3(inputValue.x, 0, inputValue.y) * BomberParameters.SpeedMultiplier /
                              CONSTANTSPEEDDEVIDER;
            if (context.performed)
            {
                transform.rotation = Quaternion.LookRotation(new Vector3(_moveDirection.x, 0, _moveDirection.z));       //TODO: Move this Out from Movement script, this is not SOLID (Snake)
            }
        }

        public void OnPlaceBomb(InputAction.CallbackContext context)
        {
            //not needed
        }

        public void OnQuit(InputAction.CallbackContext context)
        {
            //not needed
        }
    }
}
