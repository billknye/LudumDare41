using Microsoft.Xna.Framework;

namespace LudumDare41
{
    internal class Zombie
    {
        public Vector2 Position;
        public float Angle;

        public int Health, MaxHealth;

        // todo A to the I

        public Zombie()
        {
            Health = MaxHealth = 10;
        }
    }
}