using System;
using System.Text;
using System.Threading.Tasks;
using LudumDare41.ContentManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace LudumDare41.States
{
    public class IntroState : GameState
    {
        DateTime howLongMustYouBeHere;
        private readonly GameStateManager gameStateManager;
        private readonly SpriteBatch spriteBatch;

        public IntroState(GameStateManager gameStateManager, SpriteBatch spriteBatch)
        {
            this.gameStateManager = gameStateManager;
            this.spriteBatch = spriteBatch;
        }

        public override void Entered()
        {
            howLongMustYouBeHere = DateTime.UtcNow.AddSeconds(1);
            base.Entered();
        }

        public override void Updated(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            if (state.GetPressedKeys().Length > 0 && howLongMustYouBeHere < DateTime.UtcNow)
            {
                gameStateManager.Enter<PlayingState>();
            }

            base.Updated(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(Assets.Fonts.Japonesa16pt, "Welcome to some shit game.", new Vector2(300, 300), Color.Cyan);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }


}
