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
        public GroundSection _currentSection;
        private GroundSection _target;
        private Queue<GroundSection> path;
        public GroundSection Target => _target;

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

            return Mathf.Min((_agent.transform.position - _target.transform.position).magnitude,
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
                // метод нахождения кратчайшего пути до цели Grid A*
            }
        }

        public void SetSpeed(float speed)
        {
        }

        public void SetTarget(Vector3 target)
        {
            GridAPathFind();
        }
        private void GridAPathFind()
        {
            int x = Mathf.FloorToInt(_agent.transform.position.x + 0.5f);
            int z = Mathf.FloorToInt(_agent.transform.position.z + 0.5f);

            _currentSection = _sectionsPositions[x, z];
            GroundSection _goalSection = _sectionsPositions[1, 10];
            path = new Queue<GroundSection>();
            path = CalculateNextSection(_currentSection, _goalSection);
        }
        public Queue<GroundSection> DebugingGetList()
        {
            return path;
        }
        private Queue<GroundSection> CalculateNextSection(GroundSection startSection, GroundSection goalSection)
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

            openList.Add(startSection, (startSection.transform.position - goalSection.transform.position).magnitude);
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

                    // neighbor's path length equals its section cost + current cost
                    float neighborF = sectionCost[neighbor] + (neighbor.transform.position - goalSection.transform.position).magnitude;

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
    public class StandartTargetOpponentSelector : ITargetOpponentSelector
    {
        public StandartTargetOpponentSelector(NavMeshAgent agent) 
        {
            
        }
        public Transform GetCurrentOpponent()
        {
            return null;
        }

        public void SelectTargetOpponent()
        {
        }

        public void SetOpponentsList(List<GameObject> list)
        {
        }

        public void UpdateOpponentsList(string name)
        {
        }

        public void UpdatePlayerInOpponentsList()
        {
        }
    }
}

