using Core.ScriptableObjects;
using UnityEngine;

namespace CSharp
{
    public class CharacterRuntime
    {
        public CharacterData BaseData { get; private set; }

        public int CurrentHealth { get; set; }
        public float CurrentSpeed { get; set; }
        public int CurrentBombsAtTime { get; set; }
        public int CurrentBombSpread { get; set; }
        public int CurrentBombDamage { get; set; }

        public float BombCountdown => BaseData.BombCountdown;
        public float KickForce => BaseData.KickForce;

        public GameObject Visuals => BaseData.Visuals;
        public GameObject BombPrefab => BaseData.Bomb;
        public RuntimeAnimatorController Animator => BaseData.AnimatorController;

        public CharacterRuntime(CharacterData data)
        {
            BaseData = data;

            CurrentHealth = data.Health;
            CurrentSpeed = data.Speed;
            CurrentBombsAtTime = data.BombsAtTime;
            CurrentBombSpread = data.BombSpread;
            CurrentBombDamage = data.BombDamage;
        }
    }
}
