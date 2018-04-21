using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace LudumDare41.States
{
    public static class GameStateManager
    {
        static Stack<GameState> states;

        public static GameWindow Window { get; set; }

        public static int GameWidth
        {
            get
            {
                return Window.ClientBounds.Width;
            }
        }

        public static int GameHeight
        {
            get
            {
                return Window.ClientBounds.Height;
            }
        }

        static GameStateManager()
        {
            states = new Stack<GameState>();
        }

        public static void Enter(GameState state)
        {
            if (states.Count > 0)
            {
                states.Peek().Paused();
            }

            states.Push(state);
            state.Entered();
        }

        public static void Leave()
        {
            if (states.Count > 0)
            {
                states.Peek().Left();
                states.Pop();
            }
            else
            {
                throw new Exception("what?");
            }

            if (states.Count > 0)
            {
                states.Peek().Resumed();
            }
            else
            {
                // way hacky
                Environment.Exit(0);
            }
        }

        public static void Update(GameTime gameTime)
        {
            if (states.Count > 0)
            {
                states.Peek().Updated(gameTime);
            }
        }

        public static void Draw(GameTime gameTime)
        {
            if (states.Count > 0)
            {
                states.Peek().Draw(gameTime);
            }
        }
    }
}
