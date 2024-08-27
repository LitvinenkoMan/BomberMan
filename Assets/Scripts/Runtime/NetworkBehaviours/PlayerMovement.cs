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
        

        void Start()
        {
            _controller = GetComponent<CharacterController>();
            BomberParameters.ResetValues();
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

        void Update()
        {
            // TODO: remake it to listen events
            if (MoveAction.IsInProgress() && IsOwner)
            {
                OnMoveRpc(MoveAction.ReadValue<Vector2>());
            }
        }

        [Rpc(SendTo.ClientsAndHost)]
        public void OnMoveRpc(Vector2 inputValue)
        {
            _controller.Move(new Vector3(inputValue.x, 0, inputValue.y) * (BomberParameters.SpeedMultiplier * Time.deltaTime));
        }
    }
}
