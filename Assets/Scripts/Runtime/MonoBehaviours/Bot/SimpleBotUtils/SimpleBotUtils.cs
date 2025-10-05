using Core.DataTransferObjects;
using AbstractClasses;
using MonoBehaviours.GroundSectionSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot.SimpleBotUtils
{
    public class SimpleBotNavigation : BaseBotNavigation
    {
        public SimpleBotNavigation(NavMeshAgent agent)
        {
            _agent = agent;            
        }
        public override void CheckPathToTarget(Transform targetOpponent)
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
        public override Queue<GroundSection> GetPath()
        {
            return null;
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
    public class SimpleShelterFinder : BaseShelterFinder
    {
        public SimpleShelterFinder(NavMeshAgent agent)
        {
            _agent = agent;
            _blackListPos = new HashSet<Vector2Int>();
        }
        public override void GenerateBlacklistPositions(BombDto bombDto)
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
        public override HashSet<Vector2Int> FindAvailablePosForRetreat(HashSet<Vector2Int> possiblePos, HashSet<Vector2Int> blacklist)
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
        public override void RetreatFromBomb(BotLogicExecuter bot)
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
    }
}
