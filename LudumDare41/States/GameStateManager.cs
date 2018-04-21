using DryIoc;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace LudumDare41.States
{
    public class GameStateManager
    {
        Stack<GameState> states;
        private readonly Container container;

        public GameWindow Window { get; }

        public int GameWidth
        {
            get
            {
                return Window.ClientBounds.Width;
            }
        }

        public int GameHeight
        {
            get
            {
                return Window.ClientBounds.Height;
            }
        }


        public GameStateManager(Container container, GameWindow gameWindow)
        {
            states = new Stack<GameState>();
            this.container = container;
            this.Window = gameWindow;
        }

        public void Enter<T>() where T : GameState
        {
            var instance = container.New<T>();
            Enter(instance);
        }

        public void Enter(GameState state)
        {
            if (states.Count > 0)
            {
                states.Peek().Paused();
            }

            states.Push(state);
            state.Entered();
        }

        public void Leave()
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

        public void Update(GameTime gameTime)
        {
            if (states.Count > 0)
            {
                states.Peek().Updated(gameTime);
            }
        }

        public void Draw(GameTime gameTime)
        {
            if (states.Count > 0)
            {
                states.Peek().Draw(gameTime);
            }
        }
    }
}
