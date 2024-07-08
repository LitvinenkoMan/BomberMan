using System;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace MonoBehaviours.GroundSectionSystem
{
    public class GroundSectionsUtils : MonoBehaviour
    {
        public static GroundSectionsUtils Instance;
        public ObjectPoolQueue ExplosionsPool;
        
        [SerializeField]
        private LevelSectionsDataHolder _sectionsDataHolder;

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }

        void Start()
        {
        }

        public GroundSection GetNearestSectionFromPosition(Vector3 searchPosition)
        {
            GroundSection nearestSection = null;
            float distance = 99999999;
            foreach (var section in _sectionsDataHolder.sections)
            {
                if (Vector3.Distance(searchPosition, section.transform.position) < distance)
                {
                    distance = Vector3.Distance(searchPosition, section.transform.position);
                    nearestSection = section;
                }
            }

            return nearestSection;
        }

        public void SetNewDataHolder(LevelSectionsDataHolder dataHolder)
        {
            _sectionsDataHolder = dataHolder;
        }
    }
}
