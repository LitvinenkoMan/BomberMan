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

namespace Runtime.MonoBehaviours.Bot
{
    public class BotLogicExecuter : MonoBehaviour
    {
        private BotType _botType;
        private BotCharacter _character;
        private NavMeshAgent _agent;
        private Transform _player;
        private Vector3 _target;
        private Dictionary<string, IState> _states;
        private ICharacterRuntimeData _characterData;
        private IState _currentState;
        private Vector3 _spawnedBombPos;


        public Vector3 Target => _target;
        public Dictionary<string, IState> States => _states;
        public BotCharacter Character => _character;
        public ICharacterRuntimeData CharacterData => _characterData;

        List<Vector3> possiblePositions;
        List<Vector3> availablePositions;

        private void Awake()
        {
            CollectRefs();
            SwitchState(_states["Agro"]);
            SetTarget(_player.position);
            possiblePositions = new List<Vector3>();
            availablePositions = new List<Vector3>();
        }
        private void Start()
        {
            _character.concreteBombDeployer.BombSpawned += SetBombPosition;
        }
        private void OnDisable()
        {
            _character.concreteBombDeployer.BombSpawned -= SetBombPosition;
        }

        private void OnDrawGizmos()
        {
            if (possiblePositions != null)
            {
                foreach (var point in possiblePositions)
                {
                    Gizmos.color = UnityEngine.Color.red;
                    Gizmos.DrawSphere(point + new Vector3(0, 1, 0), 0.3f);
                }
            }
            if (availablePositions != null)
            {
                foreach (var point in availablePositions)
                {
                    Gizmos.color = UnityEngine.Color.green;
                    Gizmos.DrawSphere(point + new Vector3(0, 1, 0), 0.3f);
                }
            }
        }

        private void CollectRefs()
        {
            if (TryGetComponent(out BotCharacter character)) _character = character;
            if (TryGetComponent(out NavMeshAgent agent)) _agent = agent;

            _characterData = _character.CharacterRuntimeData;
            _player = Spawner.Instance.Player.transform;
            StatesSelector statesSelector = new StatesSelector();
            _states = statesSelector.GetStatesForType(BotType.Easy);

            switch (_botType)
            {
                case BotType.Easy:
                    _botType = BotType.Easy;
                    break;
            }
        }
        public float CheckDistance()
        {
            if (_target == null)
            {
                Debug.Log("target = null");
                return 0f;
            }
            return (transform.position - _target).magnitude;
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
            _agent.CalculatePath(_target, path);
            if (path.status == NavMeshPathStatus.PathComplete)
            {
                SetTarget(_target);
            }
            else
            {
                SetTarget(path.corners[path.corners.Length - 1]);
            }
        }

        private void SetAvailablePos(List<Vector3> pos)
        {
            availablePositions = null;
            availablePositions = pos;
        }

        private bool PointInArea(Vector3 point, float range)
        {
            if ((point - _spawnedBombPos).magnitude > Mathf.Pow(range * 2 - 2, 0.5f))
            {
                return false;
            }
            return true;
        }

        public void RetreatFromBomb()
        {
            possiblePositions.Clear();


            float centerX = _spawnedBombPos.x;
            float centerZ = _spawnedBombPos.z;
            int range = _characterData.BombsSpreading + 1;

            var availablePos = new List<Vector3>();
            var blackListPositions = new List<Vector3>();

            for (int x = -range; x <= range; x++)
            {
                for (int z = -range; z <= range; z++)
                {
                    Vector3 point = new Vector3(centerX + x, transform.position.y, centerZ + z);

                    if (!PointInArea(point, range))
                    {
                        possiblePositions.Add(point);
                    }
                }
            }

            foreach (Vector3 point in possiblePositions)
            {
                if (NavMesh.SamplePosition(point, out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    _agent.CalculatePath(hit.position, path);

                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        availablePos.Add(hit.position);

                        // проверяем боковую секцию чтобы сразу уйти с поля поражения бомбы
                        _agent.CalculatePath(hit.position + new Vector3(1, 0, 0), path);
                        if (path.status == NavMeshPathStatus.PathComplete && !PointInArea(hit.position + new Vector3(1, 0, 0), range))
                        {
                            availablePos.Add(hit.position + new Vector3(1, 0, 0));
                        }

                        // проверяем боковую секцию чтобы сразу уйти с поля поражения бомбы
                        _agent.CalculatePath(hit.position - new Vector3(1, 0, 0), path);
                        if (path.status == NavMeshPathStatus.PathComplete && !PointInArea(hit.position - new Vector3(1, 0, 0), range))
                        {
                            availablePos.Add(hit.position - new Vector3(1, 0, 0));
                        }
                    }
                    else continue;
                }
            }
            int randInt = UnityEngine.Random.Range(0, availablePos.Count);
            SetTarget(availablePos[randInt]);
            SetAvailablePos(availablePos);
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


