using Raylib_cs;
using SharkEngine.Core;

namespace SharkEngine.Scenes
{
    public class LightingScene : Scene
    {
        private RenderTexture2D sceneTexture;
        private RenderTexture2D lightMap;
        private float x, y;

        public override void Load()
        {
            // Create two textures the size of the window
            sceneTexture = Raylib.LoadRenderTexture(800, 600);
            lightMap = Raylib.LoadRenderTexture(800, 600);

            // Start the "player" in the center
            x = 400;
            y = 300;
        }

        public override void Unload()
        {
            // Free the textures
            Raylib.UnloadRenderTexture(sceneTexture);
            Raylib.UnloadRenderTexture(lightMap);
        }

        public override void Update(float deltaTime)
        {            
            // Move the "player" with arrow keys
            if (Raylib.IsKeyDown(KeyboardKey.KEY_RIGHT)) x += 200 * deltaTime;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_LEFT)) x -= 200 * deltaTime;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_DOWN)) y += 200 * deltaTime;
            if (Raylib.IsKeyDown(KeyboardKey.KEY_UP)) y -= 200 * deltaTime;
        }

        public override void Draw()
        {
            // 1) Draw your scene/environment into sceneTexture
            Raylib.BeginTextureMode(sceneTexture);
            Raylib.ClearBackground(Color.DARKGRAY);

            // Example text
            Raylib.DrawText("Lighting Test Scene", 10, 10, 20, Color.WHITE);

            // "Player" or object
            Raylib.DrawCircle((int)x, (int)y, 20, Color.RED);

            Raylib.EndTextureMode();

            // 2) Create a light map: a black background with a transparent hole
            Raylib.BeginTextureMode(lightMap);
            Raylib.ClearBackground(Color.BLACK);

            // Draw a transparent circle around the player position
            // This "erases" the black so the scene behind shows through
            Raylib.DrawCircle((int)x, (int)y, 100, Color.BLANK);

            Raylib.EndTextureMode();

            // 3) Draw everything to the actual screen
            Raylib.BeginDrawing();
            Raylib.ClearBackground(Color.BLACK);

            // Draw the environment
            Raylib.DrawTexture(sceneTexture.texture, 0, 0, Color.WHITE);

            // Overlay the black texture (with the hole) on top
            Raylib.DrawTexture(lightMap.texture, 0, 0, Color.WHITE);

            Raylib.EndDrawing();
        }
    }
}
