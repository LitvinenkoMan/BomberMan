using MonoBehaviours;
using ScriptableObjects;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours.UI
{
    public class GameplayPresenter : MonoBehaviour
    {
        [SerializeField]
        private BaseBomberParameters BomberParams;
        [SerializeField]
        private MatchManager CurrentMatchManager;

        [Header("Player Info UI")]
        [SerializeField] private TMP_Text HealtText;
        [SerializeField] private TMP_Text SpeedText;
        [SerializeField] private TMP_Text BombsDamageText;
        [SerializeField] private TMP_Text SpreadText;
        [SerializeField] private TMP_Text BombsPerTimeText;

        private void OnEnable()
        {
            UpdateHealthText(BomberParams.ActorHealth);
            UpdateSpeedText(BomberParams.SpeedMultiplier);
            UpdateBombsDamageText(BomberParams.BombsDamage);
            UpdateSpreadText(BomberParams.BombsSpreading);
            UpdateBombsPerTimeText(BomberParams.BombsAtTime);
            
            BomberParams.OnHealthChangedEvent += UpdateHealthText;
            BomberParams.OnSpeedChangedEvent += UpdateSpeedText;
            BomberParams.OnDamageChangedEvent += UpdateBombsDamageText;
            BomberParams.OnSpreadingChangedEvent += UpdateSpreadText;
            BomberParams.OnBombsPerTimeChangedEvent += UpdateBombsPerTimeText;
        }

        private void OnDisable()
        {
            BomberParams.OnHealthChangedEvent -= UpdateHealthText;
            BomberParams.OnSpeedChangedEvent -= UpdateSpeedText;
            BomberParams.OnDamageChangedEvent -= UpdateBombsDamageText;
            BomberParams.OnSpreadingChangedEvent -= UpdateSpreadText;
            BomberParams.OnBombsPerTimeChangedEvent -= UpdateBombsPerTimeText;
        }
        
        public void ExitToMainMenu()
        {
            CurrentMatchManager.SendDisconnectRequestServerRpc(NetworkManager.Singleton.LocalClient.ClientId);
            //TODO: Make Exit Menu
        }

        private void UpdateHealthText(int newValue) => HealtText.text = $"{newValue}";
        private void UpdateSpeedText(float newValue) => SpeedText.text = $"{newValue}";
        private void UpdateBombsDamageText(int newValue) => BombsDamageText.text = $"{newValue}";
        private void UpdateSpreadText(int newValue) => SpreadText.text = $"{newValue}";
        private void UpdateBombsPerTimeText(int newValue) => BombsPerTimeText.text = $"{newValue}";
        
    }
}
