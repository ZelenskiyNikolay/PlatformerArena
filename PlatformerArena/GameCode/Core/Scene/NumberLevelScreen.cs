using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using static System.Net.Mime.MediaTypeNames;

namespace Core
{
    public class NumberLevelScreen
    {
        private string _numberLevel;
        private float _timer;
        private Color _color;
        private Rectangle _rect;
        private Vector2 _positionText;
        private float _scale = 2f;
        public NumberLevelScreen(string numberLevel)
        {
            _numberLevel = "TEST ARENA ";// + numberLevel;
            _timer = 1f;
            _color = Color.Black;

            Vector2 dept = new Vector2(GameManager.Instance.Graphics.PreferredBackBufferWidth,
                                        GameManager.Instance.Graphics.PreferredBackBufferHeight);
            Vector2 textSize = GameManager.Instance.CoreFont.MeasureString(_numberLevel);
            _positionText = new Vector2(dept.X / 2 - (textSize.X / 2)*_scale,
                                            dept.Y / 2 - (textSize.Y / 2*_scale));
            _rect = new Rectangle(0, 0, (int)dept.X, (int)dept.Y);

        }
        public bool Update(float dt)
        {
            if (_timer <= 0)return false;

            if (_timer > 0f)
            {
                _timer -= 1 * dt;
                //_color.A = (byte)(_color.A - (255f*dt));
                _color.B = (byte)(_color.B + (500f * dt));
            }
            return true;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (_timer <= 0) return;

            spriteBatch.Draw(GameManager.Instance.Blank, _rect, _color);
            spriteBatch.DrawString(GameManager.Instance.CoreFont,_numberLevel,
                                 _positionText, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);


        }
    }
}

