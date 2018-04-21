using Billknye.GameLib;
using Billknye.GameLib.Noise;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace LudumDare41
{
    public partial class Universe
    {
        public Dictionary<Point, ChunkOfSpace> ChunksOfFreedom;

        public Player Player;

        GridFieldOfView gridFieldOfView;
        Random r;
        SimplexNoise2D noise;

        public Universe()
        {
            ChunksOfFreedom = new Dictionary<Point, ChunkOfSpace>();

            r = new Random();
            noise = new SimplexNoise2D(2235, 40);

            var chunk = generateChunk(0, 0);

            for (int x = 0; x < 10; x++)
            {
                chunk[x, 1].SomeTileShit = 1;
            }

            Player = new Player();
            AddObstacles();
            EntityToTile(Player, this[0, 0]);
        }

        private void AddObstacles()
        {
            //TODO: IMPROVE THIS SHIT
            int seed = DateTime.Now.Millisecond;
            Random numberOfObstacles = new Random(seed);

            for (int i = 0; i <= numberOfObstacles.Next(UniverseConfiguration.MinNumberOfObstacles, UniverseConfiguration.MaxNumberOfObstacles); i++)
            {
                Obstacle obstacle = new Obstacle() { Destructible = true };
                EntityToTile(obstacle, this[numberOfObstacles.Next(1, UniverseConfiguration.MaxNumberOfObstacles), numberOfObstacles.Next(1, UniverseConfiguration.MaxNumberOfObstacles)]);
            }
        }

        public void GetTilesInRange(int centerX, int centerY, int radius, Action<Tile> doThing)
        {
            GridFieldOfView.ComputeFieldOfViewWithShadowCasting(centerX, centerY, radius, (x, y) =>
            {
                return isOpaque(new Point(x, y));
            }, (x, y) =>
            {
                var tile = this[x, y];
                doThing(tile);
            });


            /*for (int x = viewRectangle.X; x < viewRectangle.Right; x++)
            {
                for (int y = viewRectangle.Y; y < viewRectangle.Bottom; y++)
                {
                    var tile = this[x, y];

                    doThing(tile);
                }
            }*/
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

        public Tile this[Point pt]
        {
            get
            {
                return this[pt.X, pt.Y];
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

            if (entity.LightEmitted > 0)
            {
                var ent = entity.Tile.Location;
                GridFieldOfView.ComputeFieldOfViewWithShadowCasting(
                    ent.X,
                    ent.Y,
                    entity.LightEmitted,
                    (x, y) => isOpaque(new Point(x, y)),
                    (x, y) =>
                    {
                        var delta = new Point(Math.Abs(x - ent.X), Math.Abs(y - ent.Y));
                        var dist = (int)Math.Floor(Math.Sqrt((delta.X * delta.X) + (delta.Y * delta.Y)));

                        if (dist > entity.LightEmitted)
                            return;

                        this[x, y].Light += (entity.LightEmitted - dist);
                    });
            }
        }

        public void EntityFromTile(Entity entity)
        {
            if (entity.LightEmitted > 0)
            {
                var ent = entity.Tile.Location;
                GridFieldOfView.ComputeFieldOfViewWithShadowCasting(
                    ent.X,
                    ent.Y,
                    entity.LightEmitted,
                    (x, y) => isOpaque(new Point(x, y)),
                    (x, y) =>
                    {
                        var delta = new Point(Math.Abs(x - ent.X), Math.Abs(y - ent.Y));
                        var dist = (int)Math.Floor(Math.Sqrt((delta.X * delta.X) + (delta.Y * delta.Y)));

                        if (dist > entity.LightEmitted)
                            return;

                        this[x, y].Light -= (entity.LightEmitted - dist);
                    });
            }

            entity.Tile.Entities.Remove(entity);
            entity.Tile = null;
        }

        public IEnumerable<Tile> GetAvailableMoves()
        {
            var neighbors = new Point[]
            {
                new Point(-1, -1),
                new Point(-1, 0),
                new Point(-1, 1),
                new Point(0, -1),
                new Point(0, 1),
                new Point(1, -1),
                new Point(1, 0),
                new Point(1, 1)
            };

            foreach (var neighbor in neighbors)
            {
                if (canMove(Player, Player.Tile.Location, neighbor))
                {
                    var dest = Player.Tile.Location + neighbor;
                    yield return this[dest.X, dest.Y];
                }
            }
        }

        private bool canMove(Player player, Point start, Point moveDir)
        {
            bool canMove = true;
            var dest = start + moveDir;

            if (isSolid(dest))
            {
                return false;
            }
            // check solid corner moves
            if (moveDir.X != 0 && moveDir.Y != 0)
            {
                if (isSolid(new Point(Player.Tile.Location.X + moveDir.X, Player.Tile.Location.Y)))
                { 
                    return false;
                }

                if (isSolid(new Point(Player.Tile.Location.X, Player.Tile.Location.Y + moveDir.Y)))
                {
                    return false;
                }
            }

            // check if we can jump if it is up, or we have the vel to do
            if (moveDir.Y < 0)
            {
                if (player.Velocity.Y >= 0 && !isOnLand(player))
                {
                    return false;
                }                                
            }

            if (moveDir.X != 0)
            {
                if (!isOnLand(player))
                {
                    // if in air, can only keep moving in current x direction
                    if (player.Velocity.X != 0)
                    {
                        if (Math.Sign(player.Velocity.X) != Math.Sign(moveDir.X))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        // if we aren't moving x, we can start if we are going at least 2
                        if (Math.Abs(player.Velocity.Y) < 2)
                        {
                            return false;
                        }
                        else if (Math.Sign(player.Velocity.Y) != Math.Sign(moveDir.Y))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        private bool isSolid(Point point)
        {
            var dest = this[point.X, point.Y];
            var destDef = TileDefinition.Definitions[dest.SomeTileShit];

            return destDef.Opaque;
        }

        private bool isOpaque(Point point)
        {
            var dest = this[point.X, point.Y];
            var destDef = TileDefinition.Definitions[dest.SomeTileShit];

            return destDef.Solid;
        }

        private bool isOnLand(Entity entity)
        {
            return isSolid(entity.Tile.Location + new Point(0, 1));
        }

        internal void DoMove(Point moveDir)
        {
            if (!canMove(Player, Player.Tile.Location, moveDir))
            {
                return; // no thanks 
            }

            if (Player.Velocity == Point.Zero && isOnLand(Player))
            {
                Player.Velocity = new Point(moveDir.X * 3, moveDir.Y * 3);
            }
            else
            {
                if (Player.Velocity.X > 0)
                {
                    Player.Velocity = new Point(Player.Velocity.X - 1, Player.Velocity.Y + 1);
                }
                else if (Player.Velocity.X < 0)
                {
                    Player.Velocity = new Point(Player.Velocity.X + 1, Player.Velocity.Y + 1);
                }
                else
                {
                    Player.Velocity = new Point(Player.Velocity.X, Player.Velocity.Y + 1);
                }
            }

            if (Player.Velocity.Y > 3)
            {
                Player.Velocity = new Point(Player.Velocity.X, 3);
            }

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

            if (isOnLand(Player))
            {
                Player.Velocity = Point.Zero;
            }

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
    }
}