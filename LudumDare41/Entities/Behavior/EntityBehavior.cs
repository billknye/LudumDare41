using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare41.Entities.Behavior
{
    public abstract class EntityBehavior
    {
        public EntityBehavior()
        {
        }
        public abstract void Tick(Entity entity);
    }
}