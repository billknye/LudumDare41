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
        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            var contentManager = new CustomContentManager(graphicsDevice, "Content");

            Sprites.SampleSprite = contentManager.Get<Texture2D>("samplesprite").Content;

            Fonts.Yikes32pt = contentManager.Get<TrueTypeFontLoader>("yikes").Content.Create(32);

            Songs.Intro = contentManager.Get<Song>("song").Content;

            SoundEffects.Coin = contentManager.Get<SoundEffect>("coin").Content;
            SoundEffects.Coin2 = contentManager.Get<SoundEffect>("test/coin2").Content;
            SoundEffects.Coin3 = contentManager.Get<SoundEffect>("coin3").Content;
        }
    }
}
