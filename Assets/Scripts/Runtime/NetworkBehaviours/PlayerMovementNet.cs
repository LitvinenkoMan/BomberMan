using Interfaces;
using ScriptableObjects;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.PlayerLoop;

namespace Runtime.NetworkBehaviours
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(NetworkObject))]
    public class PlayerMovementNet : NetworkBehaviour, IMovable
    {
        [SerializeField] 
        private float GravityAcceleration = -9.8f;
        [SerializeField]
        private bool IsGravityOn = true;
        [SerializeField]
        private float GravityMultiplyer = 0.0001f;

        
        private CharacterController _controller;

        private Vector3 _moveDirection;
        private float _velocity;
        private bool _canMove;

        private const float CONSTANTSPEEDDEVIDER = 1.25f;

        void Start()
        {
            Initialize();
        }

        public override void OnNetworkSpawn()
        {
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

        public void Initialize()
        {
            _canMove = true;
        }

        public void Move(Vector3 direction)
        {
            _moveDirection = direction / CONSTANTSPEEDDEVIDER;
        }

        public void SetAbilityToMove(bool canIt)
        {
            _canMove = canIt;
        }

        private void MoveController()
        {
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

        [ClientRpc]
        public void SetAbilityToMoveClientRpc(bool canIt)
        {
            SetAbilityToMove(canIt);
        }
    }
}
