using LudumDare41.Entities.Behavior;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LudumDare41.Entities
{
    public class Enemy : Entity
    {
        private int spriteIndex = 6;
        public int HitPoints { get; set; }
        public int BaseAttack { get; set; }
        public int ModifierAttack { get; set; }

        public Point Velocity { get; set; }

        public override int SpriteIndex
        {
            get => spriteIndex;

            set => spriteIndex = value;
        }

        public override Type Behavior => typeof(EnemyAttackBehavior);

        public override SpriteEffects SpriteEffects => (HitPoints <= 0 ? SpriteEffects.FlipVertically : SpriteEffects.None);

        public Enemy()
        {
        }
    }
}