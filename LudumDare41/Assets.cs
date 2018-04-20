using LudumDare41.ContentManagement;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LudumDare41
{
    public static class Assets
    {
        public static class Sprites
        {
            public static Texture2D SampleSprite;
            public static Texture2D TileSheet;
            public static Texture2D Character;
            public static Texture2D Bullet;
            public static Texture2D Zombie;
        }

        public static class Fonts
        {
            public static TrueTypeSpriteFont Yikes32pt;
        }

        public static class Songs
        {
            public static Song Intro;
        }

        public static class SoundEffects
        {
            public static SoundEffect Coin;
            public static SoundEffect Coin2;
            public static SoundEffect Coin3;
            public static SoundEffect Pistol;

            public static class Zombie
            {
                public static SoundEffect[] Zombies;

            }
        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            var contentManager = new CustomContentManager(graphicsDevice, "Content");

            // todo, make this not the worst perfomance thing possible...
            Sprites.SampleSprite = contentManager.Get<Texture2D>("samplesprite").Content;
            Sprites.TileSheet = contentManager.Get<Texture2D>("tilesheet").Content;
            Sprites.Character = contentManager.Get<Texture2D>("character").Content;
            Sprites.Bullet = contentManager.Get<Texture2D>("bullet").Content;
            Sprites.Zombie = contentManager.Get<Texture2D>("zombie").Content;

            Fonts.Yikes32pt = contentManager.Get<TrueTypeFontLoader>("yikes").Content.Create(32);

            Songs.Intro = contentManager.Get<Song>("song").Content;

            SoundEffects.Coin = contentManager.Get<SoundEffect>("coin").Content;
            SoundEffects.Coin2 = contentManager.Get<SoundEffect>("test/coin2").Content;
            SoundEffects.Coin3 = contentManager.Get<SoundEffect>("coin3").Content;
            SoundEffects.Pistol = contentManager.Get<SoundEffect>("pistol").Content;

            SoundEffects.Zombie.Zombies = new SoundEffect[] {
                /*contentManager.Get<SoundEffect>("zombie/zombie-1").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-2").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-3").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-4").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-5").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-6").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-7").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-8").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-9").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-10").Content,*/
                contentManager.Get<SoundEffect>("zombie/zombie-1").Content,  // why you work with hacky ogg converter but not a wav.....
                /*contentManager.Get<SoundEffect>("zombie/zombie-12").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-13").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-14").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-15").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-16").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-17").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-18").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-19").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-20").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-21").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-22").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-23").Content,
                contentManager.Get<SoundEffect>("zombie/zombie-24").Content,*/
            };
        }
    }
}
