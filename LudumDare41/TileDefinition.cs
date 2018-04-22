namespace LudumDare41
{
    public class TileDefinition
    {
        public static int LastTileDefinitionId;
        public static TileDefinition[] Definitions;

        public static TileDefinition OpenSpace;
        public static TileDefinition Asteroid;
        public static TileDefinition Decking;
        public static TileDefinition Wall;

        static TileDefinition()
        {
            Definitions = new TileDefinition[256];

            OpenSpace = AddTileDefinition(new TileDefinition());
            Asteroid = AddTileDefinition(new AsteroidTileDefinition());
            Decking = AddTileDefinition(new DeckingTileDefinition());
            Wall = AddTileDefinition(new WallTileDefinition());
        }

        private static TileDefinition AddTileDefinition(TileDefinition tileDefinition)
        {
            Definitions[LastTileDefinitionId] = tileDefinition;
            tileDefinition.TileDefinitionId = LastTileDefinitionId;
            LastTileDefinitionId++;
            return tileDefinition;
        }

        public int TileDefinitionId { get; set; }

        public virtual bool Solid
        {
            get
            {
                return false;
            }
        }

        public virtual bool Opaque
        {
            get
            {
                return false;
            }
        }

        public virtual int SpriteIndex
        {
            get
            {
                return 8;
            }
        }
    }
}
