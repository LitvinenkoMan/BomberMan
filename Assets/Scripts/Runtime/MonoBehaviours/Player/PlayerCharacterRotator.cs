using UnityEngine;

namespace Runtime.MonoBehaviours.Player
{
    public class PlayerCharacterRotator : MonoBehaviour
    {
        [SerializeField] private GameObject visuals;
        
        
        private Vector3 _moveDirection;
        private Vector3 _previousPosition;
        
        void Start()
        {
            _previousPosition = Vector3.zero;
            _moveDirection = Vector3.zero;
        }

        void Update()
        {
            if (_previousPosition == transform.position) return;
            
            
            _moveDirection =  transform.position - _previousPosition;
            
            visuals.transform.rotation = Quaternion.LookRotation(new Vector3(_moveDirection.x, 0, _moveDirection.z));
                
            _previousPosition = transform.position;
        }
    }
}
