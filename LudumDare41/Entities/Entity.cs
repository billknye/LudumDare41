using LudumDare41.Entities.Behavior;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LudumDare41
{
    public abstract class Entity
    {
        public Tile Tile { get; set; }

        public virtual int SpriteIndex => 2;

        public virtual int LightEmitted => 0;

        public virtual SpriteEffects SpriteEffects => SpriteEffects.None;

        public virtual Type Behavior => null;
    }
}