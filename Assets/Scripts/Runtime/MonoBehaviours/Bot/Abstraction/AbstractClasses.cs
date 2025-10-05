using Core.DataTransferObjects;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours;
using Runtime.MonoBehaviours.Bot;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AbstractClasses 
{
    public abstract class BaseShelterFinder
    {
        protected NavMeshAgent _agent;
        protected HashSet<Vector2Int> _blackListPos;
        
        public abstract void GenerateBlacklistPositions(BombDto bombDto);
        public abstract HashSet<Vector2Int> FindAvailablePosForRetreat(HashSet<Vector2Int> possiblePos, HashSet<Vector2Int> blacklist);
        public abstract void RetreatFromBomb(BotLogicExecuter bot);
        public bool PointInBlackList(HashSet<Vector2Int> blackList, Vector2Int point)
        {
            Vector2Int checkingPoint = point;

            if (blackList.Contains(checkingPoint)) return true;

            return false;
        }
        public HashSet<Vector2Int> GeneratePossiblePositions(byte explosionRange, HashSet<Vector2Int> blacklist, Vector3 spawnedBombPos)
        {
            var possiblePositions = new HashSet<Vector2Int>();
            Vector2Int bombPos = ConvertToVector2Int(spawnedBombPos);

            for (int x = -explosionRange - 1; x <= explosionRange + 1; x++)
            {
                for (int z = -explosionRange - 1; z <= explosionRange + 1; z++)
                {
                    Vector2Int point = new Vector2Int(bombPos.x + x, bombPos.y + z);

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
                return possiblePositions;
            }
        }
        protected Vector2Int ConvertToVector2Int(Vector3 vector)
        {
            return new Vector2Int(Mathf.FloorToInt(vector.x + 0.5f), Mathf.FloorToInt(vector.z + 0.5f));
        }
        public HashSet<Vector2Int> GetBlacklist() => _blackListPos;
    }
    public abstract class BaseBotNavigation
    {
        protected NavMeshAgent _agent;
        protected Vector3 _target;
        public Vector3 Target => _target;
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
        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }
        public abstract void CheckPathToTarget(Transform targetOpponent);
        public abstract Queue<GroundSection> GetPath();
    }
    public abstract class BaseTargetOpponentSelector
    {
        protected Transform _thisBot;
        protected Transform _targetOpponent;
        protected GameObject _currentPlayer;
        protected List<GameObject> _opponentsList;
        public Transform TargetOpponent => _targetOpponent;
        public void SetOpponentsList(List<GameObject> opponentsList)
        {
            _opponentsList = new List<GameObject>(opponentsList);
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
        public Transform GetCurrentOpponent() => _targetOpponent;
        public List<GameObject> GetOpponentsList() => _opponentsList;
        public abstract void SelectTargetOpponent();

    }
}

