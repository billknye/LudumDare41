using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using TrueTypeSharp;

namespace LudumDare41.ContentManagement
{

    public class TrueTypeSpriteFont
    {
        Texture2D texture;
        float pixelSize;
        Dictionary<char, Glyph> glyphs;
        

        public TrueTypeSpriteFont(Texture2D texture, float pixelSize, BakedCharCollection chars)
        {
            this.texture = texture;
            this.pixelSize = pixelSize;
            this.glyphs = new Dictionary<char, Glyph>();

            foreach (var ch in chars.Characters)
            {
                glyphs.Add(ch.Key, new Glyph
                {
                    Ch = ch.Key,
                    Source = new Rectangle(ch.Value.X0, ch.Value.Y0, ch.Value.X1 - ch.Value.X0, ch.Value.Y1 - ch.Value.Y0),
                    XAdvance = ch.Value.XAdvance,
                    XOffset = ch.Value.XOffset,
                    YOffset = ch.Value.YOffset
                });
            }
        }

        // TODO add more overloads

        public void DrawString(SpriteBatch spriteBatch, string text, Vector2 position, Color color)
        {
            float x = 0;

            foreach (var ch in text)
            {
                var src = glyphs[ch];
                spriteBatch.Draw(texture, position + new Vector2(x + src.XOffset, pixelSize + src.YOffset), src.Source, color, 0f, new Vector2(0, 0), 1f, SpriteEffects.None, 0f);
                x += src.XAdvance;
            }
        }

        public float MeasureString(string text)
        {
            float x = 0;

            foreach (var ch in text)
            {
                var src = glyphs[ch];
                x += src.XAdvance;
            }

            return x;
        }

        internal class Glyph
        {
            public char Ch;
            public Rectangle Source;            
            public float XOffset;
            public float YOffset;
            public float XAdvance;
        }
    }
}
