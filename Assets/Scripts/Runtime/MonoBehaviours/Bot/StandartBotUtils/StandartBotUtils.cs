using Core.DataTransferObjects;
using Interfaces;
using AbstractClasses;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours.Player;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot.StandartBotUtils
{
    public class StandartBotNavigation : BaseBotNavigation
    {
        private readonly List<GroundSection> _sections;
        private readonly GroundSection[,] _sectionsPositions;
        private GroundSection _currentSection;
        private Queue<GroundSection> _path;

        public override Queue<GroundSection> GetPath()
        {
            return _path;
        }

        public StandartBotNavigation(NavMeshAgent agent)
        {
            _agent = agent;
            GroundSectionsUtils.Instance.GetCurrentSectionDataHolder().SetGroundSectionsCousts();
            _sections = GroundSectionsUtils.Instance.GetCurrentSectionDataHolder().sections;
            _sectionsPositions = new GroundSection[16, 16];
            CreateGrid();
        }
        public override void CheckPathToTarget(Transform targetOpponent)
        {
            GridAPathFind(targetOpponent);
            GroundSection targetSection = null;
            foreach (var sectionFormPath in _path)
            {
                _path.Dequeue();
                if (sectionFormPath.PlacedObstacle != null)
                {
                    targetSection = sectionFormPath;
                    break;
                }
                targetSection = sectionFormPath;
            }
            SetTarget(targetSection.transform.position);
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
    public class StandartShelterFinder : BaseShelterFinder
    {
        private readonly BotLogicExecuter _botLogicExecuter;

        public StandartShelterFinder(NavMeshAgent agent)
        {
            _agent = agent;
            _botLogicExecuter = agent.gameObject.GetComponent<BotLogicExecuter>();
            _blackListPos = new HashSet<Vector2Int>();

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

        public override void GenerateBlacklistPositions(BombDto bombDto)
        {
            _botLogicExecuter.StartCoroutine(GeneratorBlacklistPositions(bombDto));
        }
        private IEnumerator GeneratorBlacklistPositions(BombDto bombDto)
        {
            Vector2Int bombPos = ConvertToVector2Int(bombDto.BombPosition);

            var bombs = new HashSet<Vector2Int>() { bombPos };

            for (int i = 1; i <= bombDto.BombsSpreading; i++)
            {
                bombs.Add(bombPos + new Vector2Int(i, 0));
                bombs.Add(bombPos + new Vector2Int(-i, 0));
                bombs.Add(bombPos + new Vector2Int(0, i));
                bombs.Add(bombPos + new Vector2Int(0, -i));
            }
            _blackListPos.UnionWith(bombs); 

            yield return new WaitForSeconds(bombDto.BombCountdown);

            _blackListPos.ExceptWith(bombs); 
        }
        public override HashSet<Vector2Int> FindAvailablePosForRetreat(HashSet<Vector2Int> possiblePos, HashSet<Vector2Int> blacklist)
        {
            if (possiblePos.Count == 0)
            {
                Debug.LogError(_agent.gameObject.name + " FindAvailablePosForRetreat: parametr 'possiblePos' is null");
                return null;
            }
            var availablePositions = new HashSet<Vector2Int>();

            foreach (Vector2Int point in possiblePos)
            {
                if (NavMesh.SamplePosition(new Vector3(point.x, 0, point.y), out NavMeshHit hit, 0.5f, NavMesh.AllAreas))
                {
                    NavMeshPath path = new NavMeshPath();
                    Vector2Int hitVector2Int = ConvertToVector2Int(hit.position);
                    _agent.CalculatePath(hit.position, path);

                    if (path.status == NavMeshPathStatus.PathComplete && !PointInBlackList(blacklist, point)) availablePositions.Add(hitVector2Int);
                }
            }
            if (availablePositions.Count > 0) return availablePositions;
            else
            {
                Debug.LogError(_agent.gameObject.name + " FindAvailablePosForRetreat: did not find available positions for retreat");
                return availablePositions;
            }
        }
        public override void RetreatFromBomb(BotLogicExecuter bot)
        {
            var possiblePositions = GeneratePossiblePositions((byte)bot.Character.BombDto.BombsSpreading, _blackListPos, bot.Character.BombDto.BombPosition);
            var availablePositions = FindAvailablePosForRetreat(possiblePositions, _blackListPos);

            if (availablePositions == null || availablePositions.Count == 0)
            {
                Debug.Log("RetreatFromBomb: No available positions for retreat, staying in place.");
                bot.BotNavigation.SetTarget(bot.transform.position);
            }
            else
            {
                Vector3 target = Vector3.zero;
                float closestDist = float.MaxValue;
                foreach (var position in availablePositions) 
                {
                    Vector3 position3D = new Vector3(position.x, 0, position.y); 
                    float currentDist = (position3D - _agent.transform.position).sqrMagnitude;

                    if (currentDist < closestDist)
                    {
                        closestDist = currentDist;
                        target = position3D;
                    }
                }
                bot.BotNavigation.SetTarget(target);
            }
        }
    }
}

