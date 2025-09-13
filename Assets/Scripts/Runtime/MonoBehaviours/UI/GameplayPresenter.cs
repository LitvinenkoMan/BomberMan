using Core.EventBuses;
using Core.SaveSystem;
using Core.ScriptableObjects;
using Interfaces;
using Runtime.NetworkBehaviours.MatchManagers;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours.UI
{
    public class GameplayPresenter : MonoBehaviour
    {
        [SerializeField]
        private MatchManager CurrentMatchManager;

        [Header("Player Info UI")]
        [SerializeField] private TMP_Text HealtText;
        [SerializeField] private TMP_Text SpeedText;
        [SerializeField] private TMP_Text BombsDamageText;
        [SerializeField] private TMP_Text SpreadText;
        [SerializeField] private TMP_Text BombsPerTimeText;

        protected virtual void OnEnable()
        {
            //GameplayUIEvents.Instance.Subscribe<ICharacterRuntimeData>(UpdateUI);
        }

        private void UpdateUI(ICharacterRuntimeData obj)
        {
            UpdateHealthText(obj.CharacterHealth);
            UpdateSpeedText(obj.SpeedMultiplier);
            UpdateBombsDamageText(obj.BombsDamage);
            UpdateSpreadText(obj.BombsSpreading);
            UpdateBombsPerTimeText(obj.BombsSpreading);
        }

        protected virtual void OnDisable()
        {
            //GameplayUIEvents.Instance.Unsubscribe<ICharacterRuntimeData>(UpdateUI);
        }
        
        public void ExitToMainMenu()
        {
            CurrentMatchManager.SendDisconnectRequestServerRpc(NetworkManager.Singleton.LocalClient.ClientId);
            //TODO: Make Exit Menu
        }

        private void UpdateHealthText(int newValue) => HealtText.text = $"x{newValue}";
        private void UpdateSpeedText(float newValue) => SpeedText.text = $"x{newValue}";
        private void UpdateBombsDamageText(int newValue) => BombsDamageText.text = $"x{newValue}";
        private void UpdateSpreadText(int newValue) => SpreadText.text = $"x{newValue}";
        private void UpdateBombsPerTimeText(int newValue) => BombsPerTimeText.text = $"x{newValue}";
        
    }
}
