using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NetworkBehaviours
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField, Tooltip("Used to take main player values (speed, amount of bombs, health and ex.)")]
        private BaseBomberParameters BomberParameters;
        
        [SerializeField] 
        private float GravityAcceleration = -9.8f;
        [SerializeField]
        private bool IsGravityOn = true;
        [SerializeField]
        private float GravityMultiplyer = 0.0001f;

        
        private CharacterController _controller;
        private PlayerMainControls _controls;
        private InputAction MoveAction;

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
            if (_controls == null)
            {
                _controls = new PlayerMainControls();
            }

            if (!_controller)
            {
                _controller = GetComponent<CharacterController>();
            }
            _velocity = 0;
            _controls.PlayerMainActionMaps.Enable();
            
            MoveAction = _controls.PlayerMainActionMaps.Move;

            _controller.enabled = true;
        }

        private void FixedUpdate()
        {
            if (!canMove) return;
            
            if (IsOwner)
            {
                if (MoveAction.IsInProgress())
                {
                    ApplyMovement();
                }

                if (IsGravityOn)
                {
                    ApplyGravity();
                }
                
                MoveController();
            }
        }

        private void MoveController()
        {
            _controller.Move(_moveDirection);
            _moveDirection = Vector3.zero;
        }

        private void ApplyMovement()
        {
            Vector2 inputValue = MoveAction.ReadValue<Vector2>();
            _moveDirection += new Vector3(inputValue.x, 0, inputValue.y) * BomberParameters.SpeedMultiplier / CONSTANTSPEEDDEVIDER;
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
    }
}
