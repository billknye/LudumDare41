using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

using LudumDare41.Entities.Behavior;

namespace LudumDare41.Entities
{
    public class Squidward : Enemy
    {
        public override int SpriteIndex => 6;

        public override Type Behavior => typeof(SquidwardAttackBehavior);

        public Squidward() : base()
        {

        }
    }
}
