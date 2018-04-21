using Microsoft.Xna.Framework;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare41.States
{
    public abstract class GameState
    {

        public GameState()
        {
        }

        public virtual void Updated(GameTime gameTime)
        {

        }

        public virtual void Draw(GameTime gameTime)
        {

        }

        public virtual void Entered()
        {

        }

        public virtual void Left()
        {

        }

        public virtual void Paused()
        {

        }

        public virtual void Resumed()
        {

        }
    }
}
