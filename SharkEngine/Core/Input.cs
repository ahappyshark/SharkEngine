using Raylib_cs;

namespace SharkEngine.Core
{
    public static class Input
    {
        public static bool IsKeyDown(KeyboardKey key) => Raylib.IsKeyDown(key);
        public static bool IsKeyPressed(KeyboardKey key) => Raylib.IsKeyPressed(key);
        // etc...
    }
}
