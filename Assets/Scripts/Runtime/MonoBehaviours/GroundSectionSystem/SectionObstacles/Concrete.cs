namespace Runtime.MonoBehaviours.GroundSectionSystem.SectionObstacles
{
    public class Concrete : Obstacle
    {
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            ObstacleHealthCmp.Initialize(255);
            ObstacleHealthCmp.SetAbilityToReceiveDamage(false);
            SetAbilityToPlayerCanStepOnIt(false);
        }
    }
}