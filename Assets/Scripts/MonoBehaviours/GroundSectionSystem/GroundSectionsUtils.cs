using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

namespace MonoBehaviours.GroundSectionSystem
{
    public class GroundSectionsUtils : MonoBehaviour
    {
        public static GroundSectionsUtils Instance;
        
        void Start()
        {
            Instance = this;
        }

        public GroundSection GetNearestSectionFromPosition(Vector3 searchPosition)
        {
            Collider[] colliders = Physics.OverlapSphere(searchPosition, 3);
            List<GroundSection> sections = new List<GroundSection>();
            
            foreach (var collider in colliders)
            {
                GroundSection section;
                collider.gameObject.TryGetComponent(out section);
                if (section)
                {
                    sections.Add(section);
                }
            }

            GroundSection nearestSection = null;
            float distance = 999999;
            foreach (var section in sections)
            {
                if (Vector3.Distance(searchPosition, section.transform.position) < distance)
                {
                    distance = Vector3.Distance(searchPosition, section.transform.position);
                    nearestSection = section;
                }
            }

            return nearestSection;
        }
    }
}
