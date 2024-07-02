using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MonoBehaviours
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private float PlayerMovementMultiplier = 1;

        private CharacterController _controller;
        
        private PlayerMainControls _controls;
        
        
        // InputActions 
        private InputAction MoveAction;
        private InputAction PlaceBombAction;
        
        
        
        void Start()
        {
            _controller = GetComponent<CharacterController>();
        }

        private void OnEnable()
        {
            if (_controls == null)
            {
                _controls = new PlayerMainControls();
                // Tell the "gameplay" action map that we want to get told about
                // when actions get triggered.
            }
            
            _controls.PlayerMainActionMaps.Enable();

            MoveAction = _controls.PlayerMainActionMaps.Move;
            PlaceBombAction = _controls.PlayerMainActionMaps.PlaceBomb;
        }
        
        void Update()
        {
            // TODO: remake it to listen events
            if (MoveAction.IsInProgress())
            {
                OnMove(MoveAction.ReadValue<Vector2>());
            }

            if (PlaceBombAction.WasCompletedThisFrame())
            {
                OnPlaceBomb(PlaceBombAction.ReadValue<int>());
            }
        }

        public void OnMove(Vector2 inputValue)
        {
            _controller.Move(new Vector3(inputValue.x, 0, inputValue.y) * (PlayerMovementMultiplier * Time.deltaTime));
        }

        public void OnPlaceBomb(int inputValue)
        {
            Debug.Log("Placing bomb");
        }
    }
}
