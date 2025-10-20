using AbstractClasses;
using Core.DataTransferObjects;
using MonoBehaviours.GroundSectionSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace Runtime.MonoBehaviours.Bot.StandartBotUtils
{
    public class StandartBotNavigation : BaseBotNavigation
    {
        
        private GroundSection _currentSection;
        private Queue<GroundSection> _path;
        private List<Canvas> canvas;
        private HashSet<GroundSection> _dangerSection = new HashSet<GroundSection>();
        
        public override Queue<GroundSection> GetPath()
        {
            return _path;
        }

        public StandartBotNavigation(NavMeshAgent agent, List<Canvas> canvas)
        {
            _agent = agent;
            
            this.canvas = canvas;
        }
        public override void CheckPathToTarget(Transform targetOpponent)
        {
            AStarPathFind(targetOpponent);

            SelectTargetAccordingToPath();
        }
        private void SelectTargetAccordingToPath()
        {
            var x = _agent.transform.position.x;
            var y = _agent.transform.position.y;
            GroundSection targetSection = BombPositions.SectionsPositions[Mathf.RoundToInt(x), Mathf.RoundToInt(y)];
            List<GroundSection> checkedPath = new List<GroundSection>();

            foreach (var sectionFromPath in _path)
            {
                if (_dangerSection.Contains(sectionFromPath))
                {
                    Debug.Log("Опасная зона");
                    SetTarget(targetSection.transform.position);
                    StayInTarget = 0.3f;
                    return;
                }
                
                if (sectionFromPath.PlacedObstacle != null)
                {
                    if (BombPositions.OnExplosionSections.ContainsKey(sectionFromPath))
                    {
                        for (int i = checkedPath.Count - 1; i > 0; i--)
                        {
                            if (!BombPositions.OnExplosionSections.ContainsKey(checkedPath[i]))
                            {
                                Debug.Log("Бомба на пути");
                                SetTarget(checkedPath[i].transform.position);
                                StayInTarget = BombPositions.OnExplosionSections[sectionFromPath] + 0.2f;
                                return;
                            }
                        }
                    }
                    SetTarget(targetSection.transform.position);
                    return;
                }
                checkedPath.Add(sectionFromPath);
                targetSection = sectionFromPath;
            }
            SetTarget(targetSection.transform.position);            
        }

        private void AStarPathFind(Transform target)
        {
            int x = Mathf.FloorToInt(_agent.transform.position.x + 0.5f);
            int z = Mathf.FloorToInt(_agent.transform.position.z + 0.5f);

            int targetX = Mathf.FloorToInt(target.position.x + 0.5f);
            int targetZ = Mathf.FloorToInt(target.position.z + 0.5f);

            _currentSection = BombPositions.SectionsPositions[x, z];
            GroundSection _goalSection = BombPositions.SectionsPositions[targetX, targetZ];

            _path = new Queue<GroundSection>();
            _path = CalculatePath(_currentSection, _goalSection);
        }
        private Queue<GroundSection> CalculatePath(GroundSection startSection, GroundSection goalSection)
        {
            if (canvas != null)
            {
                foreach (var c in canvas)
                {
                    c.gameObject.SetActive(false);
                }
            }            
            if (startSection == goalSection) // if bot is already on target section, return it
            {
                var queue = new Queue<GroundSection>();
                queue.Enqueue(startSection);
                return queue;   
            }
            _dangerSection.Clear();
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
                    int tentetiveG; 
                    //if (BombPositions.OnExplosionSections.ContainsKey(neighbor))
                    //{
                    //    tentetiveG = sectionCost[currentSection] + 1;
                    //}
                    //else tentetiveG = sectionCost[currentSection] + neighbor.cost;
                    tentetiveG = sectionCost[currentSection] + neighbor.cost;

                    if (BombPositions.OnExplosion(neighbor))
                    {
                        var explodeTime = BombPositions.OnExplosionSections[neighbor];
                        var convertedTime = Mathf.InverseLerp(0, 3, explodeTime);
                        var convertedToG = Mathf.FloorToInt(13 * convertedTime);                        
                        if (tentetiveG <= convertedToG + 1 && tentetiveG >= convertedToG - 2)
                        {
                            _dangerSection.Add(neighbor);
                        }
                    }

                    // if we encounter neighbor for the first time, add it to G list
                    if (!sectionCost.ContainsKey(neighbor))
                    {
                        sectionCost.Add(neighbor, tentetiveG);
                        parents.Add(neighbor, currentSection);
                    }
                    // if we've already checked this neighbor and found a better cost, update with the better cost
                    if (sectionCost.ContainsKey(neighbor) && sectionCost[neighbor] > tentetiveG)
                    {
                        sectionCost[neighbor] = tentetiveG;
                        parents[neighbor] = currentSection;
                    }

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

                    // if neighbor hasn't been checked before, add it to open list
                    if (!openList.ContainsKey(neighbor)) openList.Add(neighbor, neighborF);
                }
                // after checking all neighbors, add processed section to closed list
                //if (canvas != null)
                //{
                //    foreach (Canvas currCanvas in canvas)
                //    {
                //        if (!currCanvas.gameObject.activeInHierarchy)
                //        {
                //            bool obst = false;
                //            float cost = sectionCost[currentSection];
                //            if (currentSection.PlacedObstacle != null)
                //            {
                //                obst = true;
                //            }
                //            currCanvas.gameObject.SetActive(true);
                //            currCanvas.transform.position = currentSection.transform.position + new Vector3(0, 1.3f, 0);
                //            currCanvas.GetComponentInChildren<TextMeshProUGUI>().text = "Cost: " + cost.ToString() + "\n"
                //                + "Obstacle: " + obst.ToString();
                //            break;
                //        }
                //    }
                //}
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
            foreach(var a in _dangerSection)
            {
                //Debug.Log(a);
            }
            //----building a path--------
            currentSection = goalSection;
            while (currentSection != startSection)
            {
                path.Enqueue(currentSection);
                currentSection = parents[currentSection];
            }
            path.Enqueue(startSection);
            //Debug.Log("Закончен A");
            //-----End bulding path------
            return new Queue<GroundSection>(path.Reverse());
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

            if (_botLogicExecuter == null) Debug.LogError("StandartShelterFinder: did not find BotLogicExecuter");
            if (agent == null) Debug.LogError("StandartShelterFinder: did not find NavMeshAgent");

        }        

        public override void GenerateBlacklistPositions(BombDto bombDto)
        {
            BombPositions.InsertBombPosition(bombDto, _botLogicExecuter);
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
                    Vector2Int hitVector2Int = hit.position.ConvertToVector2Int();
                    _agent.CalculatePath(hit.position, path);

                    if (path.status == NavMeshPathStatus.PathComplete && !PointInBlackList(blacklist, point)) availablePositions.Add(hitVector2Int);
                }
            }
            if (availablePositions.Count > 0) return availablePositions;
            else
            {
                Debug.Log(_agent.gameObject.name + " FindAvailablePosForRetreat: did not find available positions for retreat");
                return availablePositions;
            }
        }
        public override void RetreatFromBomb(BotLogicExecuter bot)
        {
            HashSet<Vector2Int> bombPositions = BombPositions.GetAllBombPositions();
            var possiblePositions = GeneratePossiblePositions((byte)bot.Character.BombDto.BombsSpreading, bombPositions, bot.Character.BombDto.BombPosition);
            var availablePositions = FindAvailablePosForRetreat(possiblePositions, bombPositions);

            if (availablePositions == null || availablePositions.Count == 0)
            {
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

    public class PowerUpEvaluator 
    {
        public readonly static HashSet<PowerUpNotNet> SpawnedPowerUps = new HashSet<PowerUpNotNet>();

        public static void AddPowerUp(PowerUpNotNet powerUp)
        {
            if (SpawnedPowerUps.Contains(powerUp)) return;
            SpawnedPowerUps.Add(powerUp);
        }
        public static void RemovePowerUp(PowerUpNotNet powerUp)
        {
            if (!SpawnedPowerUps.Contains(powerUp)) return;
            SpawnedPowerUps.Remove(powerUp);
        }
        public static HashSet<PowerUpNotNet> GetSpawnedPowerUps()
        {
            return SpawnedPowerUps;
        }

    }

}

