using Core.DataTransferObjects;

namespace Interfaces
{
    public interface IBombDeployer
    {
        public void Initialize();  
        public void SetAbilityToDeployBombs(bool canIt);
        public void DeployBomb(BombDto bombDto);
    }
}
