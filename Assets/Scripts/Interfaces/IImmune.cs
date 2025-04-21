using System;

namespace Interfaces
{
    public interface IImmune
    {
        public bool IsImmune { get; }
        
        public event Action<float> OnGetImmune;
        
        public void Initialize();
        public void ActivateImmunity();
        public void SetNewImmunityTime(float newTime);
    }
}
