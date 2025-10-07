using Interfaces;
using UnityEngine;

namespace Runtime.NetworkBehaviours.Player
{
    public class PlayerCharacterAppearanceNet : ICharacterAppearance
    {
        public MeshRenderer MeshRenderer { get; private set; }
        
        public void SetNewAppearance(ICharacterRuntimeData characterRuntimeData)
        {
            
        }

        public void ClearAppearance()
        {
            throw new System.NotImplementedException();
        }
    }
}
