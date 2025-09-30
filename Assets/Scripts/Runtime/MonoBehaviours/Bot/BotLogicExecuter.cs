using Interfaces;
using Runtime.MonoBehaviours.Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot
{
    public class BotLogicExecuter : MonoBehaviour
    {
        [SerializeField] private BotType _botType;
        private IBotNavigation _botNavigation;
        private IShelterFinder _shelterFinder;
        private ICharacterRuntimeData _characterData;
        private IState _currentState;
        private BotCharacter _character;
        private BaseTargetOpponentSelector _targetOpponentFinder;
        private Dictionary<string, IState> _states;

        public Dictionary<string, IState> States => _states;
        public BotCharacter Character => _character;
        public BaseTargetOpponentSelector TargetOpponentFinder => _targetOpponentFinder;
        public ICharacterRuntimeData CharacterData => _characterData;
        public IBotNavigation BotNavigation => _botNavigation;
        public IShelterFinder ShelterFinder => _shelterFinder;

        private void Awake()
        {
            CollectRefs();
        }

        private void Start()
        {
            Spawner.Instance.OnBotSpawned += _targetOpponentFinder.UpdateOpponentsList;
            Spawner.Instance.OnPlayerSpawned += _targetOpponentFinder.UpdatePlayerInOpponentsList;

            _targetOpponentFinder.SetOpponentsList(Spawner.Instance.OpponentsList);

            SwitchState(_states["Agro"]);
        }
        private void OnDisable()
        {
            Spawner.Instance.OnBotSpawned -= _targetOpponentFinder.UpdateOpponentsList;
            Spawner.Instance.OnPlayerSpawned -= _targetOpponentFinder.UpdatePlayerInOpponentsList;
        }

        private void CollectRefs()
        {
            if (TryGetComponent(out BotCharacter character)) _character = character;

            BotBehaviorProvider botBehaviorProvider = new BotBehaviorProvider();
            botBehaviorProvider.InitializeBehaviors(GetComponent<NavMeshAgent>());

            _botNavigation = botBehaviorProvider.GetBotNavigationForType(_botType);
            _targetOpponentFinder = botBehaviorProvider.GetTargetSelectorForType(_botType);
            _shelterFinder = botBehaviorProvider.GetShelterFinder(_botType);
            _states = botBehaviorProvider.GetStatesForType(_botType);

            _characterData = _character.CharacterData;
        }
        
        public void SwitchState(IState newState)
        {
            if (_currentState != null)
            {
                _currentState.Exit(this);
            }
            _currentState = newState;
            _currentState.Enter(this);
        }
        private void Update()
        {
            _currentState.Update(this);
        }
    }    
}