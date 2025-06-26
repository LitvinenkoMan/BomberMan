using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class VisualsRotator : MonoBehaviour
    {
        [SerializeField] 
        private CharacterController characterController;
        [SerializeField] 
        private Transform VisualsTransform;
        [SerializeField] 
        private float RotationTime = 0.1f;

        
        private CancellationTokenSource _cts;
        private Quaternion _endRotation;
        private Quaternion _startRotation;
        private float timer = 0;


        private void Update()
        {
            var direction = characterController.velocity;
            direction = direction.normalized;
            if (direction != Vector3.zero)
            {
                _startRotation = VisualsTransform.rotation;
                _endRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            }
            
            if (timer < 1 && VisualsTransform.rotation != _endRotation)
            {
                timer += Time.deltaTime / RotationTime;
                if (timer > 0.95f)
                {
                    timer = 1;
                }
                VisualsTransform.rotation = Quaternion.Lerp(_startRotation, _endRotation, timer);
            }
            else 
            {
                timer = 0;
            }
        }

        private async UniTask RotateByTime(Quaternion endRotation, float timeInSec, CancellationTokenSource cts)
        {
            if (!VisualsTransform) return;
            
            cts?.Cancel();
            cts = new CancellationTokenSource();
            
            if (VisualsTransform.rotation == endRotation) return;

            var startRotation = VisualsTransform.rotation;
            var currentRotation = startRotation;
            while (true)               
            {
                timer += Time.deltaTime / timeInSec;
                currentRotation = Quaternion.Lerp(startRotation, endRotation, timer);
                if (VisualsTransform)
                {
                    VisualsTransform.rotation = currentRotation;
                }
                else return;

                if (cts.IsCancellationRequested) return;
                await UniTask.WaitForEndOfFrame();
                if (currentRotation == endRotation) return;
            }
        }
    }
}





/*using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Runtime.MonoBehaviours
{
    public class VisualsRotator : MonoBehaviour, InputActions.IPlayerMapActions
    {
        [SerializeField] 
        private Transform VisualsTransform;
        [SerializeField] 
        private CharacterController characterController;
        [SerializeField] 
        private float RotationTime = 0.1f;

        
        private CancellationTokenSource _cts;
        private InputActions _input;
        private Quaternion _endRotation;
        private Quaternion _startRotation;
        private float timer = 0;
        private Vector3 direction;

        private void OnEnable()
        {
            _input = new InputActions();
            _input.PlayerMap.AddCallbacks(this);
            _input.Enable();
        }

        private void Update()
        {
            direction = new Vector3(characterController.velocity.x, 0, characterController.velocity.z);
            direction = direction.normalized;

            if (timer < 1 && direction != Vector3.zero)
            {
                timer += Time.deltaTime / RotationTime;
                VisualsTransform.rotation = Quaternion.Lerp(_startRotation, Quaternion.LookRotation(direction), timer);
            }
            else timer = 0;
        }

        private async UniTask RotateByTime(Quaternion endRotation, float timeInSec, CancellationTokenSource cts)
        {
            if (!VisualsTransform) return;
            
            cts?.Cancel();
            cts = new CancellationTokenSource();
            
            if (VisualsTransform.rotation == endRotation) return;
        
            var startRotation = VisualsTransform.rotation;
            var currentRotation = startRotation;
            while (true)               
            {
                timer += Time.deltaTime / timeInSec;
                currentRotation = Quaternion.Lerp(startRotation, endRotation, timer);
                if (VisualsTransform)
                {
                    VisualsTransform.rotation = currentRotation;
                }
                else return;
        
                if (cts.IsCancellationRequested) return;
                await UniTask.WaitForEndOfFrame();
                if (currentRotation == endRotation) return;
            }
        }

        public void OnMove(InputAction.CallbackContext context)
        {
           
            _startRotation = VisualsTransform.rotation;
        }

        public void OnPlaceBomb(InputAction.CallbackContext context)
        {
            //Not Needed
        }

        public void OnQuit(InputAction.CallbackContext context)
        {
            //Not Needed
        }
    }
}
*/
