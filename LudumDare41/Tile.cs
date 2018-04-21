using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LudumDare41
{
    public class Tile
    {
        public Point Location;

        public byte SomeTileShit;

        public int Light;

        public List<Entity> Entities;

        public TileDefinition Definition
        {
            get
            {
                return TileDefinition.Definitions[SomeTileShit];
            }
        }

        public Tile()
        {
            Entities = new List<Entity>();
        }

        public void Tick()
        {
            foreach (var ent in Entities)
            {
                ent.Tick();
            }
        }


    }
}