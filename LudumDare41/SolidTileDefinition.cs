namespace LudumDare41
{
    public class SolidTileDefinition : TileDefinition
    {
        public override bool Solid => true;

        public override bool Opaque => true;

        public override int SpriteIndex => 1;
    }

    public class AsteroidTileDefinition : SolidTileDefinition
    {

        public override int SpriteIndex => 7;
    }
    
    public class DeckingTileDefinition : SolidTileDefinition
    {

        public override int SpriteIndex => 1;
    }
}