
using UnityEngine;

namespace Interfaces
{
    public interface ICharacterAppearance
    {
        public MeshRenderer MeshRenderer { get; }
        
        public void SetNewAppearance(ICharacterRuntimeData characterRuntimeData);
        public void ClearAppearance();
    }
}
