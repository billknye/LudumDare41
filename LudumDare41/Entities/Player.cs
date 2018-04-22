using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace LudumDare41.Entities
{
    public class Player : Entity
    {
        public int HitPoints { get; set; }
        public int BaseAttack { get; set; }
        public int ModifierAttack { get; set; }
        public Point Velocity { get; set; }

        public float Oxygen { get; set; }
        public float MaxOxygen { get; set; }
        public int MaxHitPoints { get; set; }

        public bool LastMoveLeft { get; set; }

        public bool JetPackOn { get; set; }

        public int JetPackFuel { get; set; }

        public int MaxJetPackFuel { get; set; }

        public int JetPackIncreaseFuel { get; set; }
        public int JetPackDecreaseFuel { get; set; }

        public override int SpriteIndex => 0;

        public override int LightEmitted => 4;

        public override SpriteEffects SpriteEffects => (LastMoveLeft ? SpriteEffects.FlipHorizontally : SpriteEffects.None);

        public Player()
        {
            Oxygen = MaxOxygen = UniverseConfiguration.PlayerMaxOxigen;
            HitPoints = MaxHitPoints = UniverseConfiguration.PlayerInitialHP;
            JetPackFuel = MaxJetPackFuel = UniverseConfiguration.PlayerMaxJetPackFuel;

            JetPackIncreaseFuel = UniverseConfiguration.PlayerJetPackIncrease;
            JetPackDecreaseFuel = UniverseConfiguration.PlayerJetPackDecrease;

            JetPackOn = false;
        }
    }
}
