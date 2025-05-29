using System.Collections.Generic;
using System.Timers;
using Interfaces;
using Runtime.NetworkBehaviours;
using Runtime.NetworkBehaviours.Player;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class PlayerCameraModeController : MonoBehaviour, ICameraModeController
    {
        [SerializeField] private GameObject CameraExample;


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
            string str = "Collecting Players: ";
            playersTransform.Clear();
            var playersObj =
                FindObjectsByType<CharacterController>(FindObjectsSortMode
                    .None); // Every bot or player will have it (i thinmk so..)
            foreach (var obj in playersObj)
            {
                str += $"{obj.transform.name} ";
                if (obj.gameObject.activeInHierarchy)
                {
                    playersTransform.Add(obj.transform);
                }
            }

            Debug.Log(str);
        }

        public void SwitchToGameplayMode()
        {
            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            _cameraViewer.ClearTargetsList();
            _cameraViewer.AddToViewTarget(playerObject.gameObject.transform);
        }

        public void SwitchToViewerMode()
        {
            string str = "Switching to player viewer mode";
            _cameraViewer.ClearTargetsList();
            foreach (var playerTransform in playersTransform)
            {
                str += $"\nwatching {playerTransform.gameObject.name},";
                _cameraViewer.AddToViewTarget(playerTransform);
            }

            Debug.Log(str);
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
            Debug.Log($"Player P{clientId} is spawned");
            NetworkObject playerObject = NetworkManager.Singleton.LocalClient.PlayerObject;
            CatchPlayersGameObject();

            if (clientId == NetworkManager.Singleton.LocalClient.ClientId)
            {
                if (playerObject && playerObject.gameObject.activeInHierarchy)
                {
                    if (playerObject.gameObject.TryGetComponent(out ICharacter character))
                    {
                        character.Health.OnHealthRunOut += OnPlayerDeathResponce;
                        SwitchToGameplayMode();
                        Debug.Log("Watching player");
                    }
                }
            }
            else
            {
                if (playerObject && playerObject.gameObject.activeInHierarchy) return;

                SwitchToViewerMode();
                Debug.Log("PLayer Is dead, watching others players");
            }
        }

        private void OnPlayerDeathResponce()
        {
            SwitchToViewerMode();
        }
    }
}