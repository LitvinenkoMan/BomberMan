using AbstractClasses;
using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours.Bot.StandartBotUtils;
using Runtime.MonoBehaviours.Player;
using Runtime.NetworkBehaviours.PowerUps;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot
{
    public class BotLogicExecuter : MonoBehaviour
    {
        [SerializeField] private BotType _botType;
        [SerializeField] private Canvas canvasObj;
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
        private Queue<GroundSection> path;

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(GetComponent<NavMeshAgent>().destination, 0.3f);
            //if (blacklist != null)
            //{
            //    foreach (var b in blacklist)
            //    {
            //        Gizmos.color = Color.black;
            //        Gizmos.DrawSphere(new Vector3(b.x, 0, b.y) + new Vector3(0, 1f, 0), 0.3f);
            //    }
            //}
            if (path == null) return;
            foreach (var b in path)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(b.transform.position + new Vector3(0, 0.5f, 0), 0.3f);
            }
        }
        public void UnsubscribeFromEvent()
        {
            Spawner.Instance.OnBotSpawned -= _targetOpponentFinder.UpdateOpponentsList;
            Spawner.Instance.OnPlayerSpawned -= _targetOpponentFinder.UpdatePlayerInOpponentsList;

            _character.OnBombDeployed -= ShelterFinder.GenerateBlacklistPositions;
        }
        public void Initialize(BotCharacter botCharacter) // call in Start
        {
            _character = GetComponent<BotCharacter>();
            _characterData = _character.CharacterData;

            List<Canvas> canvas = new List<Canvas>();
            //for (int i = 0; i < 100; i++)
            //{
            //    var newCanvas = Instantiate(canvasObj);
            //    newCanvas.gameObject.SetActive(false);
            //    canvas.Add(newCanvas);
            //}

            BotBehaviorProvider botBehaviorProvider = new BotBehaviorProvider();
            botBehaviorProvider.InitializeBehaviors(GetComponent<NavMeshAgent>(), canvas);

            _botNavigation = botBehaviorProvider.GetBotNavigationForType(_botType);
            _targetOpponentFinder = botBehaviorProvider.GetTargetSelectorForType(_botType);
            _shelterFinder = botBehaviorProvider.GetShelterFinder(_botType);
            _states = botBehaviorProvider.GetStatesForType(_botType);

            Spawner.Instance.OnBotSpawned += _targetOpponentFinder.UpdateOpponentsList;
            Spawner.Instance.OnPlayerSpawned += _targetOpponentFinder.UpdatePlayerInOpponentsList;

            _targetOpponentFinder.SetOpponentsList(Spawner.Instance.OpponentsList);

            SwitchState(_states["Agro"]);
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
            //path = _botNavigation.GetPath();  
            //blacklist = BombPositions.GetAllBombPositions();
            _currentState.Update(this);
        }
    }
}