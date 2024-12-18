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
                DontDestroyOnLoad(gameObject);
            } 
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

        public GroundSection GetNearestSectionFromPositionWithRadius(Vector3 searchPosition, float radius)
        {
            GroundSection nearestSection = null;
            float distance = 99999999;
            var objects = Physics.OverlapSphere(searchPosition, radius);
            foreach (var o in objects)
            {
                if (o.TryGetComponent(out GroundSection section))
                {
                    if (Vector3.Distance(searchPosition, section.transform.position) < distance)
                    {
                        distance = Vector3.Distance(searchPosition, section.transform.position);
                        nearestSection = section;
                    }
                }
            }

            return nearestSection;
        }

        public void SetNewDataHolder(LevelSectionsDataHolder dataHolder)
        {
            _sectionsDataHolder = dataHolder;
        }

        public LevelSectionsDataHolder GetCurrentSectionDataHolder()
        {
            return _sectionsDataHolder;
        }
    }
}
