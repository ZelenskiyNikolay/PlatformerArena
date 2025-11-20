using Entity;
using Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Runtime;


namespace Core
{
    public class GameOverScene : Scene
    {
        private ContentManager _content;
        private Texture2D _textureFon, _textureGameOver;
        private Rectangle _FonDest, _gemeOverDest;

        private SpriteFont _font;
        private float _scale = 3;
        private int _score;
        private Vector2 _scorePosition = new(100, 50);
        private Vector2 _textPosition;

        private float _timer = 2f;
        private float _timerBlinc = 0.5f;
        private bool _blinc = false;
        public ContentManager Content
        {
            get { return _content; }
        }
        public GameOverScene(IServiceProvider ServiceProvider, string ContentPas = "Content/Scene/GameOverScene")
        {
            _content = new ContentManager(ServiceProvider, ContentPas);

        }
        public override void LoadContent()
        {
            _textureFon = Content.Load<Texture2D>("Fon");
            _textureGameOver = Content.Load<Texture2D>("GameOver");
            _FonDest = new Rectangle(0, 0, GameManager.Instance.Graphics.PreferredBackBufferWidth,
                                            GameManager.Instance.Graphics.PreferredBackBufferHeight);
            _gemeOverDest = new Rectangle(50, 150, _FonDest.Width - 100, _FonDest.Height - 300);
            _textPosition = new Vector2(300, _FonDest.Height - 100);
            _font = GameManager.Instance.CoreFont;
            _score = LevelManager.Instance.PlayerDataGlobal.Score;
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

            spriteBatch.Draw(_textureGameOver, _gemeOverDest, Color.White);

            if (_blinc)
                spriteBatch.DrawString(_font, "PRESS ANY KЕY... ",
                                _textPosition, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
        }
        public override void Unload() => Content.Unload();
    }
}

