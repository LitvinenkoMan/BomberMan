using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace Interfaces
{
    public interface IBotNavigation
    {
        public void SetTarget(Vector3 target);
        public float CheckDistance(Transform targetOpponent);
        public void CheckPathToTarget(Transform targetOpponent);
        public void SetSpeed(float speed);
    }
    public interface ITargetOpponentSelector
    {
        public void SetOpponentsList(List<GameObject> list);
        public void SelectTargetOpponent();
        public void UpdateOpponentsList(string name);
        public void UpdatePlayerInOpponentsList();
        public Transform GetCurrentOpponent();
    }
}
public class BotNavigation : IBotNavigation
{
    private NavMeshAgent _agent;
    private Vector3 _target;
    public Vector3 Target => _target;

    public BotNavigation(NavMeshAgent agent)
    {
        _agent = agent;
    }
    public void SetTarget(Vector3 target)
    {
        _target = target;
        _agent.destination = target;
    }
    public float CheckDistance(Transform targetOpponent)
    {
        if (targetOpponent == null)
        {
            Debug.Log("CheckDistance: targetOpponent = null");
        }

        return Mathf.Min((_agent.transform.position - _target).magnitude,
            (_agent.transform.position - targetOpponent.position).magnitude);
    }

    public void CheckPathToTarget(Transform targetOpponent)
    {
        NavMeshPath path = new NavMeshPath();
        _agent.CalculatePath(targetOpponent.position, path);
        if (path.status == NavMeshPathStatus.PathComplete)
        {
            SetTarget(targetOpponent.transform.position);
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
                SetTarget(_target);
            }
        }
    }
    public void SetSpeed(float speed)
    {
        _agent.speed = speed;
    }
}
public class TargetOpponentSelector : ITargetOpponentSelector
{
    private Transform _thisBot;
    private Transform _targetOpponent;
    private GameObject _currentPlayer;
    private List<GameObject> _opponentsList;
    public Transform TargetOpponent => _targetOpponent;

    public TargetOpponentSelector(Transform thisBot)
    {
        _thisBot = thisBot;
    }

    public void SetOpponentsList(List<GameObject> opponentsList)
    {
        _opponentsList = opponentsList;
        _opponentsList.Remove(_thisBot.gameObject);

        _currentPlayer = Spawner.Instance.Player;
    }
    public void UpdateOpponentsList(string name)
    {
        _opponentsList.RemoveAll(opponent => opponent == null || opponent.name == name);
        GameObject newBot = Spawner.Instance.GetOpponentByName(name);
        _opponentsList.Add(newBot);
    }
    public void UpdatePlayerInOpponentsList()
    {
        _opponentsList.RemoveAll(opponents => opponents == null || opponents == _currentPlayer);

        GameObject newPlayer = Spawner.Instance.Player;
        _currentPlayer = newPlayer;

        if (newPlayer != null && !_opponentsList.Contains(newPlayer))
        {
            _opponentsList.Add(newPlayer);
        }
    }
    public void SelectTargetOpponent()
    {
        float minDistance = 10000f;
        Transform target = null;

        foreach (var opponent in _opponentsList)
        {
            if (opponent == null) continue;
            if ((_thisBot.position - opponent.transform.position).magnitude < minDistance)
            {
                minDistance = (_thisBot.position - opponent.transform.position).magnitude;
                target = opponent.transform;
            }
        }
        if (target != null)
        {
            _targetOpponent = target;

        }
        else Debug.Log("SelectTargetPlayer: did not find target opponent");
    }
    public Transform GetCurrentOpponent()
    {
        return _targetOpponent;
    }
}
namespace Runtime.MonoBehaviours.Bot
{
    public class BotLogicExecuter : MonoBehaviour
    {
        private IBotNavigation _botNavigation;
        private ITargetOpponentSelector _targetOpponentFinder;

        private BotCharacter _character;
        private Dictionary<string, IState> _states;
        private ICharacterRuntimeData _characterData;
        private IState _currentState;
        private Vector3 _spawnedBombPos;
        private RetreatPositionFinder _retreatPositionFinder;

        public Dictionary<string, IState> States => _states;
        public BotCharacter Character => _character;
        public ICharacterRuntimeData CharacterData => _characterData;
        public IBotNavigation BotNavigation => _botNavigation;
        public ITargetOpponentSelector TargetOpponentFinder => _targetOpponentFinder;

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
            _botNavigation.SetTarget(_targetOpponentFinder.GetCurrentOpponent().position);
            _botNavigation.SetSpeed(3f);
        }
        private void OnDisable()
        {
            Spawner.Instance.OnBotSpawned -= _targetOpponentFinder.UpdateOpponentsList;
            Spawner.Instance.OnPlayerSpawned -= _targetOpponentFinder.UpdatePlayerInOpponentsList;
        }

        private void CollectRefs()
        {
            if (TryGetComponent(out BotCharacter character)) _character = character;

            _botNavigation = new BotNavigation(GetComponent<NavMeshAgent>());
            _retreatPositionFinder = new RetreatPositionFinder(GetComponent<NavMeshAgent>());
            _targetOpponentFinder = new TargetOpponentSelector(gameObject.transform);
            
            _characterData = _character.CharacterData;
            

            StatesSelector statesSelector = new StatesSelector();
            _states = statesSelector.GetStatesForType(BotType.Easy);
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
        }
    }
}