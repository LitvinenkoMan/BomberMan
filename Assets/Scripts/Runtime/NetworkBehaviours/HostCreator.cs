using Core.ScriptableObjects;
using MonoBehaviours.Network;
using ScriptableObjects;
using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Runtime.NetworkBehaviours
{
    public class HostCreator : MonoBehaviour
    {
        [Header("Buttons to create room")] [SerializeField]
        private Button CreateHostButton;
        
        [SerializeField]
        private TMP_InputField RoomPassword;

        [SerializeField]
        private GeneralGameData GameData;

        
        [Space(20)]
        [Header("Hosting Events:")]
        [Space(10)]
        [SerializeField, Tooltip("This will fire at moment when host is only trying to launch.")]
        public UnityEvent OnHostLaunched;
        [SerializeField]
        public UnityEvent OnHostStarted;

        private void OnEnable()
        {
            CreateHostButton.onClick.AddListener(CreateHost);
            if (NetworkManager.Singleton)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }

        private void OnDisable()
        {
            CreateHostButton.onClick.RemoveListener(CreateHost);
        }

        private async void CreateHost()
        {
            OnHostLaunched?.Invoke();
            
            Allocation allocation = await RelayManager.Instance.CreateRelay(GameData.LastSelectedLevelData.MaxAmountOfPlayers);
            
            if (allocation != null)
            {
                string joinCode = await RelayManager.Instance.GetJoinCode(allocation);
                //RoomJoinCodeText.text = joinCode;
                if (!string.IsNullOrEmpty(joinCode))
                {
                    NetworkManager.Singleton.GetComponent<UnityTransport>()
                        .SetRelayServerData(new RelayServerData(allocation, "dtls"));
                        
                    NetworkManager.Singleton.StartHost();
                    ServerStarted();
                }
            }
        }

        private void ServerStarted()
        {
            //PlayerSpawner.Instance.SpawnPlayer();
            OnHostStarted?.Invoke();
        }
    }
}
