using System.Numerics;
using Raylib_cs;
using SharkEngine.Core;

namespace SharkEngine.Scenes
{
    public class LightingScene : Scene
    {
        private Camera2D camera;
        private float cameraSpeed = 300f;

        private RenderTexture2D sceneTexture;
        private RenderTexture2D lightMap;

        private Vector2 originPosition;

        private float baseRadius = 100f;
        private float pulseTime = 0f;
        private float beamPulseTime = 0f;

        // Node data (can expand later with state)
        private Vector2[] nodePositions =
        [
            new Vector2(200, 150),
            new Vector2(600, 400),
            new Vector2(300, 500),
        ];

        public override void Load()
        {
            sceneTexture = Raylib.LoadRenderTexture(GameConfig.ScreenWidth, GameConfig.ScreenHeight);
            lightMap = Raylib.LoadRenderTexture(GameConfig.ScreenWidth, GameConfig.ScreenHeight);

            originPosition = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);

            camera = new Camera2D
            {
                Offset = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f),
                Target = originPosition,
                Rotation = 0.0f,
                Zoom = 1.0f
            };
        }

        public override void Unload()
        {
            Raylib.UnloadRenderTexture(sceneTexture);
            Raylib.UnloadRenderTexture(lightMap);
        }

        public override void Update(float deltaTime)
        {
            pulseTime += deltaTime;
            beamPulseTime += deltaTime;

            HandleCameraMovement(deltaTime);
        }

        private void HandleCameraMovement(float deltaTime)
        {
            Vector2 cameraMove = Vector2.Zero;

            if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D))
                cameraMove.X += 1;
            if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A))
                cameraMove.X -= 1;
            if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S))
                cameraMove.Y += 1;
            if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W))
                cameraMove.Y -= 1;

            if (cameraMove != Vector2.Zero)
                camera.Target += cameraMove * cameraSpeed * deltaTime;
        }

        public override void Draw()
        {
            float animatedRadius = baseRadius + 20f * MathF.Sin(pulseTime * 2f);
            float beamThickness = 4f + 2f * MathF.Sin(beamPulseTime * 4f);
            float beamAlpha = 0.5f + 0.5f * MathF.Sin(beamPulseTime * 4f);
            Color beamColor = Raylib.Fade(Color.White, beamAlpha);

            // Draw scene
            Raylib.BeginTextureMode(sceneTexture);
            Raylib.ClearBackground(Color.Pink);

            Raylib.BeginMode2D(camera);
            // World-space text or visuals can go here
            Raylib.EndMode2D();

            // Screen-space UI (optional)
            Raylib.DrawText("SCENE TEXTURE", 10, 10, 20, Color.White);
            Raylib.EndTextureMode();

            // Draw lightmap
            Raylib.BeginTextureMode(lightMap);
            Raylib.ClearBackground(Color.Black);

            Raylib.BeginMode2D(camera);

            foreach (var node in nodePositions)
            {
                Raylib.DrawLineEx(originPosition, node, beamThickness, beamColor);
                Raylib.DrawCircleGradient((int)node.X, (int)node.Y, 50, Color.White, Color.Blank);
            }

            Raylib.DrawCircleGradient((int)originPosition.X, (int)originPosition.Y, animatedRadius, Color.White, Color.Blank);

            Raylib.EndMode2D();
            Raylib.EndTextureMode();

            // Composite both
            Renderer.DrawRenderTexture(sceneTexture, 0, 0, Color.White);
            Renderer.DrawRenderTexture(lightMap, 0, 0, Color.White);
        }
    }
}
