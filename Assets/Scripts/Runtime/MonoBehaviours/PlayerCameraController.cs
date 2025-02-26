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
            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            if (playerObject)
            {
                CheckForCameraInstance(playerObject);
            }
        }

        private void CheckForCameraInstance(NetworkObject playerObject)
        {
            if (!_instantiatedCamera)
            {
                _instantiatedCamera = Instantiate(CameraExample, Vector3.zero, new Quaternion(0, 0, 0, 0));
                _cameraViewer = _instantiatedCamera.GetComponent<ICameraViewer>();
                _cameraViewer.AddToViewTarget(playerObject.gameObject.transform);    
            }
        }

        private void FollowSpawnedPlayer(ulong clientId)
        {
            Debug.Log($"Started to follow local player");
            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            CheckForCameraInstance(playerObject);
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
