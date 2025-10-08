
using AbstractClasses;
using Core.DataTransferObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Runtime.MonoBehaviours.Bot
{
    public class BombPositions
    {
        private BombPositions() { }

        private static readonly HashSet<Vector2Int> BombPos = new HashSet<Vector2Int>();
        private static readonly object _sync = new object();

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
            Vector2Int bombPos = BaseShelterFinder.ConvertToVector2Int(bombDto.BombPosition);

            var bombs = new HashSet<Vector2Int>() { bombPos };

            for (int i = 1; i <= bombDto.BombsSpreading; i++)
            {
                bombs.Add(bombPos + new Vector2Int(i, 0));
                bombs.Add(bombPos + new Vector2Int(-i, 0));
                bombs.Add(bombPos + new Vector2Int(0, i));
                bombs.Add(bombPos + new Vector2Int(0, -i));
            }
            lock (_sync)
            {
                BombPos.UnionWith(bombs);
            }

            yield return new WaitForSeconds(lifeTime);

            lock (_sync)
            {
                BombPos.ExceptWith(bombs);
            }
        }
        public static bool Contains(Vector2Int point)
        {
            lock (_sync)
            {
                return BombPos.Contains(point);
            }
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

