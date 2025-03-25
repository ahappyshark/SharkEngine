using System.Numerics;
using Raylib_cs;
using SharkEngine.Core;
using SharkEngine.Gameplay;

namespace SharkEngine.Core
{
    public class GameCamera
    {
        private Camera2D _camera;
        public ref Camera2D Camera => ref _camera;

        private float cameraSpeed = 300f;
        private float cameraZoom = 1.0f;
        private const float ZoomSpeed = 0.1f;
        private const float MinZoom = 0.25f;
        private const float MaxZoom = 3.0f;

        public GameCamera()
        {
            Vector2 originPosition = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f);
            _camera = new Camera2D
            {
                Offset = new Vector2(GameConfig.ScreenWidth / 2f, GameConfig.ScreenHeight / 2f),
                Target = originPosition,
                Rotation = 0.0f,
                Zoom = 1.0f
            };
        }

        public void Update(float deltaTime)
        {
            HandleCameraMovement(deltaTime);
            HandleCameraZoom(deltaTime);
        }

        private void HandleCameraMovement(float deltaTime)
        {
            Vector2 cameraMove = Vector2.Zero;

            if (Raylib.IsKeyDown(KeyboardKey.Right) || Raylib.IsKeyDown(KeyboardKey.D)) cameraMove.X += 1;
            if (Raylib.IsKeyDown(KeyboardKey.Left) || Raylib.IsKeyDown(KeyboardKey.A)) cameraMove.X -= 1;
            if (Raylib.IsKeyDown(KeyboardKey.Down) || Raylib.IsKeyDown(KeyboardKey.S)) cameraMove.Y += 1;
            if (Raylib.IsKeyDown(KeyboardKey.Up) || Raylib.IsKeyDown(KeyboardKey.W)) cameraMove.Y -= 1;

            if (cameraMove != Vector2.Zero)
                Camera.Target += cameraMove * cameraSpeed * deltaTime;
        }

        private void HandleCameraZoom(float deltaTime)
        {
            float wheel = Raylib.GetMouseWheelMove();

            if (wheel != 0)
            {
                Vector2 mouseScreen = Raylib.GetMousePosition();
                Vector2 mouseWorldBefore = Raylib.GetScreenToWorld2D(mouseScreen, Camera);

                cameraZoom += wheel * ZoomSpeed;
                cameraZoom = Math.Clamp(cameraZoom, MinZoom, MaxZoom);
                Camera.Zoom = cameraZoom;

                Vector2 mouseWorldAfter = Raylib.GetScreenToWorld2D(mouseScreen, Camera);
                Vector2 delta = mouseWorldBefore - mouseWorldAfter;
                Camera.Target += delta;
            }
        }
    }
}