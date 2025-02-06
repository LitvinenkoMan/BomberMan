using Runtime.NetworkBehaviours;
using Runtime.NetworkBehaviours.MatchManagers;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours.UI
{
    public class DeathmatchPresenter : GameplayPresenter
    {
        [Space(30)] 
        [SerializeField] private DeathMatchMm DeathMatchManager;

        [Space(10)] 
        [SerializeField] private TMP_Text LifeCountUI;

        protected override void OnEnable()
        {
            base.OnEnable();
            PlayerSpawner.Instance.OnPlayerSpawned += UpdateLifeCount;
            DeathMatchManager.OnInitialized += VisualiseBaseParams;
        }

        private void VisualiseBaseParams()
        {
            LifeCountUI.text = DeathMatchManager.GetPlayerLifeCount(NetworkManager.Singleton.LocalClient.ClientId).ToString();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            PlayerSpawner.Instance.OnPlayerSpawned -= UpdateLifeCount;
            DeathMatchManager.OnInitialized -= VisualiseBaseParams;

        }

        private void UpdateLifeCount(ulong clientId)
        {
            if (NetworkManager.Singleton.LocalClient.ClientId == clientId)
            {
                LifeCountUI.text = DeathMatchManager.GetPlayerLifeCount(clientId).ToString();
            }
        }
    }
}
