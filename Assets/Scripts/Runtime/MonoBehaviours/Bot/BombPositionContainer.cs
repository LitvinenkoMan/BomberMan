using AbstractClasses;
using Core.DataTransferObjects;
using MonoBehaviours.GroundSectionSystem;
using Runtime.MonoBehaviours.Bot.StandartBotUtils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace Runtime.MonoBehaviours.Bot
{
    public class BombPositions
    {

        private BombPositions() { }

        public static readonly GroundSection[,] SectionsPositions = new GroundSection[32, 32];
        private static readonly HashSet<Vector2Int> BombPos = new HashSet<Vector2Int>();
        public static readonly Dictionary<GroundSection, float> OnExplosionSections = new Dictionary<GroundSection, float>();
        private static readonly object _sync = new object();

        public static void CreateGrid()
        {
            foreach (var section in GroundSectionsUtils.Instance.GetCurrentSectionDataHolder().sections)
            {
                if (section.PlacedObstacle != null) section.cost = 13;
                int x = Mathf.FloorToInt(section.transform.position.x);
                int z = Mathf.FloorToInt(section.transform.position.z);
                SectionsPositions[x, z] = section;
            }
        }

        public static void InsertBombPosition(BombDto bombDto, MonoBehaviour monoBehaviour)
        {
            if (monoBehaviour == null || bombDto == null)
            {
                Debug.LogError("BombPosition(InsertBombPosition): method have got uncorrect parametrs");
                return; 
            }
            monoBehaviour.StartCoroutine(GeneratePositions(bombDto, bombDto.BombCountdown));
        }
        private static IEnumerator GeneratePositions(BombDto bombDto, float lifeTime)
        {
            float bombTimer = lifeTime;
            Vector2Int bombPos = bombDto.BombPosition.ConvertToVector2Int();        

            var onExplosionSections = new Dictionary<GroundSection, float>() { { SectionsPositions[bombPos.x, bombPos.y], bombTimer } };
            var bombs = new HashSet<Vector2Int>() { bombPos };

            for (int i = 1; i <= bombDto.BombsSpreading; i++)
            {                
                bombs.Add(bombPos + new Vector2Int(i, 0));
                bombs.Add(bombPos + new Vector2Int(-i, 0));
                bombs.Add(bombPos + new Vector2Int(0, i));
                bombs.Add(bombPos + new Vector2Int(0, -i));

                onExplosionSections.Add(SectionsPositions[bombPos.x + i, bombPos.y], bombTimer);
                onExplosionSections.Add(SectionsPositions[bombPos.x - i, bombPos.y], bombTimer);
                onExplosionSections.Add(SectionsPositions[bombPos.x, bombPos.y + i], bombTimer);
                onExplosionSections.Add(SectionsPositions[bombPos.x, bombPos.y - i], bombTimer);
            }
            lock (_sync)
            {
                BombPos.UnionWith(bombs);
                OnExplosionSections.AddRange(onExplosionSections);
            }

            while (bombTimer > 0)
            {
                bombTimer -= Time.deltaTime;
                float clamped = Mathf.Max(0f, bombTimer);

                var keys = OnExplosionSections.Keys.ToList(); 
                foreach (var key in keys)
                {
                    OnExplosionSections[key] = clamped;
                }
                yield return null;
            }

            lock (_sync)
            {
                BombPos.ExceptWith(bombs);
                foreach (var bomb in onExplosionSections)
                {
                    OnExplosionSections.Remove(bomb.Key);
                }                
            }
        }
        public static bool Contains(Vector2Int point)
        {
            lock (_sync)
            {
                return BombPos.Contains(point);
            }
        }
        public static bool OnExplosion(GroundSection section)
        {
            lock (_sync)
            {
                return OnExplosionSections.ContainsKey(section);
            }
        }
        public static float BombRemainingTime(GroundSection section)
        {
            return 0;
        }
        public static HashSet<Vector2Int> GetAllBombPositions()
        {
            lock (_sync)
            {
                return new HashSet<Vector2Int>(BombPos);
            }
        }
    }
}

