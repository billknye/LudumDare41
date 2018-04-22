using LudumDare41.Entities.Behavior;
using System;

namespace LudumDare41.Entities
{
    public class BlasterItem : Item
    {
        public override Type Behavior => typeof(BlasterBehavior);


        public override int SpriteIndex => 9;
    }
}