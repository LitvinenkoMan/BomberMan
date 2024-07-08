using System;
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
            if (PlacedObstacle) return false;  // Decided to do this way, because I want to prevent adding a new Obstacle to section with already existing one
            if (!newObstacle) return false;
            PlacedObstacle = newObstacle;
            PlacedObstacle.transform.position = ObstaclePlacementPosition;
            return true;
        }

        public void SetNewSectionPosition(Vector3 position)
        {
            transform.position = position;
            ObstaclePlacementPosition = transform.position + new Vector3(0, 0.5f, 0);
            
            if (PlacedObstacle) PlacedObstacle.SetNewPosition(ObstaclePlacementPosition);
        }

        /// <summary>
        /// Removes obstacle if there was one.
        /// </summary>
        /// <returns>Last placed obstacle</returns>
        public Obstacle RemoveObstacle()
        {
            var lastObstacle = PlacedObstacle;
            PlacedObstacle = null;
            return lastObstacle;
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