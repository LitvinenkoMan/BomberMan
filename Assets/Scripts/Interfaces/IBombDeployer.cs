namespace Interfaces
{
    public interface IBombDeployer
    {
        public void Initialize();  
        public void SetAbilityToDeployBombs(bool canIt);
        public void DeployBomb(int bombsAtTime, float timeToExplode, int bombDamage, int bombSpread);
    }
}
