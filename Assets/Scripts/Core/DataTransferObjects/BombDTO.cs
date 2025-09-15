using System;
using UnityEngine;
using Unity.Netcode;

namespace Core.DataTransferObjects
{
    [Serializable]
    public class BombDto : INetworkSerializable
    {
        private float _bombCountdown;
        private int _bombsAtTime;
        private int _bombsSpreading;
        private int _bombsDamage;
        private Vector3 _bombPosition;

        public float BombCountdown =>_bombCountdown;
        public int BombsAtTime => _bombsAtTime;
        public int BombsSpreading => _bombsSpreading;
        public int BombsDamage => _bombsDamage;
        public Vector3 BombPosition => _bombPosition;

        public BombDto() { }

        public BombDto(float bombCountdown, int bombsAtTime, int bombsSpreading, int bombsDamage)
        {
            _bombsAtTime = bombsAtTime;
            _bombCountdown = bombCountdown;
            _bombsDamage = bombsDamage;
            _bombsSpreading = bombsSpreading;
        }
        public BombDto(float bombCountdown, int bombsAtTime, int bombsSpreading, int bombsDamage, Vector3 bombPosition) : this(bombCountdown, bombsAtTime, bombsSpreading, bombsDamage)
        {
            _bombPosition = bombPosition;
        }
        public void SetBombPosition(Vector3 pos)
        {
            _bombPosition = pos;
        }

        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref _bombCountdown);
            serializer.SerializeValue(ref _bombsAtTime);
            serializer.SerializeValue(ref _bombsSpreading);
            serializer.SerializeValue(ref _bombsDamage);
        }
    }
}