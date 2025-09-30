using Core.DataTransferObjects;
using Interfaces;
using Runtime.MonoBehaviours;
using Runtime.MonoBehaviours.Bot;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


namespace Interfaces
{
    public interface IShelterFinder
    {
        public void GenerateBlacklistPositions(BombDto bombDto);
        public List<Vector3> GeneratePossiblePositions(byte explosionRange, List<Vector3> blacklist, Vector3 spawnedBombPos);
        public List<Vector3> FindAvailablePosForRetreat(List<Vector3> possiblePos, List<Vector3> blacklist);
        public void RetreatFromBomb(BotLogicExecuter bot);
    }
    public interface IBotNavigation
    {
        public void SetTarget(Vector3 target);
        public float CheckDistance(Transform targetOpponent);
        public void CheckPathToTarget(Transform targetOpponent);
        public void SetSpeed(float speed);
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
        private NavMeshAgent _agent;
        private List<Vector3> _blackListPos;

        public SimpleShelterFinder(NavMeshAgent agent)
        {
            _agent = agent;
            _blackListPos = new List<Vector3>();
        }
        private bool PointInBlackList(List<Vector3> blackList, Vector3 point)
        {
            Vector3 checkingPoint = new Vector3(point.x, 0, point.z);

            foreach (Vector3 blackListPoint in blackList)
            {
                Vector3 blackPoint = new Vector3(blackListPoint.x, 0, blackListPoint.z);

                if (checkingPoint == blackPoint) return true;
            }
            return false;
        }

        public void GenerateBlacklistPositions(BombDto bombDto)
        {
            Vector3 bombPos = bombDto.BombPosition;
            _blackListPos.Clear();
            _blackListPos.Add(bombPos);
            for (int i = 1; i <= bombDto.BombsSpreading; i++)
            {
                _blackListPos.Add(bombPos + new Vector3(i, 0, 0));
                _blackListPos.Add(bombPos + new Vector3(-i, 0, 0));
                _blackListPos.Add(bombPos + new Vector3(0, 0, i));
                _blackListPos.Add(bombPos + new Vector3(0, 0, -i));
            }
        }

        public List<Vector3> GeneratePossiblePositions(byte explosionRange, List<Vector3> blacklist, Vector3 spawnedBombPos)
        {
            var possiblePositions = new List<Vector3>();
            float centerX = spawnedBombPos.x;
            float centerZ = spawnedBombPos.z;

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
                return possiblePositions;
            }
        }
        public List<Vector3> FindAvailablePosForRetreat(List<Vector3> possiblePos, List<Vector3> blacklist)
        {
            if (possiblePos.Count == 0)
            {
                Debug.LogError(_agent.gameObject.name + " FindAvailablePosForRetreat: parametr 'possiblePos' is null");
                return null;
            }
            Vector3[] sideOffsets = { new Vector3(1, 0, 0), new Vector3(-1, 0, 0) };
            var availablePositions = new List<Vector3>();

            foreach (Vector3 point in possiblePos)
            {
                NavMeshPath path = new NavMeshPath();
                _agent.CalculatePath(point, path);

                if (path.status == NavMeshPathStatus.PathComplete)
                {
                    availablePositions.Add(point);

                    // проверяем боковую секцию чтобы сразу уйти с поля поражения бомбы
                    foreach (var offset in sideOffsets)
                    {
                        var sidePos = point + offset;
                        _agent.CalculatePath(sidePos, path);
                        if (path.status == NavMeshPathStatus.PathComplete && !PointInBlackList(blacklist, sidePos))
                        {
                            availablePositions.Add(sidePos);
                        }
                    }
                }
                else continue;
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
                return;
            }
            else
            {
                int randInt = UnityEngine.Random.Range(0, availablePositions.Count);
                bot.BotNavigation.SetTarget(availablePositions[randInt]);
            }
        }
    }
}
