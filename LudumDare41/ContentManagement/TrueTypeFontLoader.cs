using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TrueTypeSharp;

namespace LudumDare41.ContentManagement
{
    public class TrueTypeFontLoader
    {
        private TrueTypeFont font;
        private readonly GraphicsDevice graphicsDevice;

        public TrueTypeFontLoader(GraphicsDevice graphicsDevice, string filePath)
        {
            font = new TrueTypeFont(filePath);
            this.graphicsDevice = graphicsDevice;
        }

        public TrueTypeSpriteFont Create(float pixelSize)
        {
            var bitmap = font.BakeFontBitmap(pixelSize, out var chars);

            Color[] bitmapData = new Color[bitmap.Width * bitmap.Height];

            for (int x = 0; x < bitmap.Width; x++)
            {
                for (int y = 0; y < bitmap.Height; y++)
                {
                    bitmapData[x + y * bitmap.Width] = Color.FromNonPremultiplied(255, 255, 255, bitmap[x, y]);
                }
            }

            var texture = new Texture2D(graphicsDevice, bitmap.Width, bitmap.Height);
            texture.SetData(0, new Rectangle(0, 0, bitmap.Width, bitmap.Height), bitmapData, 0, bitmapData.Length);
            return new TrueTypeSpriteFont(texture, pixelSize, chars);
        }
    }
}
