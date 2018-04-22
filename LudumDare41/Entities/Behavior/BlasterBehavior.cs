namespace LudumDare41.Entities.Behavior
{
    public class BlasterBehavior : PlayerOnlyItemPickupBehavior
    {
        public BlasterBehavior(Universe universe): base(universe)
        {

        }

        protected override void PickedUp(Entity entity)
        {
            // do nothing!
        }
    }
}