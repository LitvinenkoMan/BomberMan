using Interfaces;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PlayerCameraModeController : MonoBehaviour, ICameraModeController
    {
        [SerializeField] private GameObject CameraExample;

        private ICameraViewer _cameraViewer;
        private GameObject _instantiatedCamera;

        private void OnEnable()
        {
            PlayerSpawner.Instance.OnPlayerSpawned += FollowSpawnedPlayer;
            CheckForCameraInstance();
        }
        private void OnDisable()
        {
            PlayerSpawner.Instance.OnPlayerSpawned -= FollowSpawnedPlayer;
        }

        private void CheckForCameraInstance()
        {
            if (_instantiatedCamera == null)
            {
                _instantiatedCamera = Instantiate(CameraExample, Vector3.zero, Quaternion.identity);
                _cameraViewer = _instantiatedCamera.GetComponent<ICameraViewer>();
            }
        }

        public void SwitchToGameplayMode()
        {
            _cameraViewer.ClearTargetsList();
            _cameraViewer.AddToViewTarget(PlayerSpawner.Instance.Player.transform);
        }

        public void SwitchToViewerMode()
        {
            throw new System.NotImplementedException();
        }

        public void FollowSpawnedPlayer()
        {
            Debug.Log("FollowSpawnedPlayer");
            SwitchToGameplayMode();
        }
    }
}
