using Unity.Networking.Transport;


namespace Interfaces
{
    public interface IObjectPool<T>
    {
        public void Initialize();
        public void AddToPool(T item);
        public T GetFromPool(bool makeActive);
        public void Clear();
    }
}
