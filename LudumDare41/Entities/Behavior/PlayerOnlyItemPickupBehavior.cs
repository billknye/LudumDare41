namespace LudumDare41.Entities.Behavior
{
    public abstract class PlayerOnlyItemPickupBehavior : ItemPickupBehavior
    {
        public PlayerOnlyItemPickupBehavior(Universe universe) : base(universe)
        {

        }

        protected override bool CanPickup(Entity entity)
        {
            return entity is Player;
        }
    }
}