using Interfaces;
using MonoBehaviours.GroundSectionSystem;
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
                Debug.Log(target.name);
                _targetOpponent = target;
            }
            else Debug.Log("SelectTargetPlayer: did not find target opponent");
        }
    }
    public class StandartShelterFinder : IShelterFinder
    {
        private NavMeshAgent _agent;
        public StandartShelterFinder(NavMeshAgent agent)
        {
            _agent = agent;
        }
        public List<Vector3> FindAvailablePosForRetreat(List<Vector3> possiblePos, List<Vector3> blacklist)
        {
            throw new System.NotImplementedException();
        }

        public List<Vector3> GenerateBlacklistPositions(byte explosionRange, Vector3 bombPos)
        {
            throw new System.NotImplementedException();
        }

        public List<Vector3> GeneratePossiblePositions(byte explosionRange, List<Vector3> blacklist, Vector3 spawnedBombPos)
        {
            throw new System.NotImplementedException();
        }

        public void RetreatFromBomb(BotLogicExecuter bot)
        {
            throw new System.NotImplementedException();
        }
    }
}

