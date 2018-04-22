using System.Linq;

namespace LudumDare41.Entities.Behavior
{
    public abstract class ItemPickupBehavior : EntityBehavior
    {
        protected readonly Universe universe;

        public ItemPickupBehavior(Universe universe)
        {
            this.universe = universe;
        }

        public override void Tick(Entity entity)
        {
            var tile = entity.Tile;

            var picker = tile.Entities.FirstOrDefault(n => n != entity && CanPickup(n));
            if ((picker != null) && (picker == universe.Player))
            {
                Assets.SoundEffects.Pickup.Play();

                PickedUp(picker);
                universe.RemoveEntityFromTile(entity);
            }
        }

        protected virtual bool CanPickup(Entity entity)
        {
            return true;
        }

        protected abstract void PickedUp(Entity entity);
    }
}