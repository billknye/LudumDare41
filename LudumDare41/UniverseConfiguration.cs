namespace LudumDare41
{
    public static class UniverseConfiguration
    {
        // Environment
        public const int TileSize = 64;
        public const int ObstacleMinDamage = 15;
        public const int ObstacleMaxDamage = 40;
        public const int MinNumberOfObstacles = 100;
        public const int MaxNumberOfObstacles = 100;


        // Squidward 
        public const int SquidwardMinHP = 100;
        public const int SquidwardMaxHP = 10;
        public const int SquidwardBaseAttack = 10;
        public const int SquidwardNumberOfEnemies = 75;

        // MrMander
        public const int MrManderMinHP = 100;
        public const int MrManderMaxHP = 10;
        public const int MrManderBaseAttack = 20;
        public const int MrManderNumberOfEnemies = 25;

        // Player
        public const int PlayerInitialHP = 100;
        public const int PlayerBaseAttack = 10;
        public const int PlayerMaxJetPackFuel = 100;
        public const int PlayerJetPackIncrease = 20;
        public const int PlayerJetPackDecrease = 20;

        public const float MoveTime = 0.2f;
        public const int PlayerMaxOxigen = 100;
        public const int ItemOxygenTankAmountToRefill = 10;

        public static int TotalEnemies { get { return SquidwardNumberOfEnemies + MrManderNumberOfEnemies; } }
    }
}