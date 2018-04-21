namespace LudumDare41
{
    public abstract class Entity
    {
        public Tile Tile { get; set; }

        public virtual int SpriteIndex => 2;
    }
}