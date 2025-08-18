using UnityEngine;

namespace Runtime.MonoBehaviours.Player
{
    public class PlayerCharacterRotator : MonoBehaviour
    {
        [SerializeField] private GameObject _visuals;
        [SerializeField] private float _rotationSpeed;
        
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

            Quaternion targetDirection = Quaternion.LookRotation(transform.position - _previousPosition);

            _previousPosition = transform.position;            

            _visuals.transform.rotation = Quaternion.Slerp(_visuals.transform.rotation, targetDirection, _rotationSpeed * Time.deltaTime);
        }
    }
}
