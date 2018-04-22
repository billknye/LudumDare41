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
            public static Texture2D PlanetSprites;
            public static Texture2D Background;
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
            public static SoundEffect Pickup;
            public static SoundEffect Punch;
        }

        public static void Initialize(GraphicsDevice graphicsDevice)
        {
            var contentManager = new CustomContentManager(graphicsDevice, "Content");

            Sprites.PixelTexture = new Texture2D(graphicsDevice, 1, 1); Sprites.PixelTexture.SetData(new Color[] { Color.White });
            Sprites.Background = contentManager.Get<Texture2D>("bg").Content;
            Sprites.SampleSprite = contentManager.Get<Texture2D>("samplesprite").Content;
            Sprites.PlanetSprites = contentManager.Get<Texture2D>("planets").Content;

            Fonts.Japonesa16pt = contentManager.Get<TrueTypeFontLoader>("japonesa").Content.Create(16);

            Songs.Intro = contentManager.Get<Song>("song").Content;

            SoundEffects.Pickup = contentManager.Get<SoundEffect>("pickup").Content;
            SoundEffects.Punch = contentManager.Get<SoundEffect>("punch").Content;
        }
    }
}
