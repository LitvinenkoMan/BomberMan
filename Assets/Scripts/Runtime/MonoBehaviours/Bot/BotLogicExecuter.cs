using Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;
using static UnityEditor.PlayerSettings;

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

        public Transform TargetOpponent => _targetOpponent;
        public Vector3 Target => _target;
        public Dictionary<string, IState> States => _states;
        public BotCharacter Character => _character;
        public ICharacterRuntimeData CharacterData => _characterData;

        List<Vector3> possiblePos = new List<Vector3>();
        List<Vector3> availablePos = new List<Vector3>();

        private void Awake()
        {
            CollectRefs();
        }
        
        private void Start()
        {
            Spawner.Instance.OnBotSpawned += UpdateOpponentsList;
            Spawner.Instance.OnPlayerSpawned += UpdatePlayerInList;
            _character.PlayerBombDeployer.BombSpawned += SetBombPosition;

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
            _character.PlayerBombDeployer.BombSpawned -= SetBombPosition;
        }

        private void OnDrawGizmos()
        {            
            //Gizmos.DrawSphere(_targetOpponent.position, 0.5f);
            //if (possiblePos != null)
            //{
            //    foreach (var point in possiblePos)
            //    {
            //        Gizmos.color = UnityEngine.Color.red;
            //        Gizmos.DrawSphere(point + new Vector3(0, 1, 0), 0.3f);
            //    }
            //}
            //if (availablePos != null)
            //{
            //    foreach (var point in availablePos)
            //    {
            //        Gizmos.color = UnityEngine.Color.green;
            //        Gizmos.DrawSphere(point + new Vector3(0, 1, 0), 0.3f);
            //    }
            //}
        }

        private void CollectRefs()
        {
            if (TryGetComponent(out BotCharacter character)) _character = character;
            if (TryGetComponent(out NavMeshAgent agent)) _agent = agent;

            
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
                    Debug.Log("Нет углов по пути");
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

        private bool PointInBlackList(List<Vector3> blackList, Vector3 point)
        {
            Vector3 checkingPoint = new Vector3(point.x, 0, point.z);

            foreach(Vector3 blackListPoint in blackList)
            {
                Vector3 blackPoint = new Vector3(blackListPoint.x, 0, blackListPoint.z);

                if (checkingPoint == blackPoint) return true;
            }
            return false;
        }

        private List<Vector3> GenerateBlacklistPositions(byte explosionRange, Vector3 bombPos)
        {
            var blacklist = new List<Vector3> { bombPos };
            for (int i = 1; i <= explosionRange; i++)
            {
                blacklist.Add(bombPos + new Vector3(i, 0, 0));
                blacklist.Add(bombPos + new Vector3(-i, 0, 0));
                blacklist.Add(bombPos + new Vector3(0, 0, i));
                blacklist.Add(bombPos + new Vector3(0, 0, -i));
            }
            return blacklist;
        }

        private List<Vector3> GeneratePossiblePositions(byte explosionRange, List<Vector3> blacklist)
        {
            var possiblePositions = new List<Vector3>();
            float centerX = _spawnedBombPos.x;
            float centerZ = _spawnedBombPos.z;

            for (int x = -explosionRange - 1; x <= explosionRange + 1; x++)
            {
                for (int z = -explosionRange - 1; z <= explosionRange + 1; z++)
                {
                    Vector3 point = new Vector3(centerX + x, 0, centerZ + z);

                    if (!PointInBlackList(blacklist, point))
                    {
                        possiblePositions.Add(point);
                    }
                }
            }
            if (possiblePositions.Count > 0)
            {
                return possiblePositions;
            }
            else
            {
                Debug.LogError("GeneratePossiblePositions: list of possible positions is null");
                return null;
            }
        }
        private List<Vector3> FindAvailablePosForRetreat(List<Vector3> possiblePos, List<Vector3> blacklist)
        {
            if (possiblePos.Count == 0)
            {
                Debug.LogError(gameObject.name + " FindAvailablePosForRetreat: parametr 'possiblePos' is null");
                return null;
            }
            Vector3[] sideOffsets = { new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };
            var availablePositions = new List<Vector3>();

            foreach (Vector3 point in possiblePos)
            {
                if (NavMesh.SamplePosition(point, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    _agent.CalculatePath(hit.position, path);

                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        availablePositions.Add(hit.position);

                        // проверяем боковую секцию чтобы сразу уйти с поля поражения бомбы
                        foreach (var offset in sideOffsets)
                        {
                            var sidePos = hit.position + offset;
                            _agent.CalculatePath(sidePos, path);
                            if (path.status == NavMeshPathStatus.PathComplete && !PointInBlackList(blacklist, sidePos))
                            {
                                availablePositions.Add(sidePos);
                            }
                        }
                    }
                    else continue;
                }
            }
            if (availablePositions.Count > 0)
            {
                return availablePositions;
            }
            else
            {
                Debug.LogError(gameObject.name + " FindAvailablePosForRetreat: did not find available positions for retreat");
                return null;
            }
        }

        public void RetreatFromBomb()
        {
            byte explosionRange = (byte)_characterData.BombsSpreading;

            var blacklistPositions = GenerateBlacklistPositions(explosionRange, _spawnedBombPos);
            var possiblePositions = GeneratePossiblePositions(explosionRange, blacklistPositions);
            var availablePositions = FindAvailablePosForRetreat(possiblePositions, blacklistPositions);

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

            availablePos = availablePositions;
            possiblePos = possiblePositions;
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