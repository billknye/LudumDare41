using LudumDare41.ContentManagement;
using Microsoft.Xna.Framework;
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
            public static Texture2D PixelTexture;
            public static Texture2D SampleSprite;
        }

        public static class Fonts
        {
            public static TrueTypeSpriteFont Japonesa16pt;
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

            Sprites.PixelTexture = new Texture2D(graphicsDevice, 1, 1); Sprites.PixelTexture.SetData(new Color[] { Color.White });
            Sprites.SampleSprite = contentManager.Get<Texture2D>("samplesprite").Content;

            Fonts.Japonesa16pt = contentManager.Get<TrueTypeFontLoader>("japonesa").Content.Create(16);

            Songs.Intro = contentManager.Get<Song>("song").Content;

            SoundEffects.Coin = contentManager.Get<SoundEffect>("coin").Content;
            SoundEffects.Coin2 = contentManager.Get<SoundEffect>("test/coin2").Content;
            SoundEffects.Coin3 = contentManager.Get<SoundEffect>("coin3").Content;
        }
    }
}
