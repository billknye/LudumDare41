namespace LudumDare41
{
    public class TileDefinition
    {
        public static int LastTileDefinitionId;
        public static TileDefinition[] Definitions;

        public static TileDefinition OpenSpace;
        public static TileDefinition SolidThing;

        static TileDefinition()
        {
            Definitions = new TileDefinition[256];

            OpenSpace = AddTileDefinition(new TileDefinition());
            SolidThing = AddTileDefinition(new SolidTileDefinition());
        }

        private static TileDefinition AddTileDefinition(TileDefinition tileDefinition)
        {
            Definitions[LastTileDefinitionId] = tileDefinition;
            LastTileDefinitionId++;
            return tileDefinition;
        }

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
                return 4;
            }
        }
    }
}
