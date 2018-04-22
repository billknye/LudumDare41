using System;

namespace LudumDare41.Entities.Behavior
{
    public class OxygenTankBehavior : EntityBehavior
    {
        private readonly Universe universe;

        public OxygenTankBehavior(Universe universe)
        {
            this.universe = universe;
        }

        public override void Tick(Entity entity)
        {
            var tank = entity as OxygenTank;
            var tile = tank.Tile;

            if (universe.Player.FutureTile != null && tile.Location == universe.Player.FutureTile.Location)
            {
                Assets.SoundEffects.Pickup.Play();
                FillOxygen(tank);
                universe.RemoveEntityFromTile(tank);
            }
        }

        private void FillOxygen(OxygenTank item)
        {
            universe.Player.Oxygen = universe.Player.Oxygen + UniverseConfiguration.ItemOxygenTankAmountToRefill > UniverseConfiguration.PlayerMaxOxigen 
                ? UniverseConfiguration.PlayerMaxOxigen : universe.Player.Oxygen += UniverseConfiguration.ItemOxygenTankAmountToRefill;

            Console.WriteLine($"Oxygen: {universe.Player.Oxygen}");
        }
    }
}