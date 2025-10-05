using AbstractClasses;
using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot
{
    public class BotLogicExecuter : MonoBehaviour
    {
        [SerializeField] private BotType _botType;
        private ICharacterRuntimeData _characterData;
        private IState _currentState;
        private BaseBotNavigation _botNavigation;
        private BaseShelterFinder _shelterFinder;        
        private BaseTargetOpponentSelector _targetOpponentFinder;
        private BotCharacter _character;
        private Dictionary<string, IState> _states;

        public Dictionary<string, IState> States => _states;
        public BotCharacter Character => _character;
        public BaseTargetOpponentSelector TargetOpponentFinder => _targetOpponentFinder;
        public ICharacterRuntimeData CharacterData => _characterData;
        public BaseBotNavigation BotNavigation => _botNavigation;
        public BaseShelterFinder ShelterFinder => _shelterFinder;

        /*------------Debugging--------------*/
        private HashSet<Vector2Int> blacklist;
        private Queue<GroundSection> queue;

        private void Awake()
        {
            CollectRefs();
        }

        private void OnDrawGizmos()
        {
            foreach (var b in blacklist)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(new Vector3(b.x, 0, b.y) + new Vector3(0, 0.5f, 0), 0.3f);
            }
            if (queue == null) return;  
            foreach (var b in queue)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(b.transform.position + new Vector3(0, 0.5f, 0), 0.3f);
            }
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
            queue = _botNavigation.GetPath();  
            blacklist = _shelterFinder.GetBlacklist();
            _currentState.Update(this);
        }
    }
}