using Core.DataTransferObjects;
using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours;
using Runtime.MonoBehaviours.Bot;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;


namespace Interfaces
{
    public interface IShelterFinder
    {
        public void GenerateBlacklistPositions(BombDto bombDto);
        public HashSet<Vector2Int> GeneratePossiblePositions(byte explosionRange, HashSet<Vector2Int> blacklist, Vector3 spawnedBombPos);
        public HashSet<Vector2Int> FindAvailablePosForRetreat(HashSet<Vector2Int> possiblePos, HashSet<Vector2Int> blacklist);
        public void RetreatFromBomb(BotLogicExecuter bot);
        /*--------For Debugging---------*/
        public HashSet<Vector2Int> GetBlacklist();
        //--------------------------------
        public static Vector2Int ConvertToVector2Int(Vector3 vector)
        {
            return new Vector2Int(Mathf.RoundToInt(vector.x), Mathf.RoundToInt(vector.z));
        }
    }
    public interface IBotNavigation
    {
        public void SetTarget(Vector3 target);
        public float CheckDistance(Transform targetOpponent);
        public void CheckPathToTarget(Transform targetOpponent);
        public void SetSpeed(float speed);
        public Queue<GroundSection> GetPath();
    }
    
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

namespace Runtime.MonoBehaviours.Bot.SimpleBotUtils
{
    public class SimpleBotNavigation : IBotNavigation
    {
        private NavMeshAgent _agent;
        private Vector3 _target;
        public Vector3 Target => _target;

        public SimpleBotNavigation(NavMeshAgent agent)
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

        public Queue<GroundSection> GetPath()
        {
            throw new NotImplementedException();
        }
    }
    public class SimpleTargetOpponentSelector : BaseTargetOpponentSelector
    {
        public SimpleTargetOpponentSelector(NavMeshAgent agent)
        {
            _thisBot = agent.transform;
        }
        public override void SelectTargetOpponent()
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
    }
    public class SimpleShelterFinder : IShelterFinder
    {
        private readonly NavMeshAgent _agent;
        private readonly HashSet<Vector2Int> _blackListPos;

        public SimpleShelterFinder(NavMeshAgent agent)
        {
            _agent = agent;
            _blackListPos = new HashSet<Vector2Int>();
        }
        private bool PointInBlackList(HashSet<Vector2Int> blackList, Vector2Int point)
        {
            Vector2Int checkingPoint = point;

            if (blackList.Contains(checkingPoint)) return true;

            return false;
        }

        public void GenerateBlacklistPositions(BombDto bombDto)
        {
            Vector2Int bombPos = ConvertToVector2Int(bombDto.BombPosition);
            _blackListPos.Clear();
            _blackListPos.Add(bombPos);
            for (int i = 1; i <= bombDto.BombsSpreading; i++)
            {
                _blackListPos.Add(bombPos + new Vector2Int(i, 0));
                _blackListPos.Add(bombPos + new Vector2Int(-i, 0));
                _blackListPos.Add(bombPos + new Vector2Int(0, i));
                _blackListPos.Add(bombPos + new Vector2Int(0, -i));
            }
        }

        public static Vector2Int ConvertToVector2Int(Vector3 vector)
        {
            return new Vector2Int(Mathf.FloorToInt(vector.x + 0.5f), Mathf.FloorToInt(vector.z + 0.5f));
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
        public HashSet<Vector2Int> FindAvailablePosForRetreat(HashSet<Vector2Int> possiblePos, HashSet<Vector2Int> blacklist)
        {
            if (possiblePos.Count == 0)
            {
                Debug.LogError(_agent.gameObject.name + " FindAvailablePosForRetreat: parametr 'possiblePos' is null");
                return null;
            }
            Vector2Int[] sideOffsets = { new Vector2Int(1, 0), new Vector2Int(-1, 0) };
            var availablePositions = new HashSet<Vector2Int>();

            foreach (Vector2Int point in possiblePos)
            {
                if (NavMesh.SamplePosition(new Vector3(point.x, 0, point.y), out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    Vector2Int hitVector2Int = ConvertToVector2Int(hit.position);
                    _agent.CalculatePath(hit.position, path);

                    if (path.status == NavMeshPathStatus.PathComplete)
                    {
                        availablePositions.Add(hitVector2Int);

                        // проверяем боковую секцию чтобы сразу уйти с поля поражения бомбы
                        foreach (var offset in sideOffsets)
                        {
                            Vector2Int sidePos = hitVector2Int + offset;
                            _agent.CalculatePath(new Vector3(sidePos.x, 0, sidePos.y), path);
                            if (path.status == NavMeshPathStatus.PathComplete && !PointInBlackList(blacklist, sidePos))
                            {
                                availablePositions.Add(sidePos);
                            }
                        }
                    }
                }
            }
            if (availablePositions.Count > 0)
            {
                return availablePositions;
            }
            else
            {
                Debug.LogError(_agent.gameObject.name + " FindAvailablePosForRetreat: did not find available positions for retreat");
                return availablePositions;
            }
        }
        public void RetreatFromBomb(BotLogicExecuter bot)
        {
            byte explosionRange = (byte)bot.CharacterData.BombsSpreading;

            GenerateBlacklistPositions(bot.Character.BombDto);
            var possiblePositions = GeneratePossiblePositions(explosionRange, _blackListPos, bot.Character.BombDto.BombPosition);
            var availablePositions = FindAvailablePosForRetreat(possiblePositions, _blackListPos);

            if (availablePositions == null || availablePositions.Count == 0)
            {
                Debug.Log("No available positions for retreat, staying in place.");
                bot.BotNavigation.SetTarget(bot.transform.position);
            }
            else
            {
                int randInt = UnityEngine.Random.Range(0, availablePositions.Count);
                Vector2Int randomPos = availablePositions.ElementAt(randInt);
                bot.BotNavigation.SetTarget(new Vector3(randomPos.x, 0, randomPos.y));
            }
        }

        public HashSet<Vector2Int> GetBlacklist()
        {
            return _blackListPos;
        }   
    }
}
