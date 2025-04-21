namespace Interfaces
{
    public interface ICharacter
    {
        public IHealth Health { get; }
        public IImmune Immune { get; }
        public IBombDeployer BombDeployer { get; }
        public IMovable CharacterMovement { get;}

        public void Initialize();

        public void Damage(int damageAmount);

        public void Heal(int healAmount);

        public void ActivateSpecial();

        public void DeployBomb();

        public void SetMoveAbility(bool canMove);

        public void SetBombDeployAbility(bool canDeploy);
    }
}