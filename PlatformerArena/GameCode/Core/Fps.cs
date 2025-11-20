using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Timers;
using System;

namespace Core
{
    static class Fps
    {
        static public SpriteFont Font { get; set; }
        private static double _elapsedTime;
        static private int _fpsCount;
        static private int _fpsValue = 0;
        static Color color = Color.FromNonPremultiplied(0, 255, 0, 256);
        public static Vector2 textPosition = new Vector2(0, 0);
        static public void FramePrinted() { _fpsCount++; }
        public static void Update(GameTime gameTime)
        {
            _elapsedTime += gameTime.ElapsedGameTime.TotalSeconds;

            if (_elapsedTime >= 1.0)
            {
                _fpsValue = _fpsCount;
                _fpsCount = 0;
                _elapsedTime = 0;
            }
        }
        static public void DrawFps(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(Font, _fpsValue + " FPS.", textPosition, color);
        }
    }
}




