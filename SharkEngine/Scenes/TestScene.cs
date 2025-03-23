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
            // Movement logic
            if (Input.IsKeyDown(KeyboardKey.KEY_RIGHT)) x += 200 * deltaTime;
            if (Input.IsKeyDown(KeyboardKey.KEY_LEFT)) x -= 200 * deltaTime;
            if (Input.IsKeyDown(KeyboardKey.KEY_DOWN)) y += 200 * deltaTime;
            if (Input.IsKeyDown(KeyboardKey.KEY_UP)) y -= 200 * deltaTime;
        }

        public override void Draw()
        {
            Raylib.DrawCircle((int)x, (int)y, 20, Color.YELLOW);
            Raylib.DrawText("Test Scene", 10, 10, 20, Color.WHITE);
        }
    }
}
