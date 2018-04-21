using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LudumDare41
{
    public class Player : Entity
    {
        public int HitPoints { get; set; }
        public int BaseAttack { get; set; }
        public int ModifierAttack { get; set; }
        public Point Velocity { get; set; }

        public float Oxygen { get; set; }
        public float MaxOxygen { get; set; }
        public int MaxHitPoints { get; set; }

        public bool LastMoveLeft { get; set; }

        public override int SpriteIndex => 0;

        public override int LightEmitted => 4;

        public override SpriteEffects SpriteEffects => (LastMoveLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

        public Player()
        {
            Oxygen = MaxOxygen = 100;
            HitPoints = MaxHitPoints = 100;
        }
    }
}
