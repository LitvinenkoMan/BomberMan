using System;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours.GroundSectionSystem;
using Unity.Collections;
using UnityEngine;

namespace MonoBehaviours.GroundSectionSystem
{
    public class GroundSection : MonoBehaviour
    {
        
        [ReadOnly]
        public Obstacle PlacedObstacle;
        public SectionPathways ConnectedSections;

        public Vector3 ObstaclePlacementPosition
        { 
            get => _obstaclePosition;
            private set => _obstaclePosition = value;
        }

        private Vector3 _obstaclePosition;

        private void Start()
        {
            _obstaclePosition = transform.position + new Vector3(0, 0.5f, 0);
        }

        /// <summary>
        /// Returns true if newObstacle was placed, false if otherwise.
        /// </summary>
        /// <param name="newObstacle">Any object inherited from obstacle</param>
        /// <returns></returns>
        public void AddObstacle(Obstacle newObstacle)
        {
            PlacedObstacle = newObstacle;
            PlacedObstacle.transform.position = _obstaclePosition;
        }

        public void SetNewSectionPosition(Vector3 position)
        {
            transform.position = position;
            _obstaclePosition = transform.position + new Vector3(0, 0.5f, 0);
            
            if (PlacedObstacle) PlacedObstacle.SetNewPosition(_obstaclePosition);
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