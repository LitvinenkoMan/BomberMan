using Interfaces;
using MonoBehaviours.GroundSectionSystem;
using System.Collections;
using System.Collections.Generic;
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

            GroundSection nextSection;

            //Dictionary<GroundSection, int> keyValuePairs = CalculeteCoustForSection();
        }
        private Dictionary<GroundSection, int> CalculeteCoustForSection(GroundSection section)
        {
            Dictionary<GroundSection, int> cousts = new Dictionary<GroundSection, int>();

            GroundSection upperSection = section.ConnectedSections.upperSection;
            GroundSection lowerSection = section.ConnectedSections.lowerSection;
            GroundSection leftSection = section.ConnectedSections.leftSection;
            GroundSection rightSection = section.ConnectedSections.rightSection;

            cousts.Add(upperSection, upperSection.coust);
            cousts.Add(rightSection, rightSection.coust);
            cousts.Add(leftSection, leftSection.coust);
            cousts.Add(lowerSection, lowerSection.coust);

            return cousts;
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

