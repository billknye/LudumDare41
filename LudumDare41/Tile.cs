using DryIoc;
using LudumDare41.Entities.Behavior;
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

        public void Tick(Container container)
        {
            foreach (var ent in Entities)
            {
                if (ent.Behavior != null)
                {
                    var behavior = container.New(ent.Behavior) as EntityBehavior;
                    behavior.Tick(ent);
                }
            }
        }
    }
}