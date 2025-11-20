using Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace Core
{
    public class Settings
    {
        private ScreenDept _screenDept;
        private SelectScreenDept _select, _previousSelect;
        private GraphicsDeviceManager _graphics;
        private SpriteFont _font;
        public bool MenuActive { get; set; } = false;
        public bool ChangeSettings { get; set; } = false;
        public Point Dept { get { return _screenDept.ScreenDeptNov; } }
        public Settings()
        {
            _font = GameManager.Instance.CoreFont;
            _graphics = GameManager.Instance.Graphics;
            _screenDept = new();
            Point dept = new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);
            _screenDept.DefoltScreenDept = _screenDept.ScreenDeptNov = dept;
            _previousSelect = _select;
        }
        public void SetDept()
        {
            Set();
            _graphics.PreferredBackBufferWidth = _screenDept.ScreenDeptNov.X;
            _graphics.PreferredBackBufferHeight = _screenDept.ScreenDeptNov.Y;
            _graphics.ApplyChanges();
            GameManager.Instance.Graphics = _graphics;
        }

        private void Set()
        {
            switch (_select)
            {
                case SelectScreenDept.Svga: _screenDept.ScreenDeptNov = _screenDept.Svga; break;
                case SelectScreenDept.Xga: _screenDept.ScreenDeptNov = _screenDept.Xga; break;
                case SelectScreenDept.Hd: _screenDept.ScreenDeptNov = _screenDept.Hd; break;
            }
        }
        public void Update()
        {
            if (!MenuActive) return;
            var input = InputManager.Instance;
            if (input.IsKeyPressed(Keys.Up))
            {
                if (_select != SelectScreenDept.Svga)
                    _select--;
            }
            if (input.IsKeyPressed(Keys.Down))
            {
                if (_select != SelectScreenDept.Hd)
                    _select++;
            }
            if (input.IsKeyPressed(Keys.Enter))
            {
                SetDept();
                _previousSelect = _select;
                MenuActive = false;
                ChangeSettings = true;
            }
        }

        public void Draw(SpriteBatch spriteBatch, float scale)
        {
            switch (_select)
            {

                case SelectScreenDept.Svga:
                    spriteBatch.DrawString(_font, $" МЕНЮ : Settings       " +
                                                                          $"\n--> Svga(800X600) " +
                                                                          $"\n    Xga(1024X768)" +
                                                                          $"\n    Hd(1280X720)",
                                        new Vector2(5, 100), Color.Red, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f); break;
                case SelectScreenDept.Xga:
                    spriteBatch.DrawString(_font, $" МЕНЮ : Settings" +
                                                                          $"\n    Svga(800X600) " +
                                                                          $"\n--> Xga(1024X768)" +
                                                                          $"\n    Hd(1280X720)",
                                        new Vector2(5, 100), Color.Red, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f); break;
                case SelectScreenDept.Hd:
                    spriteBatch.DrawString(_font, $" МЕНЮ : Settings" +
                                                                          $"\n    Svga(800X600) " +
                                                                          $"\n    Xga(1024X768)" +
                                                                          $"\n--> Hd(1280X720)",
                                        new Vector2(5, 100), Color.Red, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f); break;

            }
        }

    }
    public class ScreenDept
    {
        public readonly Point Svga = new Point(800, 600);
        /// <summary>Разрешение 1024x768 (Xga)</summary>
        public readonly Point Xga = new Point(1024, 768);
        /// <summary>Разрешение 1280x720 (Hd)</summary>
        public readonly Point Hd = new Point(1280, 720);
        public Point ScreenDeptNov, DefoltScreenDept;
    }
    public enum SelectScreenDept
    {
        Svga,
        Xga,
        Hd
    }
}

