using System;

namespace LudumDare41
{
    public class ChunkOfSpace
    {
        public Tile[] Tiles;

        public Tile this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= 16 || y < 0 || y >= 16)
                    return null;

                return Tiles[x | y << 4];
            }
            set
            {
                if (x < 0 || x >= 16 || y < 0 || y >= 16)
                    throw new Exception("nope");

                Tiles[x | y << 4] = value;
            }
        }

        public ChunkOfSpace()
        {
        }

        public void Tick()
        {
            foreach (var tile in Tiles)
            {
                tile.Tick();
            }
        }
    }
}