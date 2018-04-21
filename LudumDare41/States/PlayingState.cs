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
        KeyboardState lastKeyboard;
        MouseState lastMouse;

        List<Tuple<Rectangle, Point>> moveThings;

        Point viewOffset;
        Universe universe;
        private readonly GameStateManager gameStateManager;
        private readonly SpriteBatch spriteBatch;

        public PlayingState(GameStateManager gameStateManager, SpriteBatch spriteBatch, Universe universe)
        {
            this.gameStateManager = gameStateManager;
            this.spriteBatch = spriteBatch;
            this.universe = universe;
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

            if (dest != Point.Zero)
            {
                universe.DoTick(dest);

                if ((universe.Player.Oxygen <= 0) || (universe.Player.HitPoints <= 0))
                {
                    gameStateManager.Leave();
                    gameStateManager.Enter<GameOverState>();
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

            universe.GetTilesInRange(universe.Player.Tile.Location.X, universe.Player.Tile.Location.Y, 5, tile =>
            {
                var spriteIndex = tile.Definition.SpriteIndex;
                if (spriteIndex == 8)
                    spriteIndex += Math.Abs((tile.Location.X * 23 + tile.Location.Y * 5) % 8);

                var light = tile.Light;

                spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((tile.Location.X - viewOffset.X) * 64, (tile.Location.Y - viewOffset.Y) * 64), new Rectangle((spriteIndex % 4) * 64, (spriteIndex / 4) * 64, 64, 64), getLightColor(light));

                if (availableMoves.Contains(tile))
                {
                    var loc = new Vector2((tile.Location.X - viewOffset.X) * 64, (tile.Location.Y - viewOffset.Y) * 64);
                    moveThings.Add(Tuple.Create(new Rectangle((int)loc.X, (int)loc.Y, 64, 64), tile.Location));
                    spriteBatch.Draw(Assets.Sprites.SampleSprite, loc, new Rectangle(192, 0, 64, 64),
                        Color.FromNonPremultiplied(0, 255, 0, 255 - (int)(Vector2.Distance(new Vector2(gameStateManager.GameWidth / 2, gameStateManager.GameHeight / 2), new Vector2(lastMouse.Position.X, lastMouse.Position.Y))))
                        );
                }

                spriteBatch.DrawString(Assets.Fonts.Japonesa16pt, $"{tile.Location.X},{tile.Location.Y}", new Vector2((tile.Location.X - viewOffset.X) * 64 + 2, (tile.Location.Y - viewOffset.Y) * 64 + 40), Color.Black);
                foreach (var entity in tile.Entities)
                {
                    var entSprite = entity.SpriteIndex;
                    spriteBatch.Draw(Assets.Sprites.SampleSprite, new Vector2((tile.Location.X - viewOffset.X) * UniverseConfiguration.TileSize, (tile.Location.Y - viewOffset.Y) * UniverseConfiguration.TileSize), new Rectangle((entSprite % 4) * 64, (entSprite / 4) * 64, UniverseConfiguration.TileSize, UniverseConfiguration.TileSize), getLightColor(light), 0f, Vector2.Zero, 1f, entity.SpriteEffects, 0f);
                }
            });

            // UI

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
}