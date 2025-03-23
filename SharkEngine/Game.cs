using System;
using Raylib_cs;
using SharkEngine.Core;
using SharkEngine.Scenes;

namespace SharkEngine
{
    public class Game : IDisposable
    {
        private SceneManager sceneManager;

        public Game()
        {
            Raylib.InitWindow(GameConfig.ScreenWidth, GameConfig.ScreenHeight, "SharkEngine Ritual");
            Raylib.SetTargetFPS(GameConfig.TargetFps);

            sceneManager = new SceneManager();
            sceneManager.ChangeScene(new LightingScene());
        }

        public void Run()
        {
            while (!Raylib.WindowShouldClose())
            {
                float deltaTime = Raylib.GetFrameTime();

                sceneManager.Update(deltaTime);

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.Black); // Global clear
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
