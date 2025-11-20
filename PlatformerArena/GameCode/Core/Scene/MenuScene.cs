using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using Input;

namespace Core
{
    public class MenuScene : Scene
    {
        private ContentManager _content;
        private Texture2D _textureFon;
        private SpriteFont _font;
        private Chec _chec;
        private float _scale = 3;
        private Settings _settings;
        private Rectangle _FonDest;
        
        public ContentManager Content
        {
            get { return _content; }
        }
        public MenuScene(IServiceProvider ServiceProvider, string ContentPas = "Content/Scene/MenuScene")
        {
            _settings = new Settings();
            _settings.MenuActive = false;
            _content = new ContentManager(ServiceProvider, ContentPas);
        }

        public override void LoadContent()
        {
            _font = GameManager.Instance.CoreFont;
            _chec = Chec.NewGame;
            _textureFon = Content.Load<Texture2D>("Fon");
            _FonDest = new Rectangle(0,0,_settings.Dept.X, _settings.Dept.Y);
        }
        public override void Update(GameTime gameTime)
        {
            var input = InputManager.Instance;

            if (_settings.ChangeSettings)
            {
                _FonDest.Width = _settings.Dept.X;
                _FonDest.Height = _settings.Dept.Y;
                _settings.ChangeSettings = false;
            }

            if (!_settings.MenuActive)
            {
                if (input.IsKeyPressed(Keys.Up))
                    if (_chec != 0)
                        _chec--;
                if (input.IsKeyReleased(Keys.Down))
                    if (_chec != Chec.Exit)
                        _chec++;
                if(input.IsKeyPressed(Keys.Enter))
                {
                    if(_chec == Chec.Settings)
                    {
                        _settings.MenuActive = true;
                    }
                    if(_chec == Chec.Exit)
                    {
                        EventManager.Instance.Trigger(new ExitGameEvent());
                    }
                    if(_chec == Chec.NewGame)
                    {
                        EventManager.Instance.Trigger(new ChangeSceneEvent("Start"));
                    }
                }
            }
            else
                _settings.Update();
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_textureFon,_FonDest, Color.White);
            if (!_settings.MenuActive)
            {
                switch (_chec)
                {
                    case Chec.NewGame:
                        spriteBatch.DrawString(_font, "    MENU   \n" +
                                                      " -> New Game \n" +
                                                      "    Settings  \n" +
                                                      "    Exit"
                                                      , Vector2.Zero, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
                        break;
                    case Chec.Settings:
                        spriteBatch.DrawString(_font, "    MENU   \n" +
                                                      "    New Game \n" +
                                                      " -> Settings  \n" +
                                                      "    Exit"
                                                      , Vector2.Zero, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
                        break;
                    case Chec.Exit:
                        spriteBatch.DrawString(_font, "    MENU   \n" +
                                                      "    New Game \n" +
                                                      "    Settings  \n" +
                                                      " -> Exit"
                                                      , Vector2.Zero, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
                        break;
                }
            }
            else
                _settings.Draw(spriteBatch,_scale);
        }
        public override void Unload() => Content.Unload();
        private enum Chec
        {
            NewGame,
            Settings,
            Exit
        }
    }
}

