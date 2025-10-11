
using Core.ScriptableObjects;
using UnityEngine;

namespace Interfaces
{
    public interface ICharacterAppearance
    {
        public MeshRenderer MeshRenderer { get; }
        
        public void SetNewAppearance(ICharacterData characterData);
        public void ClearAppearance();
    }
}
