using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace LudumDare41.ContentManagement
{
    public static class TrueTypeSpriteFontSpriteBatchEx
    {
        // TODO add more overloads

        public static void DrawString(this SpriteBatch spriteBatch, TrueTypeSpriteFont font, string text, Vector2 position, Color color)
        {
            font.DrawString(spriteBatch, text, position, color);
        }
    }
}
