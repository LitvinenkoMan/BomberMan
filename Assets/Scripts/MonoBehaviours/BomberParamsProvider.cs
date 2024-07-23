using ScriptableObjects;
using UnityEngine;

namespace MonoBehaviours
{
    public class BomberParamsProvider : MonoBehaviour
    {
        [SerializeField]
        private BaseBomberParameters BomberParams;

        public BaseBomberParameters GetBomberParams()
        {
            return BomberParams;
        }
    }
}
