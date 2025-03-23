using Raylib_cs;
using SharkEngine.Core;

namespace SharkEngine.Scenes
{
    public class TestScene : Scene
    {
        private float x, y;

        public override void Load()
        {
            // Called once when the scene is set active
            x = 400;
            y = 300;
        }

        public override void Unload()
        {
            // Called when the scene is replaced or closed
        }

        public override void Update(float deltaTime)
        {            
            // Move the "player" with arrow keys
            if (Raylib.IsKeyDown(KeyboardKey.Right)) x += 200 * deltaTime;
            if (Raylib.IsKeyDown(KeyboardKey.Left)) x -= 200 * deltaTime;
            if (Raylib.IsKeyDown(KeyboardKey.Down)) y += 200 * deltaTime;
            if (Raylib.IsKeyDown(KeyboardKey.Up)) y -= 200 * deltaTime;
        }

        public override void Draw()
        {
            Raylib.ClearBackground(Color.Black);
            Raylib.DrawCircle((int)x, (int)y, 20, Color.Yellow);
            Raylib.DrawText("Test Scene", 10, 10, 20, Color.White);
        }
    }
}
