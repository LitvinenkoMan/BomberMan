using Core.ScriptableObjects;
using Runtime.MonoBehaviours;
using Runtime.MonoBehaviours.Player;
using Runtime.MonoBehaviours.Bot;
using UnityEngine;
using UnityEngine.Events;

namespace Runtime.NetworkBehaviours.MatchManagers
{
    public class TrainingMatchManager : MonoBehaviour
    {
        [SerializeField] private BaseBomberParameters bomberParams;


        public UnityEvent StartMatchUnityEvent;

        private void Start()
        {            
            SubscribeToEvents();
            Spawn();
            StartMatchUnityEvent?.Invoke();
        }
        private void OnDestroy()
        {
            UnSubscribeToEvents();
        }
        public void Spawn()
        {
            Spawner.Instance.SpawnPlayer(0);
            Spawner.Instance.SpawnBots();
        }

        private void RespawnPlayer()
        {
            Spawner.Instance.SpawnPlayer(3);
        }
        
        private void RespawnBotByName(string name)
        {
            Spawner.Instance.RespawnBot(name, 3);
        }

        private void RegisterPlayerForEvents()
        {
            if (Spawner.Instance.Player.TryGetComponent(out PlayerCharacter playerCharacter)) {
                playerCharacter.OnPlayerDeath += RespawnPlayer;
            }
        }

        private void RegisterBotForEvents(string name)
        {
            GameObject bot = Spawner.Instance.GetBotByName(name);
            if (bot.TryGetComponent(out BotCharacter botCharacter))
            {
                botCharacter.OnBotDeath += RespawnBotByName;
            }
        }
        
        private void SubscribeToEvents()
        {
            Spawner.Instance.OnPlayerSpawned += RegisterPlayerForEvents;
            Spawner.Instance.OnBotSpawned += RegisterBotForEvents;
        }
        private void UnSubscribeToEvents()
        {
            Spawner.Instance.OnPlayerSpawned -= RegisterPlayerForEvents;
            Spawner.Instance.OnBotSpawned -= RegisterBotForEvents;
        }
    }
}