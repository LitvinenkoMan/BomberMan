using Core.SaveSystem;

namespace Interfaces
{
    public interface ISavable
    {
        public GameData Save();
        public void Load(GameData data);
    }
}
