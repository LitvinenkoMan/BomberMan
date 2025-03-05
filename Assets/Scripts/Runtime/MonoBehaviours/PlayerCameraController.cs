using System;
using System.Collections.Generic;
using Cinemachine;
using Interfaces;
using Runtime.NetworkBehaviours;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PlayerCameraController : MonoBehaviour, ICameraModeController
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
            var playersObj = FindObjectsByType<BomberParamsProvider>(FindObjectsSortMode.None);
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
            CatchPlayersGameObject();
        }

        private void OnPlayerDeathResponce(ulong obj)
        {
            SwitchToViewerMode();
        }
    }
}
