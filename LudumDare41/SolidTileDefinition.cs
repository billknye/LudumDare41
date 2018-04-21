namespace LudumDare41
{
    public class SolidTileDefinition : TileDefinition
    {
        public override bool Solid => true;

        public override bool Opaque => true;

        public override int SpriteIndex => 1;
    }
    
}