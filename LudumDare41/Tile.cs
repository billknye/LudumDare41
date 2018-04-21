﻿using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace LudumDare41
{
    public class Tile
    {
        public Point Location;

        public byte SomeTileShit;

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


    }
}