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
        [SerializeField]
        private GameObject VictoryScreen;
        [SerializeField]
        private TMP_Text WinnerTextFielad;

        protected override void OnEnable()
        {
            base.OnEnable();
            DeathMatchManager.OnInitialized += VisualiseBaseParams;
            DeathMatchManager.OnLifeCountInfoReceived += UpdateLifeCount;
            DeathMatchManager.OnWinnerAppeared += EnableVictoryScreen;
            DeathMatchManager.OnResetUI += ResetUI;
        }

        private void VisualiseBaseParams()
        {
            DeathMatchManager.RequestLifeCountDataRpc(NetworkManager.Singleton.LocalClientId);
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            DeathMatchManager.OnInitialized -= VisualiseBaseParams;

        }

        private void UpdateLifeCount(int lifeCount)
        {
            LifeCountUI.text = lifeCount.ToString();
        }

        private void EnableClientView()
        {
            
        }

        private void EnableServerView()
        {
            
        }

        private void EnableVictoryScreen(ulong winnerId)
        {
            WinnerTextFielad.text = $"Winner: P{winnerId}";
            VictoryScreen.SetActive(true);
        }

        private void ResetUI(bool isServer)
        {
            VictoryScreen.SetActive(false);
        }
    }
}
