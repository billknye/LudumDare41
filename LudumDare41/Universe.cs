using Billknye.GameLib.Noise;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace LudumDare41
{
    public class Universe
    {
        public Dictionary<Point, ChunkOfSpace> ChunksOfFreedom;

        public Player Player;
        public List<Obstacle> Obstacles;

        Random r;
        SimplexNoise2D noise;

        public Universe()
        {
            ChunksOfFreedom = new Dictionary<Point, ChunkOfSpace>();

            r = new Random();
            noise = new SimplexNoise2D(293234, 100);

            generateChunk(0, 0);
            Player = new Player();
            Obstacles = new List<Obstacle>();
            AddObstacles(Obstacles);
            EntityToTile(Player, this[0, 0]);
        }

        private void AddObstacles(List<Obstacle> obstacles)
        {
            //TODO: IMPROVE THIS SHIT
            int seed = DateTime.Now.Millisecond;
            Random numberOfObstacles = new Random(seed);

            for (int i = 0; i <= numberOfObstacles.Next(UniverseConfiguration.MinNumberOfObstacles, UniverseConfiguration.MaxNumberOfObstacles); i++)
            {
                Obstacle obstacle = new Obstacle() { Destructible = true };
                EntityToTile(obstacle, this[numberOfObstacles.Next(1, UniverseConfiguration.MaxNumberOfObstacles), numberOfObstacles.Next(1, UniverseConfiguration.MaxNumberOfObstacles)]);
                obstacles.Add(obstacle);
            }
        }

        public void GetTilesInRange(Rectangle viewRectangle, Action<Tile> doThing)
        {
            for (int x = viewRectangle.X; x < viewRectangle.Right; x++)
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
                        SomeTileShit = (byte)(noise.GetValue(chunkX + x, chunkY + y) > 0.5f ? 1 : 0)
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

        internal void DoMove(Point moveDir)
        {
            var point = Player.Tile.Location + moveDir;

            var dest = this[point.X, point.Y];
            var destDef = TileDefinition.Definitions[dest.SomeTileShit];

            if (destDef.Solid)
            {
                return; // nope
            }

            // check solid corner moves
            if (moveDir.X != 0 && moveDir.Y != 0)
            {
                var corner1 = this[Player.Tile.Location.X + moveDir.X, Player.Tile.Location.Y];
                var corner1def = TileDefinition.Definitions[corner1.SomeTileShit];
                if (corner1def.Solid)
                    return;

                var corner2 = this[Player.Tile.Location.X, Player.Tile.Location.Y + moveDir.Y];
                var corner2def = TileDefinition.Definitions[corner2.SomeTileShit];
                if (corner2def.Solid)
                    return;
            }

            EntityFromTile(Player);
            EntityToTile(Player, dest);

            // tick!
            attackTheThings();
        }

        private void attackTheThings()
        {
            var entitiesToMove = new List<Enemy>();

            for (int x = Player.Tile.Location.X - 5; x <= Player.Tile.Location.X + 5; x++)
            {
                for (int y = Player.Tile.Location.Y - 5; y <= Player.Tile.Location.Y + 5; y++)
                {
                    var tile = this[x, y];
                    foreach (var entity in tile.Entities)
                    {
                        if (entity is Enemy enemy)
                        {
                            if (r.Next(0, 100) > 30) // move to attack 70%
                            {
                                entitiesToMove.Add(enemy);
                            }
                        }
                    }
                }
            }

            foreach (var enemy in entitiesToMove)
            {
                {
                    // move towards da player...
                    var dx = Math.Sign(Player.Tile.Location.X - enemy.Tile.Location.X);
                    var dy = Math.Sign(Player.Tile.Location.Y - enemy.Tile.Location.Y);

                    if (dx != 0 && dy == 0)
                    {
                        var move = r.Next(0, 3);
                        if (move == 1)
                        {
                            var destPt = enemy.Tile.Location + new Point(dx, 0);
                            var dest = this[destPt.X, destPt.Y];
                            EntityFromTile(enemy);
                            EntityToTile(enemy, dest);
                        }
                        else if (move == 0)
                        {
                            var destPt = enemy.Tile.Location + new Point(0, dy);
                            var dest = this[destPt.X, destPt.Y];
                            EntityFromTile(enemy);
                            EntityToTile(enemy, dest);
                        }
                        else
                        {
                            var destPt = enemy.Tile.Location + new Point(dx, dy);
                            var dest = this[destPt.X, destPt.Y];
                            EntityFromTile(enemy);
                            EntityToTile(enemy, dest);
                        }
                    }
                    else if (dx != 0)
                    {
                        var destPt = enemy.Tile.Location + new Point(dx, 0);
                        var dest = this[destPt.X, destPt.Y];
                        EntityFromTile(enemy);
                        EntityToTile(enemy, dest);
                    }
                    else if (dy != 0)
                    {
                        var destPt = enemy.Tile.Location + new Point(0, dy);
                        var dest = this[destPt.X, destPt.Y];
                        EntityFromTile(enemy);
                        EntityToTile(enemy, dest);
                    }
                    else
                    {
                        Console.WriteLine(); // what do
                    }
                }
            }
        }

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
        }

        public class SolidTileDefinition : TileDefinition
        {
            public override bool Solid => true;
        }
    }
}