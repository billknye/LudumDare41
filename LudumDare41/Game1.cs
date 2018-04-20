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
        MouseState lastMouse;

        Vector2 viewOffset;
        Vector2 playerPosition = new Vector2(10, 5);
        float playerAngle;
        int playerHealth, maxPlayerHealth;

        Random r;

        byte[,] map;

        List<Bullet> bullets;
        List<Zombie> zombies;

        Texture2D pixelTexture;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            graphics.PreferredBackBufferHeight = 768;
            graphics.PreferredBackBufferWidth = 1024;
            this.IsMouseVisible = true;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {// TODO: Add your initialization logic here

            base.Initialize();

            r = new Random();
            map = new byte[256, 256];

            for (int x = 0; x < 256; x++)
            {
                for (int y =0; y < 256; y++)
                {
                    map[x, y] = (byte)r.Next(0, 2);
                }
            }

            bullets = new List<Bullet>();
            zombies = new List<Zombie>();
            
            for (int z =0; z < 1000; z++)
            {

                zombies.Add(new Zombie
                {
                    Position = new Vector2((float)r.NextDouble() * 256, (float)r.NextDouble() * 256),
                    Angle = MathHelper.TwoPi * (float)r.NextDouble()
                });
            }

            playerHealth = maxPlayerHealth = 20;
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

            MediaPlayer.Play(Assets.Songs.Intro);

            pixelTexture = new Texture2D(GraphicsDevice, 1, 1);
            pixelTexture.SetData<Color>(new[] { Color.White });
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
            MouseState mouse = Mouse.GetState();

            if (keyboard.IsKeyDown(Keys.Space) && lastKeyboard.IsKeyUp(Keys.Space))
            {
                Assets.SoundEffects.Coin.Play();
            }

            var moveVector = Vector2.Zero;

            if (keyboard.IsKeyDown(Keys.W))
            {
                moveVector += new Vector2(1, 0);
            }
            if (keyboard.IsKeyDown(Keys.A))
            {
                moveVector += new Vector2(0, -1);
            }
            if (keyboard.IsKeyDown(Keys.S))
            {
                moveVector += new Vector2(-1, 0);
            }
            if (keyboard.IsKeyDown(Keys.D))
            {
                moveVector += new Vector2(0, 1);
            }

            var speed = 4.0f;
            playerAngle = (float)Math.Atan2(mouse.Y - (viewOffset.Y + playerPosition.Y * 64.0f), mouse.X - (viewOffset.X + playerPosition.X * 64.0f));

            if (moveVector != Vector2.Zero)
            {
                moveVector.Normalize();
                var rot = Matrix.CreateRotationZ(playerAngle);
                playerPosition += Vector2.Transform(moveVector, rot) * speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
            }

            playerPosition = new Vector2(
                Math.Min(playerPosition.X, 64 * map.GetLength(0)),
                Math.Min(playerPosition.Y, 64 * map.GetLength(1))
                );

            playerPosition = new Vector2(
                Math.Max(playerPosition.X, 0),
                Math.Max(playerPosition.Y, 0)
                );

            if (lastMouse.LeftButton == ButtonState.Released && mouse.LeftButton == ButtonState.Pressed)
            {
                // shoot

                var bulletSpeed = 15f;

                var bullet = new Bullet();
                bullet.Position = playerPosition + new Vector2((float)Math.Cos(playerAngle), (float)Math.Sin(playerAngle)) * 0.25f;
                bullet.Velocity = new Vector2((float)Math.Cos(playerAngle), (float)Math.Sin(playerAngle)) * bulletSpeed;
                bullet.Expiration = DateTime.UtcNow.AddSeconds(10);
                bullets.Add(bullet);

                Assets.SoundEffects.Pistol.Play();
            }

            // todo make not suck
            bullets.RemoveAll(n => n.Expiration < DateTime.UtcNow);

            List<Bullet> hitBullets = new List<Bullet>();
            List<Zombie> deadZombies = new List<Zombie>();

            bullets.ForEach(n =>
            {
                n.Position += n.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
                var bulletBounds = new BoundingSphere(new Vector3(n.Position, 0), 0.1f);

                // check for hits
                foreach (var zombie in zombies)
                {
                    var bounds = new BoundingSphere(new Vector3(zombie.Position, 0), 0.15f);

                    if (bounds.Intersects(bulletBounds))
                    {
                        hitBullets.Add(n);
                        zombie.Health--;

                        Assets.SoundEffects.Zombie.Zombies[0].Play(); // r.Next(0, Assets.SoundEffects.Zombie.Zombies.Length)].Play();

                        if (zombie.Health <= 0)
                        {
                            deadZombies.Add(zombie);
                        }
                    }
                }
            });

            foreach (var bullet in hitBullets)
            {
                bullets.Remove(bullet);
            }

            foreach (var zombie in deadZombies)
            {
                zombies.Remove(zombie);
            }

            // wow this is total garbage...  TODO
            // rewrite all of this, later.

            if (zombies.Count > 0)
            {
                foreach (var luckyZombie in zombies)
                {
                    //var luckyZombie = zombies[r.Next(zombies.Count)];
                    luckyZombie.Angle = (float)Math.Atan2(playerPosition.Y - luckyZombie.Position.Y, playerPosition.X - luckyZombie.Position.X);

                    var zombieMove = new Vector2(1, 0);
                    var rot = Matrix.CreateRotationZ(luckyZombie.Angle);
                    luckyZombie.Position += Vector2.Transform(zombieMove, rot) * speed * 0.3f * (float)gameTime.ElapsedGameTime.TotalSeconds;

                    if (Vector2.DistanceSquared(luckyZombie.Position, playerPosition) < 0.2f && luckyZombie.AttackCooldown < DateTime.UtcNow)
                    {
                        // do attack thing
                        Console.WriteLine("attack");

                        luckyZombie.AttackCooldown = DateTime.UtcNow.AddSeconds(1.5);

                        playerHealth--;

                        if (playerHealth <= 0)
                        {
                            Exit(); // you lose
                        }
                    }
                }
            }

            var viewRect = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            var preferredRect = new Rectangle(viewRect.X + 300, viewRect.Y + 300, viewRect.Width - 600, viewRect.Height - 600);

            if (playerPosition.X * 64.0f + viewOffset.X < preferredRect.X)
            {
                viewOffset.X = preferredRect.X - playerPosition.X * 64.0f;
            }
            if (playerPosition.Y * 64.0f + viewOffset.Y < preferredRect.Y)
            {
                viewOffset.Y = preferredRect.Y - playerPosition.Y * 64.0f;
            }
            if (playerPosition.X * 64.0f + viewOffset.X > preferredRect.Right)
            {
                viewOffset.X = preferredRect.Right - playerPosition.X * 64.0f;
            }
            if (playerPosition.Y * 64.0f + viewOffset.Y > preferredRect.Bottom)
            {
                viewOffset.Y = preferredRect.Bottom - playerPosition.Y * 64.0f;
            }

            lastKeyboard = keyboard;
            lastMouse = mouse;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            int minx = (int)Math.Floor(-viewOffset.X / 64.0);
            int miny = (int)Math.Floor(-viewOffset.Y / 64.0);
            int maxx = (int)Math.Ceiling((Window.ClientBounds.Width - viewOffset.X) / 64.0);
            int maxy = (int)Math.Ceiling((Window.ClientBounds.Height - viewOffset.Y) / 64.0);

            for (int x = minx; x <= maxx; x++)
            {
                for (int y = miny; y < maxy; y++)
                {
                    var srcRect = new Rectangle(0, 0, 64, 64);

                    if (x >= 0 && y >= 0 && x < map.GetLength(0) && y < map.GetLength(1))
                    {
                        var tile = map[x, y];

                        if (tile == 1)
                            srcRect = new Rectangle(1152, 0, 0, 0);
                    }

                    spriteBatch.Draw(Assets.Sprites.TileSheet, new Rectangle((int)viewOffset.X + x * 64, (int)viewOffset.Y + y * 64, 64, 64), srcRect, Color.White);
                }
            }

            spriteBatch.Draw(Assets.Sprites.Character, playerPosition * 64f + new Vector2(-6, -6) + viewOffset, null, Color.FromNonPremultiplied(0, 0, 0, 64), playerAngle, new Vector2(17, 21), 1.0f, SpriteEffects.None, 0f);
            spriteBatch.Draw(Assets.Sprites.Character, playerPosition * 64f + viewOffset, null, Color.White, playerAngle, new Vector2(17, 21), 1.0f, SpriteEffects.None, 0f);


            foreach (var bullet in bullets)
            {
                spriteBatch.Draw(Assets.Sprites.Bullet, bullet.Position * 64f + viewOffset, new Rectangle(0, 0, 14, 14), Color.White, 0f, new Vector2(7, 7), 1f, SpriteEffects.None, 0f);
            }

            foreach (var zombie in zombies)
            {
                spriteBatch.Draw(Assets.Sprites.Zombie, zombie.Position * 64f + viewOffset, null, Color.White, zombie.Angle, new Vector2(16, 22), 1f, SpriteEffects.None, 0f);

                if (zombie.Health < zombie.MaxHealth)
                {
                    spriteBatch.Draw(pixelTexture, new Rectangle((int)(zombie.Position.X * 64f + viewOffset.X) - 16, (int)(zombie.Position.Y * 64f + viewOffset.Y) + 30, 32, 12), Color.FromNonPremultiplied(0, 0, 0, 96));
                    spriteBatch.Draw(pixelTexture, new Rectangle((int)(zombie.Position.X * 64f + viewOffset.X) - 15, (int)(zombie.Position.Y * 64f + viewOffset.Y) + 31, (zombie.Health * 30 / zombie.MaxHealth), 10), Color.FromNonPremultiplied(0, 255, 0, 128));
                }
            }

            spriteBatch.DrawString(Assets.Fonts.Yikes32pt, "Kill all the things", new Vector2(0, 0), Color.Black);

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }

}