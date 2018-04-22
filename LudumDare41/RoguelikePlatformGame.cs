using DryIoc;
using LudumDare41.ContentManagement;
using LudumDare41.States;
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
        GameStateManager gameStateManager;

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
        {
            // TODO: Add your initialization logic here
            
            Container container = new Container();

            container.RegisterInstance(GraphicsDevice);
            container.Register<SpriteBatch>(Reuse.Singleton);
            container.RegisterInstance<GameWindow>(Window);
            container.RegisterInstance(container);
            container.Register<GameStateManager>(Reuse.Singleton);
            container.Register<Universe>(Reuse.Singleton);

            container.Register<Entities.Behavior.SquidwardAttackBehavior>(Reuse.Singleton);
            container.Register<Entities.Behavior.MrManderAttackBehavior>(Reuse.Singleton);
            container.Register<Entities.Behavior.ObstacleAttackBehavior>(Reuse.Singleton);
            container.Register<Entities.Behavior.OxygenTankBehavior>(Reuse.Singleton);
            gameStateManager = container.Resolve<GameStateManager>();
            var intro = container.New<IntroState>();
            gameStateManager.Enter(intro);

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
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
            
            gameStateManager.Update(gameTime);
            return;
            
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            gameStateManager.Draw(gameTime);
            return;
        }

    }
}