using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare41.Entities.Behavior
{
    public abstract class EntityBehavior
    {
        public EntityBehavior()
        {
        }
        public abstract void Tick(Entity entity);
    }

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

    public class EnemyAttackBehavior : EntityBehavior
    {
        private readonly Universe universe;

        public EnemyAttackBehavior(Universe universe)
        {
            this.universe = universe;
        }

        public override void Tick(Entity entity)
        {
            var enemy = entity as Enemy;

            if (enemy.HitPoints <= 0)
            {
                enemy = null; //Enemy is dead
                return;
            }

            var tile = enemy.Tile;

            // move towards da player...
            var dx = Math.Sign(universe.Player.Tile.Location.X - tile.Location.X);
            var dy = Math.Sign(universe.Player.Tile.Location.Y - tile.Location.Y);

            if (dx != 0 && dy == 0)
            {
                var move = universe.Random.Next(0, 3);
                if (move == 1)
                {
                    var destPt = tile.Location + new Point(dx, 0);
                    var dest = universe[destPt.X, destPt.Y];
                    universe.RemoveEntityFromTile(enemy);
                    universe.AddEntityToTile(enemy, dest);
                }
                else if (move == 0)
                {
                    var destPt = tile.Location + new Point(0, dy);
                    var dest = universe[destPt.X, destPt.Y];
                    universe.RemoveEntityFromTile(enemy);
                    universe.AddEntityToTile(enemy, dest);
                }
                else
                {
                    var destPt = tile.Location + new Point(dx, dy);
                    var dest = universe[destPt.X, destPt.Y];
                    universe.RemoveEntityFromTile(enemy);
                    universe.AddEntityToTile(enemy, dest);
                }
            }
            else if (dx != 0)
            {
                var destPt = tile.Location + new Point(dx, 0);
                var dest = universe[destPt.X, destPt.Y];
                universe.RemoveEntityFromTile(enemy);
                universe.AddEntityToTile(enemy, dest);
            }
            else if (dy != 0)
            {
                var destPt = tile.Location + new Point(0, dy);
                var dest = universe[destPt.X, destPt.Y];
                universe.RemoveEntityFromTile(enemy);
                universe.AddEntityToTile(enemy, dest);
            }
            else
            {
                Console.WriteLine(); // what do
            }

            //Both 
            if (tile.Location == universe.Player.Tile.Location)
            {
                Combat(enemy);
            }
        }

        private void Combat(Enemy enemy)
        {
            universe.Player.HitPoints = universe.Player.HitPoints - universe.Random.Next(1, enemy.BaseAttack);
            enemy.HitPoints = enemy.HitPoints - universe.Random.Next(1, universe.Player.BaseAttack);

            Console.WriteLine($"Enemy hitpoints: {universe.Player.HitPoints}");
            Console.WriteLine($"Enemy hitpoints: {enemy.HitPoints}");
        }
    }

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

            if (tile.Location == universe.Player.Tile.Location)
            {
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