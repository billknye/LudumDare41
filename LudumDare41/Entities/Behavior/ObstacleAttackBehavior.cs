namespace LudumDare41.Entities.Behavior
{
    public class ObstacleAttackBehavior : EntityBehavior
    {
        private readonly Universe universe;

        public ObstacleAttackBehavior(Universe universe)
        {
            this.universe = universe;
        }

        public override void Tick(Entity entity)
        {
            if (universe.Player.Tile == entity.Tile)
            {
                var damage = universe.Random.Next(UniverseConfiguration.ObstacleMinDamage, UniverseConfiguration.ObstacleMaxDamage);
                universe.Player.HitPoints -= damage;
            }
        }
    }
}