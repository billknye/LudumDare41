using Microsoft.Xna.Framework;

namespace LudumDare41
{
    public class Player : Entity
    {
        int HitPoints { get; set; }
        public Point Velocity { get; set; }

        public override int SpriteIndex => 0;

        public override int LightEmitted => 4;
    }
}
