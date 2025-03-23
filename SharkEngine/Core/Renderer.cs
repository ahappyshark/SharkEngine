using Raylib_cs;
using System.Numerics;

namespace SharkEngine.Core
{
    public static class Renderer
    {
        public static void DrawRenderTexture(RenderTexture2D texture, int x, int y, Color tint)
        {
            Raylib.DrawTextureRec(
                texture.Texture,
                new Rectangle(0, 0, texture.Texture.Width, -texture.Texture.Height),
                new Vector2(x, y),
                tint
            );
        }
    }
}
