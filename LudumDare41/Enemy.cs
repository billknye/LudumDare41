using Microsoft.Xna.Framework;

namespace LudumDare41
{
    public class Enemy : Entity
    {
        public int HitPoints { get; set; }
        public int BaseAttack { get; set; }
        public int ModifierAttack { get; set; }

        public Point Velocity { get; set; }

        public override int SpriteIndex => 0;
    }
}