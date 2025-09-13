using UnityEngine;

namespace Interfaces
{
    public interface IPlayerUIPanel
    {
        public void UpdateHealth(int prev,  int current);
        public void UpdateBombsAtTime(int prev,  int current);
        public void UpdateBombsSpread(int prev,  int current);
        public void UpdateBombsDamage(int prev,  int current);
        public void UpdateSpeedMultiplier(float prev,  float current);
    }
}
