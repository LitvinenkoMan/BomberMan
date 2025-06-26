using System;

namespace Interfaces
{
    public interface IImmuneVisualizer
    {
        public event Action<float> OnImmuneVisualized;
        
        public void VisualizeImmunity(float time);
    }
}
