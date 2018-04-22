using LudumDare41.Entities.Behavior;
using System;

namespace LudumDare41.Entities
{
    public class OxygenTank : Item
    {
        public override Type Behavior => typeof(OxygenTankBehavior);
    }
}