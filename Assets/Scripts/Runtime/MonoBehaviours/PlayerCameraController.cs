using System;
using Cinemachine;
using Interfaces;
using Runtime.NetworkBehaviours;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PlayerCameraController : MonoBehaviour, IPlayerViewCamera
    {
        [SerializeField] 
        private GameObject CameraExample;


        private GameObject _instantiatedCamera;
        private ICameraViewer _cameraViewer;

        private void OnEnable()
        {
            PlayerSpawner.Instance.OnPlayerSpawned += FollowSpawnedPlayer;
            CheckForCameraInstance();
            CheckForInstancedPlayer();
        }

        private void OnDisable()
        {
            PlayerSpawner.Instance.OnPlayerSpawned -= FollowSpawnedPlayer;
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
            foreach (var client in NetworkManager.Singleton.ConnectedClients)
            {
                _cameraViewer.AddToViewTarget(client.Value.PlayerObject.transform);
            }
        }

        private void CheckForInstancedPlayer()
        {
            Debug.Log($"Searching for PlayerObject to follow");
            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (playerObject)
            {
                Debug.Log($"Got One");
                FollowSpawnedPlayer(NetworkManager.Singleton.LocalClient.ClientId);
            }
        }

        private void CheckForCameraInstance()
        {
            Debug.Log($"Checking if camera exist");
            if (_instantiatedCamera == null)
            {
                Debug.Log($"Instantiating camera");
                _instantiatedCamera = Instantiate(CameraExample, Vector3.zero, new Quaternion(0, 0, 0, 0));
                _cameraViewer = _instantiatedCamera.GetComponent<ICameraViewer>();
            }
        }

        private void FollowSpawnedPlayer(ulong clientId)
        {
            Debug.Log($"Started to follow local player");
            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            SwitchToGameplayMode();

            if (playerObject.gameObject.TryGetComponent(out DeathResultHandler handler))
            {
                handler.OnPlayerDeathAction += OnPlayerDeathResponce;
            }
        }

        private void OnPlayerDeathResponce(ulong obj)
        {
            SwitchToViewerMode();
        }
    }
}
