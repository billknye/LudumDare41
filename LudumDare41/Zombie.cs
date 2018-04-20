using Microsoft.Xna.Framework;
using System;

namespace LudumDare41
{
    internal class Zombie
    {
        public Vector2 Position;
        public float Angle;

        public int Health, MaxHealth;

        public DateTime AttackCooldown;

        // todo A to the I

        public Zombie()
        {
            Health = MaxHealth = 10;
        }
    }
}