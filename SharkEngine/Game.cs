using System;
using Raylib_cs;
using SharkEngine.Core;
using SharkEngine.Scenes;

namespace SharkEngine
{
    public class Game : IDisposable
    {
        private const int ScreenWidth = 800;
        private const int ScreenHeight = 600;
        private const int TargetFps = 60;

        private SceneManager sceneManager;

        public Game()
        {
            Raylib.InitWindow(ScreenWidth, ScreenHeight, "SharkEngine Ritual");
            Raylib.SetTargetFPS(TargetFps);

            // Initialize SceneManager with an initial Scene
            sceneManager = new SceneManager();
            sceneManager.ChangeScene(new TestScene());
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime();

                // Update the current scene
                sceneManager.Update(deltaTime);

                // Draw
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.BLACK);

                sceneManager.Draw();

                Raylib.EndDrawing();
            }
        }

        public void Dispose()
        {
            Raylib.CloseWindow();
        }
    }
}
