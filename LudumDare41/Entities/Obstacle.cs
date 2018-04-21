using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare41.Entities
{
    public class Obstacle : Entity
    {
        public bool Destructible { get; set; }

        public override int SpriteIndex => 2;

        public override Type Behavior => typeof(Behavior.ObstacleAttackBehavior);
    }
}
