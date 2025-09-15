using Interfaces;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MonoBehaviours.GroundSectionSystem;

namespace Interfaces
{
    public interface IBotEasyLogic
    {

    }
}
namespace Runtime.MonoBehaviours.Bot
{
    public class BotLogicExecuter : MonoBehaviour
    {
        private BotCharacter _character;
        private NavMeshAgent _agent;
        private List<GameObject> _opponents;
        private Transform _targetOpponent;
        private Vector3 _target;
        private Dictionary<string, IState> _states;
        private ICharacterRuntimeData _characterData;
        private IState _currentState;
        private Vector3 _spawnedBombPos;
        private GameObject _player;
        private RetreatPositionFinder _retreatPositionFinder;

        public Transform TargetOpponent => _targetOpponent;
        public Vector3 Target => _target;
        public Dictionary<string, IState> States => _states;
        public BotCharacter Character => _character;
        public ICharacterRuntimeData CharacterData => _characterData;

        private void Awake()
        {
            CollectRefs();
        }
        
        private void Start()
        {
            Spawner.Instance.OnBotSpawned += UpdateOpponentsList;
            Spawner.Instance.OnPlayerSpawned += UpdatePlayerInList;

            GetOpponentsList();

            _player = Spawner.Instance.Player;

            SwitchState(_states["Agro"]);
            SetTarget(_targetOpponent.position);
            SetSpeed(3f);
        }
        private void OnDisable()
        {
            Spawner.Instance.OnBotSpawned -= UpdateOpponentsList;
            Spawner.Instance.OnPlayerSpawned -= UpdatePlayerInList;
        }

        private void CollectRefs()
        {
            if (TryGetComponent(out BotCharacter character)) _character = character;
            if (TryGetComponent(out NavMeshAgent agent)) _agent = agent;
            _retreatPositionFinder = new RetreatPositionFinder();
            
            _characterData = _character.CharacterRuntimeData;
            

            StatesSelector statesSelector = new StatesSelector();
            _states = statesSelector.GetStatesForType(BotType.Easy);
        }
        public void GetOpponentsList()
        {
            _opponents = new List<GameObject>();
            foreach (var opponent in Spawner.Instance.OpponentsList)
            {
                if (opponent != null && opponent != gameObject) _opponents.Add(opponent);
            }
        }
        public float CheckDistance()
        {
            if (_target == null)
            {
                Debug.Log("CheckDistance: target = null");
                return 0f;
            }
            if (_targetOpponent == null)
            {
                Debug.Log("CheckDistance: targetOpponent = null");
                SelectTargetPlayer();
            }

            return Mathf.Min((transform.position - _target).magnitude, 
                (transform.position - _targetOpponent.position).magnitude);
        }
        public void SetBombPosition(Vector3 position)
        {
            _spawnedBombPos = position;
        }

        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public void SetTarget(Vector3 target)
        {
            _target = target;
            _agent.destination = target;
        }

        public void CheckPath()
        {
            NavMeshPath path = new NavMeshPath();
            _agent.CalculatePath(_targetOpponent.position, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                SetTarget(_targetOpponent.transform.position);
            }
            else
            {
                if (path.corners.Length > 0)
                {
                    SetTarget(path.corners[path.corners.Length - 1]);
                }
                else
                {
                    Debug.Log("CheckPath: have not corners of the path");
                    SelectTargetPlayer();
                }
            }
        }
        public void SelectTargetPlayer()
        {
            float minDistance = 10000f;
            Transform target = null;

            foreach (var opponent in _opponents)
            {
                if (opponent == null) continue;
                if ((transform.position - opponent.transform.position).magnitude < minDistance)
                {
                    minDistance = (transform.position - opponent.transform.position).magnitude;
                    target = opponent.transform;
                }
            }
            if (target != null)
            {
                _targetOpponent = target;
                
            }
            else Debug.Log("SelectTargetPlayer: не найдена цель преследования");
        }

        public void UpdateOpponentsList(string name)
        {         
            _opponents.RemoveAll(opponent => opponent == null || opponent.name == name);
            GameObject newBot = Spawner.Instance.GetOpponentByName(name);
            _opponents.Add(newBot);
        }

        public void UpdatePlayerInList()
        {
            _opponents.RemoveAll(opponents => opponents == null || opponents == _player);

            GameObject newPlayer = Spawner.Instance.Player;
            if (newPlayer != null && !_opponents.Contains(newPlayer))
            {
                _opponents.Add(newPlayer);
                _player = newPlayer;
            }
        }
        public void RetreatFromBomb()
        {
            byte explosionRange = (byte)_characterData.BombsSpreading;

            var blacklistPositions = _retreatPositionFinder.GenerateBlacklistPositions(explosionRange, _character.BombDto.BombPosition);
            var possiblePositions = _retreatPositionFinder.GeneratePossiblePositions(explosionRange, blacklistPositions, _character.BombDto.BombPosition);
            var availablePositions = _retreatPositionFinder.FindAvailablePosForRetreat(possiblePositions, blacklistPositions, _agent);

            if (availablePositions == null || availablePositions.Count == 0)
            {
                Debug.LogWarning("No available positions for retreat, staying in place.");
                SetTarget(transform.position);
                return;
            }
            else
            {
                int randInt = UnityEngine.Random.Range(0, availablePositions.Count);
                SetTarget(availablePositions[randInt]);
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
        }
    }
}