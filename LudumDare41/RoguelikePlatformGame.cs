﻿using LudumDare41.ContentManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LudumDare41
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class RoguelikePlatformGame : Game
    {
        const int tileSize = UniverseConfiguration.TileSize;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState lastKeyboard;
        MouseState lastMouse;

        List<Tuple<Rectangle, Point>> moveThings;

        Point viewOffset;
        Universe universe;

        public RoguelikePlatformGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            this.IsMouseVisible = true;
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
            moveThings = new List<Tuple<Rectangle, Point>>();

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

            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();


            // make move go
            var dest = Point.Zero;

            if (lastMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed)
            {
                foreach (var rect in moveThings)
                {
                    if (rect.Item1.Contains(mouse.Position))
                    {
                        dest = rect.Item2 - universe.Player.Tile.Location;
                    }
                }
            }

            if (wasKeyJustPressed(Keys.NumPad1, keyboard, lastKeyboard))
            {
                dest = new Point(-1, 1);
            }
            if (wasKeyJustPressed(Keys.NumPad2, keyboard, lastKeyboard) || (wasKeyJustPressed(Keys.Down, keyboard, lastKeyboard)))
            {
                dest = new Point(0, 1);
            }
            if (wasKeyJustPressed(Keys.NumPad3, keyboard, lastKeyboard))
            {
                dest = new Point(1, 1);
            }
            if (wasKeyJustPressed(Keys.NumPad4, keyboard, lastKeyboard) || (wasKeyJustPressed(Keys.Left, keyboard, lastKeyboard)))
            {
                dest = new Point(-1, 0);
            }
            if (wasKeyJustPressed(Keys.NumPad6, keyboard, lastKeyboard) || (wasKeyJustPressed(Keys.Right, keyboard, lastKeyboard)))
            {
                dest = new Point(1, 0);
            }
            if (wasKeyJustPressed(Keys.NumPad7, keyboard, lastKeyboard))
            {
                dest = new Point(-1, -1);
            }
            if (wasKeyJustPressed(Keys.NumPad8, keyboard, lastKeyboard) || (wasKeyJustPressed(Keys.Up, keyboard, lastKeyboard)))
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

            var width = (int)Math.Ceiling(Window.ClientBounds.Width / (double)tileSize);
            var height = (int)Math.Ceiling(Window.ClientBounds.Height / (double)tileSize);

            // make player be center
            viewOffset = new Point(-width / 2 + universe.Player.Tile.Location.X, -height / 2 + universe.Player.Tile.Location.Y);


            lastMouse = mouse;
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
            
            spriteBatch.Begin();

            var availableMoves = universe.GetAvailableMoves().ToList();

            moveThings.Clear();

            universe.GetTilesInRange(universe.Player.Tile.Location.X, universe.Player.Tile.Location.Y, 5, tile =>
            {
                var spriteIndex = tile.Definition.SpriteIndex;
                if (spriteIndex == 4)
                    spriteIndex += Math.Abs((tile.Location.X * 23 + tile.Location.Y * 5) % 12);

                spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((tile.Location.X - viewOffset.X) * 64, (tile.Location.Y - viewOffset.Y) * 64), new Rectangle((spriteIndex % 4) * 64, (spriteIndex / 4) * 64, 64, 64), Color.White);
                
                if (availableMoves.Contains(tile))
                {
                    var loc = new Vector2((tile.Location.X - viewOffset.X) * 64, (tile.Location.Y - viewOffset.Y) * 64);
                    moveThings.Add(Tuple.Create(new Rectangle((int)loc.X, (int)loc.Y, 64, 64), tile.Location));
                    spriteBatch.Draw(Assets.Sprites.SampleSprite, loc, new Rectangle(64, 0, 64, 64), Color.Lime);
                }

                spriteBatch.DrawString(Assets.Fonts.Japonesa16pt, $"{tile.Location.X},{tile.Location.Y}", new Vector2((tile.Location.X - viewOffset.X) * 64 + 2, (tile.Location.Y - viewOffset.Y) * 64 + 40), Color.Black);                
                foreach(var people in tile.Entities)
                {
                    spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((tile.Location.X - viewOffset.X) * tileSize, (tile.Location.Y - viewOffset.Y) * tileSize), new Rectangle(0, 0, tileSize, tileSize), Color.White);
                }

               
            });

            for (int i = 0; i < universe.Obstacles.Count; i++)
            {
                universe.GetTilesInRange(universe.Obstacles[i].Tile.Location.X, universe.Obstacles[i].Tile.Location.Y, 5, tile =>
                {
                    spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((universe.Obstacles[i].Tile.Location.X - viewOffset.X) * tileSize, (universe.Obstacles[i].Tile.Location.Y - viewOffset.Y) * tileSize), new Rectangle(tileSize * 2, 0, tileSize, tileSize), Color.White);
                });
            }
          
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}