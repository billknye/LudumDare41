using System;
using System.Collections.Generic;
using System.Linq;
using LudumDare41.ContentManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LudumDare41.States
{
    public class PlayingState : GameState
    {
        const int tileSize = UniverseConfiguration.TileSize;

        KeyboardState lastKeyboard;
        MouseState lastMouse;

        List<Tuple<Rectangle, Point>> moveThings;

        Point viewOffset;
        Universe universe;
        private readonly GameStateManager gameStateManager;
        private readonly SpriteBatch spriteBatch;

        private List<Particle> particles;

        public PlayingState(GameStateManager gameStateManager, SpriteBatch spriteBatch, Universe universe)
        {
            this.gameStateManager = gameStateManager;
            this.spriteBatch = spriteBatch;
            this.universe = universe;
            this.particles = new List<Particle>();
        }

        public override void Entered()
        {
            moveThings = new List<Tuple<Rectangle, Point>>();
        }

        public override void Updated(GameTime gameTime)
        {
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            if (wasKeyJustPressed(Keys.Space, keyboard, lastKeyboard))
            {
                Assets.SoundEffects.Pickup.Play();
            }

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
            
            // Handle Jetpack on/off
            if(wasKeyJustPressed(Keys.Space, keyboard, lastKeyboard))
            {
                var player = universe.Player;
                if(!player.JetPackOn && player.JetPackFuel > 0)
                {
                    player.JetPackOn = true;
                }
                else if (player.JetPackOn)
                {
                    player.JetPackOn = false;
                }
            }

            if (dest != Point.Zero)
            {
                universe.DoTick(dest);

                if ((universe.Player.Oxygen <= 0) || (universe.Player.HitPoints <= 0))
                {
                    gameStateManager.Leave();
                    gameStateManager.Enter<GameOverState>();
                }
            }

            // particles update
            particles.RemoveAll(n => n.Expiration < gameTime.TotalGameTime);
            particles.ForEach(n => n.Position += n.Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds);

            for (int n = 0; n < 1; n++)
            {
                var dir = MathHelper.TwoPi * universe.Random.NextDouble();
                var mat = Matrix.CreateRotationZ((float)dir);

                var air = new Particle
                {
                    BaseColor = Color.Cyan,
                    Expiration = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(1)),
                    Position = universe.Player.Tile.Location.ToVector2() + new Vector2(0.5f, 0.2f),
                    Velocity = Vector2.Transform(new Vector2(1, 0), mat) * 2.0f,
                };

                particles.Add(air);
            }

            if (universe.Player.JetPackOn)
            {
                for (int n = 0; n < 5; n++)
                {
                    var dir = ((float)universe.Random.NextDouble() - 0.5f) * 0.2f;
                    var mat = Matrix.CreateRotationZ((float)dir);

                    var air = new Particle
                    {
                        BaseColor = Color.Orange,
                        Expiration = gameTime.TotalGameTime.Add(TimeSpan.FromSeconds(0.5f)),
                        Position = universe.Player.Tile.Location.ToVector2() + new Vector2(0.25f, 0.7f) + (universe.Player.LastMoveLeft ? new Vector2(0.5f, 0f) : Vector2.Zero),
                        Velocity = Vector2.Transform(new Vector2(0, 1), mat) * (2.0f + (float)universe.Random.NextDouble() * 0.5f),
                    };

                    particles.Add(air);
                }
            }

            var width = (int)Math.Ceiling(gameStateManager.Window.ClientBounds.Width / (double)UniverseConfiguration.TileSize);
            var height = (int)Math.Ceiling(gameStateManager.Window.ClientBounds.Height / (double)UniverseConfiguration.TileSize);

            // make player be center
            viewOffset = new Point(-width / 2 + universe.Player.Tile.Location.X, -height / 2 + universe.Player.Tile.Location.Y);
            
            lastMouse = mouse;
            lastKeyboard = keyboard;
        }

        private bool wasKeyJustPressed(Keys key, KeyboardState keyboard, KeyboardState lastKeyboard)
        {
            return keyboard.IsKeyDown(key) && lastKeyboard.IsKeyUp(key);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();

            var availableMoves = universe.GetAvailableMoves().ToList();

            moveThings.Clear();

            var tiles = new List<Tile>();

            universe.GetTilesInRange(universe.Player.Tile.Location.X, universe.Player.Tile.Location.Y, 5, tile =>
            {
                tiles.Add(tile);
            });

            // tiles
            foreach (var tile in tiles)
            {
                var spriteIndex = tile.Definition.SpriteIndex;
                if (spriteIndex == 8)
                    spriteIndex += Math.Abs((tile.Location.X * 23 + tile.Location.Y * 5) % 8);

                var light = tile.Light;

                spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((tile.Location.X - viewOffset.X) * tileSize, (tile.Location.Y - viewOffset.Y) * tileSize), new Rectangle((spriteIndex % 4) * tileSize, (spriteIndex / 4) * tileSize, tileSize, tileSize), getLightColor(light));

                if (availableMoves.Contains(tile))
                {
                    var loc = new Vector2((tile.Location.X - viewOffset.X) * tileSize, (tile.Location.Y - viewOffset.Y) * tileSize);
                    moveThings.Add(Tuple.Create(new Rectangle((int)loc.X, (int)loc.Y, tileSize, tileSize), tile.Location));
                    spriteBatch.Draw(Assets.Sprites.SampleSprite, loc, new Rectangle(192, 0, tileSize, tileSize),
                        Color.FromNonPremultiplied(0, 255, 0, 255 - (int)(Vector2.Distance(new Vector2(gameStateManager.GameWidth / 2, gameStateManager.GameHeight / 2), new Vector2(lastMouse.Position.X, lastMouse.Position.Y))))
                        );
                }

                spriteBatch.DrawString(Assets.Fonts.Japonesa16pt, $"{tile.Location.X},{tile.Location.Y}", new Vector2((tile.Location.X - viewOffset.X) * 64 + 2, (tile.Location.Y - viewOffset.Y) * 64 + 40), Color.Black);
            }

            // particles
            foreach (var particle in particles)
            {
                var color = Color.FromNonPremultiplied(particle.BaseColor.R, particle.BaseColor.G, particle.BaseColor.B, Math.Min(255, (int)((particle.Expiration - gameTime.TotalGameTime).TotalSeconds * 255.0)));
                spriteBatch.Draw(Assets.Sprites.PixelTexture, (particle.Position - viewOffset.ToVector2()) * UniverseConfiguration.TileSize, color);
            }

            // entities
            foreach (var tile in tiles)
            {
                var light = tile.Light;

                foreach (var entity in tile.Entities)
                {
                    var entSprite = entity.SpriteIndex;
                    spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((tile.Location.X - viewOffset.X) * UniverseConfiguration.TileSize, (tile.Location.Y - viewOffset.Y) * UniverseConfiguration.TileSize), new Rectangle((entSprite % 4) * tileSize, (entSprite / 4) * tileSize, UniverseConfiguration.TileSize, UniverseConfiguration.TileSize), getLightColor(light), 0f, Vector2.Zero, 1f, entity.SpriteEffects, 0f);
                }
            }


            // UI
            // jetpack bar
            spriteBatch.Draw(Assets.Sprites.PixelTexture, new Rectangle(0, 12, (int)(gameStateManager.GameWidth * universe.Player.JetPackFuel / universe.Player.MaxJetPackFuel), 12), Color.Aqua);

            // oxygen bar
            spriteBatch.Draw(Assets.Sprites.PixelTexture, new Rectangle(0, gameStateManager.GameHeight - 12, (int)(gameStateManager.GameWidth * universe.Player.Oxygen / universe.Player.MaxOxygen), 12), Color.DarkCyan);

            if (universe.Player.Oxygen < 30)
            {
                spriteBatch.Draw(Assets.Sprites.PixelTexture, new Rectangle(0, 0, gameStateManager.GameWidth, gameStateManager.GameHeight), Color.FromNonPremultiplied(0, 0, 0, (int)(30 - universe.Player.Oxygen) * 8));
            }

            // HitPoints bar
            spriteBatch.Draw(Assets.Sprites.PixelTexture, new Rectangle(0, 0, (int)(gameStateManager.GameWidth * universe.Player.HitPoints / UniverseConfiguration.PlayerInitialHP), 12), Color.Red);

            spriteBatch.End();
        }

        private Color getLightColor(int light)
        {
            var num = Math.Min(255, light * 32);
            num = Math.Max(32, num);

            return new Color(num, num, num);
        }
    }

    public class Particle
    {
        public TimeSpan Expiration;
        public Vector2 Velocity;
        public Vector2 Position;
        public Color BaseColor;
    }
}