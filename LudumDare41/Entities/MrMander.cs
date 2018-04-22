using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LudumDare41.Entities.Behavior;

namespace LudumDare41.Entities
{
    public class MrMander : Enemy
    {
        public override int SpriteIndex => 5;

        public override Type Behavior => typeof(MrManderAttackBehavior);

        public MrMander() : base()
        {

        }
    }
}
