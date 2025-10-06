
using System.Collections.Generic;

namespace Interfaces
{
    public interface IObjectPool<T> : IEnumerable<T>
    {
        public void Initialize();
        public void AddToPool(T item);
        public T GetFromPool(bool makeActive);
        public void Clear();
    }
}
