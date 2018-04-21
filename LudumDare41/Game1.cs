using Billknye.GameLib.Noise;
using LudumDare41.ContentManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace LudumDare41
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState lastKeyboard;

        Point viewOffset;
        Universe universe;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1280;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {// TODO: Add your initialization logic here

            universe = new Universe();
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            Assets.Initialize(GraphicsDevice);

           // MediaPlayer.Play(Assets.Songs.Intro);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here            
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Space) && lastKeyboard.IsKeyUp(Keys.Space))
            {
                Assets.SoundEffects.Coin.Play();
            }

            // make move go
            var dest = Point.Zero;
            if (keyboard.IsKeyDown(Keys.NumPad1))
            {
                dest = new Point(-1, 1);
            }
            if (keyboard.IsKeyDown(Keys.NumPad2))
            {
                dest = new Point(0, 1);
            }
            if (keyboard.IsKeyDown(Keys.NumPad3))
            {
                dest = new Point(1, 1);
            }
            if (keyboard.IsKeyDown(Keys.NumPad4))
            {
                dest = new Point(-1, 0);
            }
            if (keyboard.IsKeyDown(Keys.NumPad6))
            {
                dest = new Point(1, 0);
            }
            if (keyboard.IsKeyDown(Keys.NumPad7))
            {
                dest = new Point(-1, -1);
            }
            if (keyboard.IsKeyDown(Keys.NumPad8))
            {
                dest = new Point(0, -1);
            }
            if (keyboard.IsKeyDown(Keys.NumPad9))
            {
                dest = new Point(1, -1);
            }

            universe.Player += dest;

            var width = (int)Math.Ceiling(Window.ClientBounds.Width / 64.0);
            var height = (int)Math.Ceiling(Window.ClientBounds.Height / 64.0);

            // make player be center
            viewOffset = new Point(-width / 2 + universe.Player.X, -height / 2 + universe.Player.Y);

            //Console.WriteLine(viewOffset);

            lastKeyboard = keyboard;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            Color[] testColors = new Color[]
            {
                Color.Blue,
                Color.White,
                Color.Red,
                Color.Orange,
                Color.Yellow,
                Color.Green,
                Color.Magenta,
                Color.Brown,
                Color.Pink
            };

            spriteBatch.Begin();

            var viewRectangle = new Rectangle(
                viewOffset.X,
                viewOffset.Y,
                (int)Math.Ceiling(Window.ClientBounds.Width / 64.0),
                (int)Math.Ceiling(Window.ClientBounds.Height / 64.0)
                );

            Console.WriteLine(viewRectangle);

            universe.GetTilesInRange(viewRectangle, tile =>
            {
                spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((tile.Location.X - viewOffset.X) * 64, (tile.Location.Y - viewOffset.Y) * 64), new Rectangle(64, 0, 64, 64), testColors[ tile.SomeTileShit % testColors.Length]);
                spriteBatch.DrawString(Assets.Fonts.Japonesa16pt, $"{tile.Location.X},{tile.Location.Y}", new Vector2((tile.Location.X - viewOffset.X) * 64, (tile.Location.Y - viewOffset.Y) * 64 + 40), Color.Black);
            });


            spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((universe.Player.X - viewOffset.X) * 64, (universe.Player.Y - viewOffset.Y) * 64), new Rectangle(0, 0, 64, 64), Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }


    public class Universe
    {
        public Dictionary<Point, ChunkOfSpace> ChunksOfFreedom;

        public Point Player;

        SimplexNoise2D noise;

        public Universe()
        {
            ChunksOfFreedom = new Dictionary<Point, ChunkOfSpace>();
            noise = new SimplexNoise2D(293234, 100);
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
    }



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
    }

    public class Entity
    {
        public Tile Tile;
    }

    public class Tile
    {
        public Point Location;

        public byte SomeTileShit;

        public List<Entity> Entities;

        public Tile()
        {
            Entities = new List<Entity>();
        }
    }
}