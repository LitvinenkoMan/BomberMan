using System.Collections.Generic;
using Interfaces;
using Runtime.NetworkBehaviours;
using Runtime.NetworkBehaviours.Player;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PlayerCameraModeController : MonoBehaviour, ICameraModeController
    {
        [SerializeField] 
        private GameObject CameraExample;


        private GameObject _instantiatedCamera;
        private ICameraViewer _cameraViewer;

        private List<Transform> playersTransform;

        private void OnEnable()
        {
            playersTransform = new List<Transform>();
            PlayerSpawner.Instance.OnPlayerSpawned += FollowSpawnedPlayer;
            CheckForCameraInstance();
            CheckForInstancedPlayer();
        }

        private void OnDisable()
        {
            PlayerSpawner.Instance.OnPlayerSpawned -= FollowSpawnedPlayer;
        }

        private void CatchPlayersGameObject()
        {
            playersTransform.Clear();
            var playersObj = FindObjectsByType<CharacterController>(FindObjectsSortMode.None); // Every bot or player will have it (i thinmk so..)
            foreach (var obj in playersObj)
            {
                playersTransform.Add(obj.transform);
            }
        }
        
        public void SwitchToGameplayMode()
        {
            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            _cameraViewer.ClearTargetsList();
            _cameraViewer.AddToViewTarget(playerObject.gameObject.transform);
        }

        public void SwitchToViewerMode()
        {
            _cameraViewer.ClearTargetsList();
            foreach (var playerTransform in playersTransform)
            {
                _cameraViewer.AddToViewTarget(playerTransform);
            }
        }

        private void CheckForInstancedPlayer()
        {
            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (playerObject)
            {
                FollowSpawnedPlayer(NetworkManager.Singleton.LocalClient.ClientId);
            }
        }

        private void CheckForCameraInstance()
        {
            if (_instantiatedCamera == null)
            {
                _instantiatedCamera = Instantiate(CameraExample, Vector3.zero, new Quaternion(0, 0, 0, 0));
                _cameraViewer = _instantiatedCamera.GetComponent<ICameraViewer>();
            }
        }

        private void FollowSpawnedPlayer(ulong clientId)
        {
            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            SwitchToGameplayMode();

            if (playerObject.gameObject.TryGetComponent(out IHealth health))
            {
                health.OnHealthRunOut += OnPlayerDeathResponce;
            }
            CatchPlayersGameObject();
        }

        private void OnPlayerDeathResponce()
        {
            SwitchToViewerMode();
        }
    }
}
