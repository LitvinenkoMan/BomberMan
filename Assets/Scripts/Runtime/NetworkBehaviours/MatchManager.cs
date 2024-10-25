using System.Threading.Tasks;
using MonoBehaviours.Network;
using Runtime.MonoBehaviours;
using Runtime.NetworkBehaviours;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace MonoBehaviours
{
    public class MatchManager : NetworkBehaviour
    {
        [SerializeField] private TMP_Text JoinCodeText;
        public byte ConnectedPlayers;
        public byte CurrentPlayers;


        public UnityEvent StartMatch;

        public override void OnNetworkSpawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback += RegisterPlayerForEvents;
                RegisterPlayerForEvents(NetworkManager.Singleton.LocalClientId);
            }
        }

        public override void OnNetworkDespawn()
        {
            if (IsServer)
            {
                NetworkManager.OnClientConnectedCallback -= RegisterPlayerForEvents;
            }
        }

        private void Start()
        {
            JoinCodeText.text = RelayManager.Instance.JoinCode;
        }

        private void RegisterPlayerForEvents(ulong clientID)
        {
            if (NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject.TryGetComponent(out DeathResultHandler deathResultHandler))
            {
                deathResultHandler.OnPlayerDeathAction += SendRespawnRequestForPlayerWrapper;
                Debug.LogWarning($"Registered player with ID: {clientID}");
            }
            else
            {
                Debug.LogError($"Unable to find DeathResultHandler on object {NetworkManager.Singleton.ConnectedClients[clientID].PlayerObject}: {clientID}");
            }
        }

        private void SendRespawnRequestForPlayerWrapper(ulong clientID) => SendRespawnRequestForPlayer(clientID);

        private async Task SendRespawnRequestForPlayer(ulong clientID)
        {
            // TODO Add check for ability to respawn
            await PlayerSpawner.Instance.SpawnClient(clientID, 3); // TODO Set this Time to variable
            RegisterPlayerForEvents(clientID);
        }
    }
}
