using UnityEngine;

namespace Runtime.MonoBehaviours.Player
{
    public class PlayerCharacterRotator : MonoBehaviour
    {
        [SerializeField] private GameObject _visuals;
        [SerializeField] private float _rotationSpeed;
        
        private Quaternion _targetDirection;
        private Vector3 _previousPosition;
        
        void Start()
        {
            _previousPosition = transform.position;
            _targetDirection = Quaternion.identity;
        }

        void Update()
        {
            if (_previousPosition == transform.position) return;

            _targetDirection = Quaternion.LookRotation(transform.position - _previousPosition);
            _targetDirection.x = 0; // Y-axis rotation only

            _previousPosition = transform.position;            

            _visuals.transform.rotation = Quaternion.Slerp(_visuals.transform.rotation, _targetDirection, _rotationSpeed * Time.deltaTime);
        }
    }
}
