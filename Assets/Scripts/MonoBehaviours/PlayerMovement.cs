using System;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace MonoBehaviours
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField, Tooltip("Used to take main player values (speed, amount of bombs, health and ex.)")]
        private BasePlayerParameters PlayerParameters;

        private CharacterController _controller;
        
        private PlayerMainControls _controls;
        
        
        // InputActions 
        private InputAction MoveAction;
        
        
        
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
            PlayerParameters.ResetValues();
            
            MoveAction = _controls.PlayerMainActionMaps.Move;
        }
        
        void Update()
        {
            // TODO: remake it to listen events
            if (MoveAction.IsInProgress())
            {
                OnMove(MoveAction.ReadValue<Vector2>());
            }
        }

        public void OnMove(Vector2 inputValue)
        {
            _controller.Move(new Vector3(inputValue.x, 0, inputValue.y) * (PlayerParameters.SpeedMultiplier * Time.deltaTime));
        }

        public void OnPlaceBomb(int inputValue)
        {
            Debug.Log("Placing bomb");
        }
    }
}
