using Billknye.GameLib.Noise;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace LudumDare41
{
    public class Universe
    {
        public Dictionary<Point, ChunkOfSpace> ChunksOfFreedom;

        public Entity Player;

        SimplexNoise2D noise;

        public Universe()
        {
            ChunksOfFreedom = new Dictionary<Point, ChunkOfSpace>();
            noise = new SimplexNoise2D(293234, 100);

            generateChunk(0, 0);
            Player = new Entity();

            EntityToTile(Player, this[0, 0]);
        }

        public void GetTilesInRange(Rectangle viewRectangle, Action<Tile> doThing)
        {
            for (int x =viewRectangle.X; x < viewRectangle.Right; x++)
            {
                for (int y = viewRectangle.Y; y < viewRectangle.Bottom; y++)
                {
                    var tile = this[x, y];

                    doThing(tile);
                }
            }
        }


        public Tile this[int x, int y]
        {
            get
            {
                var chunkX = x & ~0xf;
                var chunkY = y & ~0xf;

                var localX = x & 0xf;
                var localY = y & 0xf;

                if (ChunksOfFreedom.TryGetValue(new Point(chunkX, chunkY), out var chunk))
                {
                    return chunk[localX, localY];
                }

                // make the thing
                var chunk2 = generateChunk(chunkX, chunkY);
                return chunk2[localX, localY];
            }
        }

        private ChunkOfSpace generateChunk(int chunkX, int chunkY)
        {
            var chunk = new ChunkOfSpace();
            chunk.Tiles = new Tile[16 * 16];

            for (int x = 0; x < 16; x++)
            {
                for (int y = 0; y < 16; y++)
                {
                    var pos = new Point(chunkX + x, chunkY + y);
                    chunk[x, y] = new Tile
                    {
                        Location = pos,
                        SomeTileShit = (byte)(noise.GetValue(chunkX + x, chunkY + y) * 10)
                    };
                }
            }

            ChunksOfFreedom[new Point(chunkX, chunkY)] = chunk;


            return chunk;

        }

        public void EntityToTile(Entity entity, Tile tile)
        {
            entity.Tile = tile;
            tile.Entities.Add(entity);
        }

        public void EntityFromTile(Entity entity)
        {
            entity.Tile.Entities.Remove(entity);
            entity.Tile = null;
        }

        internal void DoMove(Point point)
        {
            var dest = this[point.X, point.Y];
            EntityFromTile(Player);
            EntityToTile(Player, dest);

            // tick!
        }
    }
}