using Microsoft.Xna.Framework.Graphics;

namespace LudumDare41
{
    public abstract class Entity
    {
        public Tile Tile { get; set; }

        public virtual int SpriteIndex => 2;

        public virtual int LightEmitted => 0;

        public virtual SpriteEffects SpriteEffects => SpriteEffects.None;
    }
}