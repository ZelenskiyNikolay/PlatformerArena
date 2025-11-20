using Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class WinScene : Scene
    {
        private ContentManager _content;
        private Texture2D _textureFon, _textureGameOver;
        private Rectangle _FonDest, _gemeOverDest;

        private SpriteFont _font;
        private float _scale = 3;
        private int _score;
        private Vector2 _scorePosition = new(100, 50);
        private Vector2 _textPosition;
        private Vector2 _winPosition;

        private float _timer = 2f;
        private float _timerBlinc = 0.5f;
        private bool _blinc = false;
        public ContentManager Content
        {
            get { return _content; }
        }
        public WinScene(IServiceProvider ServiceProvider, string ContentPas = "Content/Scene/GameOverScene")
        {
            _content = new ContentManager(ServiceProvider, ContentPas);

        }
        public override void LoadContent()
        {
            _textureFon = Content.Load<Texture2D>("Fon");

            _FonDest = new Rectangle(0, 0, GameManager.Instance.Graphics.PreferredBackBufferWidth,
                                            GameManager.Instance.Graphics.PreferredBackBufferHeight);

            _textPosition = new Vector2(300, _FonDest.Height - 100);
            _font = GameManager.Instance.CoreFont;
            _score = LevelManager.Instance.PlayerDataGlobal.Score;

            var winText = "YOU WIN!!!";
            var size = _font.MeasureString(winText) * (_scale * 2);
            _winPosition = new Vector2(_FonDest.Width / 2f - size.X / 2f, _FonDest.Height / 3f);
        }
        public override void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_timer > 0)
            {
                _timer -= dt;
                return;
            }

            var input = InputManager.Instance;

            _timerBlinc -= dt;
            if (_timerBlinc < 0)
            {
                _timerBlinc = 0.5f;
                _blinc = !_blinc;
            }
            if (input.AnyKeyPressed())
            {
                LevelManager.Instance.PlayerDataGlobal.Score = 0;
                EventManager.Instance.Trigger(new ChangeSceneEvent("Menu"));
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_textureFon, _FonDest, Color.White);

            spriteBatch.DrawString(_font, "SCORE: " + _score,
                            _scorePosition, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);

            spriteBatch.DrawString(_font, "YOU WIN!!! ",
            _winPosition, Color.Green, 0f, Vector2.Zero, _scale * 2, SpriteEffects.None, 0f);

            if (_blinc)
                spriteBatch.DrawString(_font, "PRESS ANY KЕY... ",
                                _textPosition, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }
        public override void Unload() => Content.Unload();
    }
}

