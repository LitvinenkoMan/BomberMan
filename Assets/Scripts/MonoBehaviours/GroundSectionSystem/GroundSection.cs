using System;
using System.ComponentModel;
using MonoBehaviours.GroundSectionSystem;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class GroundSection : MonoBehaviour
    {
        
        public Vector3 ObstaclePlacementPosition { get; private set; }
        public Obstacle PlacedObstacle { get; private set; }
        public SectionPathways ConnectedSections;

        private void Start()
        {
            ObstaclePlacementPosition = transform.position + new Vector3(0, 0.5f, 0);
        }

        /// <summary>
        /// Returns true if newObstacle was placed, false if otherwise.
        /// </summary>
        /// <param name="newObstacle">Any object inherited from obstacle</param>
        /// <returns></returns>
        public bool AddObstacle(Obstacle newObstacle)
        {
            if (PlacedObstacle) return false;
            PlacedObstacle = newObstacle;
            PlacedObstacle.transform.position = ObstaclePlacementPosition;
            return true;
        }

        public void SetNewSectionPosition(Vector3 position)
        {
            transform.position = position;
            ObstaclePlacementPosition = transform.position + new Vector3(0, 0.5f, 0);
            PlacedObstacle.SetNewPosition(ObstaclePlacementPosition);
        }
    }
}

[Serializable]
public struct SectionPathways
{
    public GroundSection upperSection;
    public GroundSection lowerSection;
    public GroundSection rightSection;
    public GroundSection leftSection;
}