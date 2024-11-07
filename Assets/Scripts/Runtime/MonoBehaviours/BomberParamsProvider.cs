using ScriptableObjects;
using TMPro;
using Unity.Netcode;
using UnityEngine;

namespace Runtime.MonoBehaviours
{
    public class BomberParamsProvider : NetworkBehaviour
    {
        [SerializeField]
        private TMP_Text PlayerName;
        [SerializeField]
        private BaseBomberParameters BomberParams;

        private void Start()
        {
            BomberParams.ResetValues();
            if (!IsOwner)
            {
                enabled = false;
            }
        }

        public BaseBomberParameters GetBomberParams()
        {
            return BomberParams;
        }

        public override void OnNetworkSpawn()
        {
            name = $"P{GetComponent<NetworkObject>().OwnerClientId}";
            PlayerName.text = name;
        }

        protected override void OnOwnershipChanged(ulong previous, ulong current)
        {
            name = $"P{GetComponent<NetworkObject>().OwnerClientId}";
            PlayerName.text = name;
        }
        
        private void 
    }
}
