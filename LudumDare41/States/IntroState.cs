using System;
using System.Text;
using System.Threading.Tasks;
using LudumDare41.ContentManagement;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace LudumDare41.States
{
    public class IntroState : GameState
    {
        DateTime howLongMustYouBeHere;

        public IntroState()
        {
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
                GameStateManager.Enter(new PlayingState());
            }

            base.Updated(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Assets.Game.SpriteBatch.Begin();
            Assets.Game.SpriteBatch.DrawString(Assets.Fonts.Japonesa16pt, "Welcome to some shit game.", new Vector2(300, 300), Color.Cyan);
            Assets.Game.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }


}
