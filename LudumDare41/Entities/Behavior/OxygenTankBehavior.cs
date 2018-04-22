using System;

namespace LudumDare41.Entities.Behavior
{

    public class OxygenTankBehavior : PlayerOnlyItemPickupBehavior
    {
        public OxygenTankBehavior(Universe universe) : base(universe)
        {

        }

        protected override void PickedUp(Entity entity)
        {
            universe.Player.Oxygen = universe.Player.Oxygen + UniverseConfiguration.ItemOxygenTankAmountToRefill > UniverseConfiguration.PlayerMaxOxigen
                ? UniverseConfiguration.PlayerMaxOxigen : universe.Player.Oxygen += UniverseConfiguration.ItemOxygenTankAmountToRefill;
        }
    }
}