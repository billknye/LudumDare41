using LudumDare41.ContentManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace LudumDare41
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class RoguelikePlatformGame : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState lastKeyboard;

        Point viewOffset;
        Universe universe;

        public RoguelikePlatformGame()
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
            if (wasKeyJustPressed(Keys.NumPad1, keyboard, lastKeyboard))
            {
                dest = new Point(-1, 1);
            }
            if (wasKeyJustPressed(Keys.NumPad2, keyboard, lastKeyboard))
            {
                dest = new Point(0, 1);
            }
            if (wasKeyJustPressed(Keys.NumPad3, keyboard, lastKeyboard))
            {
                dest = new Point(1, 1);
            }
            if (wasKeyJustPressed(Keys.NumPad4, keyboard, lastKeyboard))
            {
                dest = new Point(-1, 0);
            }
            if (wasKeyJustPressed(Keys.NumPad6, keyboard, lastKeyboard))
            {
                dest = new Point(1, 0);
            }
            if (wasKeyJustPressed(Keys.NumPad7, keyboard, lastKeyboard))
            {
                dest = new Point(-1, -1);
            }
            if (wasKeyJustPressed(Keys.NumPad8, keyboard, lastKeyboard))
            {
                dest = new Point(0, -1);
            }
            if (wasKeyJustPressed(Keys.NumPad9, keyboard, lastKeyboard))
            {
                dest = new Point(1, -1);
            }

            if (dest != Point.Zero)
            {
                universe.DoMove(dest);                
            }

            var width = (int)Math.Ceiling(Window.ClientBounds.Width / 64.0);
            var height = (int)Math.Ceiling(Window.ClientBounds.Height / 64.0);

            // make player be center
            viewOffset = new Point(-width / 2 + universe.Player.Tile.Location.X, -height / 2 + universe.Player.Tile.Location.Y);

            //Console.WriteLine(viewOffset);

            lastKeyboard = keyboard;
            base.Update(gameTime);
        }

        private bool wasKeyJustPressed(Keys key, KeyboardState keyboard, KeyboardState lastKeyboard)
        {
            return keyboard.IsKeyDown(key) && lastKeyboard.IsKeyUp(key);
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


            spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((universe.Player.Tile.Location.X - viewOffset.X) * 64, (universe.Player.Tile.Location.Y - viewOffset.Y) * 64), new Rectangle(0, 0, 64, 64), Color.White);


            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}