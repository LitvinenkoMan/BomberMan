using System;
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

        public float BombCountdown =>_bombCountdown;
        public int BombsAtTime => _bombsAtTime;
        public int BombsSpreading => _bombsSpreading;
        public int BombsDamage => _bombsDamage;

        public BombDto()
        {
        }

        public BombDto(float bombCountdown, int bombsAtTime, int bombsSpreading, int bombsDamage)
        {
            _bombsAtTime = bombsAtTime;
            _bombCountdown = bombCountdown;
            _bombsDamage = bombsDamage;
            _bombsSpreading = bombsSpreading;
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