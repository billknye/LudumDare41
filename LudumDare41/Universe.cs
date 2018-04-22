using Billknye.GameLib;
using Billknye.GameLib.Noise;
using DryIoc;
using LudumDare41.Entities;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LudumDare41
{
    public partial class Universe
    {
        public Dictionary<Point, ChunkOfSpace> ChunksOfFreedom;
        public Player Player;

        public Random Random;
        SimplexNoise2D noise;
        SimplexNoise2D deckingNoise;
        private readonly Container container;

        private List<Action> thingsToDoAfterTick;

        public Universe(Container container)
        {
            this.container = container;
            thingsToDoAfterTick = new List<Action>();
            int seed = DateTime.Now.Millisecond;
            Random = new Random(seed);

            ChunksOfFreedom = new Dictionary<Point, ChunkOfSpace>();

            noise = new SimplexNoise2D(2, 5);
            deckingNoise = new SimplexNoise2D(456565, 30);

            var chunk = generateChunk(0, 0);

            //REMOVE THIS. Set single enemy close to player
            var singleEnemy = container.New<Enemy>();
            singleEnemy.HitPoints = UniverseConfiguration.SquidwardMaxHP;
            singleEnemy.BaseAttack = UniverseConfiguration.SquidwardBaseAttack;
            singleEnemy.SpriteIndex = 5;

            AddEntityToTile(singleEnemy, this[3, 0]);


            Player = new Player() { HitPoints = UniverseConfiguration.PlayerInitialHP, BaseAttack = UniverseConfiguration.PlayerBaseAttack };
            AddEntityToTile(Player, this[0, 0]);

            AddObstacles();
            AddEnemies();

            DoTick();

            Console.WriteLine();
        }

        private void AddEnemies()
        {
            int[] kindOfEnemy = { 5, 6 }; //5 = Mr. Mander, 6 = Squid

            for(var i = 0; i <= UniverseConfiguration.SquidwardNumberOfEnemies; ++i)
            {
                Enemy enemy = container.New<Enemy>();
                enemy.HitPoints = UniverseConfiguration.SquidwardMaxHP;
                enemy.BaseAttack = UniverseConfiguration.SquidwardBaseAttack;
                enemy.SpriteIndex = kindOfEnemy[Random.Next(kindOfEnemy.Length)];
                AddEntityToTile(enemy, this[Random.Next(1, UniverseConfiguration.TotalEnemies), Random.Next(1, UniverseConfiguration.TotalEnemies)]);
            }
        }


        private void AddObstacles()
        {
            for (int i = 0; i <= Random.Next(UniverseConfiguration.MinNumberOfObstacles, UniverseConfiguration.MaxNumberOfObstacles); i++)
            {
                Obstacle obstacle = new Obstacle() { Destructible = true };
                AddEntityToTile(obstacle, this[Random.Next(1, UniverseConfiguration.MaxNumberOfObstacles), Random.Next(1, UniverseConfiguration.MaxNumberOfObstacles)]);
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

                    var index = TileDefinition.OpenSpace.TileDefinitionId;
                    var val = deckingNoise.GetValue(pos.X, pos.Y);

                    if (val > 0.5f)
                    {
                        var val2 = noise.GetValue(pos.X, pos.Y);
                        if (val2 > 0.75f)
                        {
                            // open, no decking here
                            //index = TileDefinition.Wall.TileDefinitionId;
                        }
                        else
                        {
                            if (pos.Y % 3 == 0)
                            {
                                index = TileDefinition.Decking.TileDefinitionId;
                            }
                            else
                            {
                                index = TileDefinition.Wall.TileDefinitionId;
                            }
                        }
                    }

                    chunk[x, y] = new Tile
                    {
                        Location = pos,
                        TileDefinitionId = index
                    };

                    if (!chunk[x, y].Definition.Solid)
                    {
                        if (Random.NextDouble() > 0.99)
                        {
                            var o2Tank = new OxygenTank() { };
                            AddEntityToTile(o2Tank, chunk[x, y]);
                        }
                    }
                }
            }

            ChunksOfFreedom[new Point(chunkX, chunkY)] = chunk;

            return chunk;
        }

        public void AddEntityToTile(Entity entity, Tile tile)
        {
            thingsToDoAfterTick.Add(() =>
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
            });
        }

        public void RemoveEntityFromTile(Entity entity)
        {
            thingsToDoAfterTick.Add(() =>
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
                entity.PreviousTile = entity.Tile;
                entity.Tile = null;
            });
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

            // When jetpack is on, player can move anywhere
            if (Player.JetPackOn)
            {
                // Jetpack on
                foreach (var neighbor in neighbors)
                {
                    var dest = Player.Tile.Location + neighbor;
                    if (!isSolid(dest))
                        yield return this[dest];
                }
            }

            else
            {
                // Jetpack not on
                foreach (var neighbor in neighbors)
                {
                    if (canMove(Player, Player.Tile.Location, neighbor))
                    {
                        var dest = Player.Tile.Location + neighbor;
                        yield return this[dest.X, dest.Y];
                    }
                }
            }
        }

        private bool canMove(Player player, Point start, Point moveDir)
        {
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
            var destDef = TileDefinition.Definitions[dest.TileDefinitionId];

            return destDef.Opaque;
        }

        private bool isOpaque(Point point)
        {
            var dest = this[point.X, point.Y];
            var destDef = TileDefinition.Definitions[dest.TileDefinitionId];

            return destDef.Solid;
        }

        private bool isOnLand(Entity entity)
        {
            return isSolid(entity.Tile.Location + new Point(0, 1));
        }
        internal void DoTick(Point? moveDir = null)
        {
            if (moveDir != null)
            {
                doPlayerMove(moveDir.Value);
            }

            // tick!
            Player.Oxygen--;

            // Jetpack stuff
            if (Player.JetPackOn)
            {
                if (Player.JetPackFuel <= 0)
                {
                    Player.JetPackOn = false;
                }
                else
                {
                    Player.JetPackFuel -= Player.JetPackDecreaseFuel;
                    if (Player.JetPackFuel < 0)
                    {
                        Player.JetPackFuel = 0;
                    }
                }
            }
            else if (Player.JetPackFuel < Player.MaxJetPackFuel)
            {
                Player.JetPackFuel += Player.JetPackIncreaseFuel;
                if (Player.JetPackFuel > Player.MaxJetPackFuel)
                {
                    Player.JetPackFuel = Player.MaxJetPackFuel;
                }
            }

            // Tick-ing
            foreach (var chunk in ChunksOfFreedom.Values.ToArray())
            {
                chunk.Tick(container);
            }

            foreach (var action in thingsToDoAfterTick)
            {
                action();
            }
            thingsToDoAfterTick.Clear();
        }

        private void doPlayerMove(Point moveDir)
        {
            // Check to see if move is valid and if Jet pack is off
            if (!canMove(Player, Player.Tile.Location, moveDir) && !Player.JetPackOn)
            {
                return; // no thanks 
            }

            if (isOnLand(Player))
            {
                Player.Velocity = Point.Zero;
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
            var destDef = TileDefinition.Definitions[dest.TileDefinitionId];

            if (destDef.Solid)
            {
                return; // nope
            }

            // check solid corner moves
            if (moveDir.X != 0 && moveDir.Y != 0)
            {
                var corner1 = this[Player.Tile.Location.X + moveDir.X, Player.Tile.Location.Y];
                var corner1def = TileDefinition.Definitions[corner1.TileDefinitionId];
                if (corner1def.Solid)
                    return;

                var corner2 = this[Player.Tile.Location.X, Player.Tile.Location.Y + moveDir.Y];
                var corner2def = TileDefinition.Definitions[corner2.TileDefinitionId];
                if (corner2def.Solid)
                    return;
            }

            if (moveDir.X < 0)
                Player.LastMoveLeft = true;
            else if (moveDir.X > 0)
                Player.LastMoveLeft = false;

            Player.FutureTile = dest;
            RemoveEntityFromTile(Player);
            AddEntityToTile(Player, dest);

            Console.WriteLine(Player.Velocity);
            var playerTile = this[Player.Tile.Location.X, Player.Tile.Location.Y];
            foreach (var entity in playerTile.Entities)
            {
                if (entity is Obstacle obstacle)
                {
                    var damage = Random.Next(UniverseConfiguration.ObstacleMinDamage, UniverseConfiguration.ObstacleMaxDamage);
                    Player.HitPoints -= damage;
                }
            }
        }
    }
}