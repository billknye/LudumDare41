using LudumDare41.Entities.Behavior;
using Microsoft.Xna.Framework;
using System;

namespace LudumDare41.Entities
{
    public class Enemy : Entity
    {
        public int HitPoints { get; set; }
        public int BaseAttack { get; set; }
        public int ModifierAttack { get; set; }

        public Point Velocity { get; set; }

        public override int SpriteIndex => 6;

        public override Type Behavior => typeof(EnemyAttackBehavior);

        public Enemy()
        {
        }

    }
}