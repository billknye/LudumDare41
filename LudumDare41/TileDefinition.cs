namespace LudumDare41
{
    public class TileDefinition
    {
        public static int LastTileDefinitionId;
        public static TileDefinition[] Definitions;

        public static TileDefinition OpenSpace;
        public static TileDefinition SoidTHing;

        static TileDefinition()
        {
            Definitions = new TileDefinition[256];

            OpenSpace = AddTileDefinition(new TileDefinition());
            SoidTHing = AddTileDefinition(new SolidTileDefinition());
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
    }
}