using Core.DataTransferObjects;
using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot.StandartBotUtils
{
    public class StandartBotNavigation : IBotNavigation
    {
        private NavMeshAgent _agent;
        private List<GroundSection> _sections;
        private GroundSection[,] _sectionsPositions;
        private GroundSection _currentSection;
        private Vector3 _target;
        private Queue<GroundSection> _path;

        public StandartBotNavigation(NavMeshAgent agent)
        {
            _agent = agent;
            GroundSectionsUtils.Instance.GetCurrentSectionDataHolder().SetGroundSectionsCousts();
            _sections = GroundSectionsUtils.Instance.GetCurrentSectionDataHolder().sections;
            _sectionsPositions = new GroundSection[16, 16];
            CreateGrid();
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
            GridAPathFind(targetOpponent);
            GroundSection targetSection = null;
            foreach (var sectionFormPath in _path)
            {
                if (sectionFormPath.PlacedObstacle != null)
                {
                    targetSection = sectionFormPath;
                    break;
                }
                targetSection = sectionFormPath;
            }
            SetTarget(targetSection.transform.position);
        }

        public void SetSpeed(float speed)
        {
            _agent.speed = speed;
        }

        public void SetTarget(Vector3 target)
        {
            _agent.destination = target;
            _target = target;
        }
        private void GridAPathFind(Transform target)
        {
            int x = Mathf.FloorToInt(_agent.transform.position.x + 0.5f);
            int z = Mathf.FloorToInt(_agent.transform.position.z + 0.5f);

            int targetX = Mathf.FloorToInt(target.position.x + 0.5f);
            int targetZ = Mathf.FloorToInt(target.position.z + 0.5f);

            _currentSection = _sectionsPositions[x, z];
            GroundSection _goalSection = _sectionsPositions[targetX, targetZ];

            _path = new Queue<GroundSection>();
            _path = CalculatePath(_currentSection, _goalSection);
        }
        private Queue<GroundSection> CalculatePath(GroundSection startSection, GroundSection goalSection)
        {
            if (startSection == goalSection) // if bot is already on target section, return it
            {
                var queue = new Queue<GroundSection>();
                queue.Enqueue(startSection);
                return queue;
            }

            var path = new Queue<GroundSection>(); // path calculated by the algorithm
            var closeList = new HashSet<GroundSection>(); // closed list, sections that have had all neighbors checked
            var openList = new Dictionary<GroundSection, float>(); // open list, sections waiting for neighbor checking
            var parents = new Dictionary<GroundSection, GroundSection>(); // dictionary with section (key) and its parent (value). Needed to backtrack the path
            var sectionCost = new Dictionary<GroundSection, int>(); // section costs, total path cost to reach each section

            float distance = (startSection.transform.position - goalSection.transform.position).magnitude;

            openList.Add(startSection, distance);
            sectionCost.Add(startSection, 0);

            GroundSection currentSection = startSection;

            while (openList.Count > 0)
            {
                List<GroundSection> neighbours = new List<GroundSection>(); // neighbors of current section being checked

                // TODO: instead of checking, we could add a list of connected sections to the section itself
                if (currentSection.ConnectedSections.rightSection != null) neighbours.Add(currentSection.ConnectedSections.rightSection);
                if (currentSection.ConnectedSections.leftSection != null) neighbours.Add(currentSection.ConnectedSections.leftSection);
                if (currentSection.ConnectedSections.upperSection != null) neighbours.Add(currentSection.ConnectedSections.upperSection);
                if (currentSection.ConnectedSections.lowerSection != null) neighbours.Add(currentSection.ConnectedSections.lowerSection);

                if (currentSection == goalSection)
                {
                    GroundSection neighbourOfGoalSection = null;
                    foreach (GroundSection neighbour in neighbours)
                    {
                        if (closeList.Contains(neighbour))
                        {
                            neighbourOfGoalSection = neighbour;
                        }
                    }
                    parents[currentSection] = neighbourOfGoalSection;
                    break;
                }

                foreach (GroundSection neighbor in neighbours) // process all neighbors of current section and add them to open list
                {
                    if (neighbor == null || closeList.Contains(neighbor)) continue;

                    //-----------------------CALCULATE tentetiveG-----------------------------
                    int tentetiveG = sectionCost[currentSection] + neighbor.cost;

                    // if we encounter neighbor for the first time, add it to G list
                    if (!sectionCost.ContainsKey(neighbor))
                    {
                        sectionCost.Add(neighbor, sectionCost[currentSection] + neighbor.cost);
                        parents.Add(neighbor, currentSection);
                    }

                    // if we've already checked this neighbor and found a better cost, update with the better cost
                    if (sectionCost.ContainsKey(neighbor) && sectionCost[neighbor] > tentetiveG)
                    {
                        sectionCost[neighbor] = tentetiveG;
                        parents[neighbor] = currentSection;
                    }

                    //-----------------------------------------------------------------------
                    //-----------------------CALCULATE neighbourF------------------------------

                    float h = (neighbor.transform.position - goalSection.transform.position).magnitude;
                    // neighbor's path length equals its section cost + current cost
                    float neighborF = sectionCost[neighbor] + h;

                    // IMPORTANT! if neighbor's F is greater than current F, we assign the minimum. This finds the shortest path
                    if (openList.ContainsKey(neighbor) && openList[neighbor] > neighborF)
                    {
                        openList[neighbor] = neighborF;
                        continue; // break to avoid adding neighbor to open list again
                    }
                    //-----------------------------------------------------------------------
                    // if neighbor hasn't been checked before, add it to open list
                    if (!openList.ContainsKey(neighbor)) openList.Add(neighbor, neighborF);
                }
                // after checking all neighbors, add processed section to closed list
                closeList.Add(currentSection);
                openList.Remove(currentSection);

                GroundSection nextSection = null;
                float currentMinF = 10000;
                foreach (KeyValuePair<GroundSection, float> section in openList) // calculate next section to check its neighbors
                {
                    if (section.Value < currentMinF)
                    {
                        currentMinF = section.Value;
                        nextSection = section.Key;
                    }
                }
                currentSection = nextSection;
                neighbours.Clear();
            }
            //----building a path--------
            currentSection = goalSection;
            while (currentSection != startSection)
            {
                path.Enqueue(currentSection);
                currentSection = parents[currentSection];
            }
            path.Enqueue(startSection);
            //-----End bulding path------

            return new Queue<GroundSection>(path.Reverse());
        }

        private void CreateGrid()
        {
            foreach(var section in _sections)
            {
                int x = Mathf.FloorToInt(section.transform.position.x);
                int z = Mathf.FloorToInt(section.transform.position.z);

                _sectionsPositions[x,z] = section;
            }
        }
    }
    public class StandartTargetOpponentSelector : BaseTargetOpponentSelector
    {
        public StandartTargetOpponentSelector(NavMeshAgent agent)
        {
            _thisBot = agent.transform;
        }
        public override void SelectTargetOpponent()
        {
            float minDistance = 1000000f;
            Transform target = null;
            foreach (var opponent in _opponentsList)
            {
                if (opponent == null) continue;
                float distance = (_thisBot.position - opponent.transform.position).sqrMagnitude;
                if (distance < minDistance)
                {
                    minDistance = distance;
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
    public class StandartShelterFinder : IShelterFinder
    {
        private NavMeshAgent _agent;
        private BotLogicExecuter _botLogicExecuter;
        private List<Vector3> _blackListPos;

        public StandartShelterFinder(NavMeshAgent agent)
        {
            _agent = agent;
            _botLogicExecuter = agent.gameObject.GetComponent<BotLogicExecuter>();
            _blackListPos = new List<Vector3>();

            if (_botLogicExecuter == null) Debug.LogError("StandartShelterFinder: did not find BotLogicExecuter");
            if (agent == null) Debug.LogError("StandartShelterFinder: did not find NavMeshAgent");

            SubcribeToEvents();
        }
        private void SubcribeToEvents()
        {            
            var list = new List<GameObject>(Spawner.Instance.OpponentsList);
            list.Add(_agent.gameObject);
            foreach (GameObject opponent in list)
            {
                if (opponent == null) continue;
                if (opponent.TryGetComponent(out BotCharacter character))
                {
                    character.OnBombDeployed += GenerateBlacklistPositions;
                }
                else if (opponent.TryGetComponent(out PlayerCharacter playerCharacter))
                {
                    playerCharacter.OnBombDeployed += GenerateBlacklistPositions;
                }
                else Debug.Log("SubscribeToEvents: Неизвестный оппонент");
            }
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
            _botLogicExecuter.StartCoroutine(GeneratorBlacklistPositions(bombDto));
            
        }     
        private IEnumerator GeneratorBlacklistPositions(BombDto bombDto)
        {
            Vector3 bombPos = bombDto.BombPosition;
            _blackListPos.Add(bombPos);
            for (int i = 1; i <= bombDto.BombsSpreading; i++)
            {
                _blackListPos.Add(bombPos + new Vector3(i, 0, 0));
                _blackListPos.Add(bombPos + new Vector3(-i, 0, 0));
                _blackListPos.Add(bombPos + new Vector3(0, 0, i));
                _blackListPos.Add(bombPos + new Vector3(0, 0, -i));
            }

            yield return new WaitForSeconds(bombDto.BombCountdown);

            _blackListPos.Remove(bombPos);
            for (int i = 1; i <= bombDto.BombsSpreading; i++)
            {
                _blackListPos.Remove(bombPos + new Vector3(i, 0, 0));
                _blackListPos.Remove(bombPos + new Vector3(-i, 0, 0));
                _blackListPos.Remove(bombPos + new Vector3(0, 0, i));
                _blackListPos.Remove(bombPos + new Vector3(0, 0, -i));
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

                    if (!PointInBlackList(blacklist, point)) possiblePositions.Add(point);
                }
            }
            if (possiblePositions.Count > 0) return possiblePositions;
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
            if (availablePositions.Count > 0) return availablePositions;
            else
            {
                Debug.Log(_agent.gameObject.name + " FindAvailablePosForRetreat: did not find available positions for retreat");
                return availablePositions;
            }
        }
        public void RetreatFromBomb(BotLogicExecuter bot)
        {
            var possiblePositions = GeneratePossiblePositions((byte)bot.Character.BombDto.BombsSpreading, _blackListPos, bot.Character.BombDto.BombPosition);
            var availablePositions = FindAvailablePosForRetreat(possiblePositions, _blackListPos);

            if (availablePositions == null || availablePositions.Count == 0)
            {
                Debug.Log("RetreatFromBomb: No available positions for retreat, staying in place.");
                bot.BotNavigation.SetTarget(bot.transform.position);
                return;
            }
            else
            {
                Vector3 target = Vector3.zero;
                float closestDist = float.MaxValue;
                for (int i = 0; i < availablePositions.Count; i++)
                {
                    float currentDist = (availablePositions[i] - _agent.transform.position).sqrMagnitude;
                    if (currentDist < closestDist)
                    {
                        closestDist = currentDist;
                        target = availablePositions[i];
                    }
                }                
                bot.BotNavigation.SetTarget(target);
            }
        }
    }
}

