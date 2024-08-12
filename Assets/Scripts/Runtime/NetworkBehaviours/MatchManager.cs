using System.Collections;
using MonoBehaviours.Network;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace MonoBehaviours
{
    public class MatchManager : NetworkBehaviour
    {
        [SerializeField] private TMP_Text JoinCodeText;


        public UnityEvent StartMatch;

        [SerializeField]
        private 
        void Start()
        {
            JoinCodeText.text = RelayManager.Instance.JoinCode;
        }

        void Update()
        {
        
        }
    }
}
