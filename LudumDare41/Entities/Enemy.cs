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
        private SpriteEffects LastEffect = SpriteEffects.None;

        public override SpriteEffects SpriteEffects
        {
            get
            {
                if(HitPoints <= 0)
                {
                    return SpriteEffects.FlipVertically;
                }

                if(Velocity.X > 0)
                {
                    LastEffect = SpriteEffects.FlipHorizontally;
                    return LastEffect;
                }

                if(Velocity.X == 0)
                {
                    return LastEffect;
                }
                LastEffect = SpriteEffects.None;
                return LastEffect;
            }
        }


        public Enemy()
        {
        }
    }
}