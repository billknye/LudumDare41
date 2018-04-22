using LudumDare41.Entities.Behavior;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LudumDare41.Entities
{
    public abstract class Entity
    {
        private int spriteIndex;

        public Tile FutureTile { get; set; }
        public Tile Tile { get; set; }
        public Tile PreviousTile { get; set; }

        public virtual int SpriteIndex
        {
            get => spriteIndex;
            set => spriteIndex = value;     
        }

        public virtual int LightEmitted => 0;

        public virtual SpriteEffects SpriteEffects => SpriteEffects.None;

        public virtual Type Behavior => null;
    }
}