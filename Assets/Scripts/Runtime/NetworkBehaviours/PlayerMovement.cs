using System;
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

        
        private CharacterController _controller;
        private PlayerMainControls _controls;
        private InputAction MoveAction;

        private bool canMove;

        private const float CONSTANTSPEEDDEVIDER = 1.25f;

        void Start()
        {
            _controller = GetComponent<CharacterController>();
            canMove = true;
        }

        public override void OnNetworkSpawn()
        {
            if (_controls == null)
            {
                _controls = new PlayerMainControls();
            }
            
            _controls.PlayerMainActionMaps.Enable();
            
            MoveAction = _controls.PlayerMainActionMaps.Move;
        }

        private void FixedUpdate()
        {
            if (MoveAction.IsInProgress() && IsOwner && canMove)
            {
                OnMove(MoveAction.ReadValue<Vector2>(), BomberParameters.SpeedMultiplier);
            }
        }

        private void OnMove(Vector2 inputValue, float speedMultiplier)
        {
            _controller.Move(new Vector3(inputValue.x, 0, inputValue.y) * speedMultiplier / CONSTANTSPEEDDEVIDER);
        }

        public void SetAbilityToMove(bool canIt)
        {
            canMove = canIt;
        }

        [ClientRpc]
        public void SetAbilityToMoveClientRpc(bool canIt)
        {
            canMove = canIt;
        }
    }
}
