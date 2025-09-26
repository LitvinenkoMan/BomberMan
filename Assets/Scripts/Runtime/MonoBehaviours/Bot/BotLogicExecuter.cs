using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours.Bot.SimpleBotUtils;
using Runtime.MonoBehaviours.Bot.StandartBotUtils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot
{
    public class BotLogicExecuter : MonoBehaviour
    {
        [SerializeField] private BotType _botType;
        [SerializeField] private bool _onAStar;
        private IBotNavigation _botNavigation;
        private ITargetOpponentSelector _targetOpponentFinder;
        private ICharacterRuntimeData _characterData;
        private IState _currentState;
        private BotCharacter _character;
        private SimpleShelterFinder _retreatPositionFinder;

        private Dictionary<string, IState> _states;
        private Vector3 _spawnedBombPos;

        public Dictionary<string, IState> States => _states;
        public BotCharacter Character => _character;
        public ICharacterRuntimeData CharacterData => _characterData;
        public IBotNavigation BotNavigation => _botNavigation;
        public ITargetOpponentSelector TargetOpponentFinder => _targetOpponentFinder;

        //-----for Debuging-----
        private Queue<GroundSection> groundSections;
        //----------------------

        private void OnDrawGizmos()
        {
            if (groundSections == null) return;
            foreach (var groundSection in groundSections)
            {
                Gizmos.DrawSphere(groundSection.transform.position + new Vector3(0, 1, 0), 0.3f);
            }
        }

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
            _states = botBehaviorProvider.GetStatesForType(_botType);

            _retreatPositionFinder = new SimpleShelterFinder(GetComponent<NavMeshAgent>());

            _characterData = _character.CharacterData;
        }
        public void SetBombPosition(Vector3 position)
        {
            _spawnedBombPos = position;
        }

        public void RetreatFromBomb()
        {
            byte explosionRange = (byte)_characterData.BombsSpreading;

            var blacklistPositions = _retreatPositionFinder.GenerateBlacklistPositions(explosionRange, _character.BombDto.BombPosition);
            var possiblePositions = _retreatPositionFinder.GeneratePossiblePositions(explosionRange, blacklistPositions, _character.BombDto.BombPosition);
            var availablePositions = _retreatPositionFinder.FindAvailablePosForRetreat(possiblePositions, blacklistPositions);

            if (availablePositions == null || availablePositions.Count == 0)
            {
                Debug.LogWarning("No available positions for retreat, staying in place.");
                _botNavigation.SetTarget(transform.position);
                return;
            }
            else
            {
                int randInt = UnityEngine.Random.Range(0, availablePositions.Count);
                _botNavigation.SetTarget(availablePositions[randInt]);
            }
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
            if (_onAStar)
            {
                _botNavigation.SetTarget(Vector3.zero);
                groundSections = _botNavigation.DebugingGetList();
            }
        }
    }
}