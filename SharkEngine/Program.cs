﻿using System;

namespace SharkEngine
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            using var game = new Game();
            game.Run();
        }
    }
}