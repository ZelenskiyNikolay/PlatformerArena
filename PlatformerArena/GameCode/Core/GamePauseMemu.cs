using Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace Core
{
    public class GamePauseMemu
    {
        public  static GamePauseMemu _instance;

        public bool PauseMemu;
        private float _scale;
        private Vector2 _dest;
        private SpriteFont _font;
        private Menu _chec;
        private Texture2D _texture;
        private Rectangle _destRect;
        public GamePauseMemu(SpriteFont font, Point Dept,Texture2D texture2D)
        {
            _scale = 3.0f;
            PauseMemu = false;
            _font = font;
            _texture = texture2D;

            _chec = Menu.Continue;

            Vector2 menuSize = _font.MeasureString(str);
            _dest = new Vector2((Dept.X / 2 - menuSize.X / 2) / _scale, 100);
            _destRect = new Rectangle((int)_dest.X - 20,(int)_dest.Y - 20,(int)(menuSize.X*_scale+20),(int)(menuSize.Y*_scale+20));
            _instance = this;
        }
        public void Update()
        {
            var input = InputManager.Instance;

            if (PauseMemu) 
            {
                if(input.IsKeyPressed(Keys.Up))
                    if (_chec != Menu.Continue)
                        _chec--;
                if (input.IsKeyPressed(Keys.Down))
                    if (_chec != Menu.Exit)
                        _chec++;
                if (input.IsKeyPressed(Keys.Enter))
                {
                    if (_chec == Menu.Continue)
                        PauseMemu = false;
                    if (_chec == Menu.Exit)
                        EventManager.Instance.Trigger(new ExitGameEvent());
                    if (_chec == Menu.ReturnMenu)
                    {
                        PauseMemu = false;
                        LevelManager.Instance.Camera = null;
                        EventManager.Instance.Trigger(new ChangeSceneEvent("Menu"));
                    }
                }
                if (input.IsKeyPressed(Keys.Escape))
                    PauseMemu = false;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (PauseMemu)
            {
                spriteBatch.Draw(_texture,_destRect, Color.Black * 0.3f);

                switch (_chec)
                {
                    case Menu.Continue:
                        spriteBatch.DrawString(_font, "          Pause   \n" +
                                                                  " -> Сontinue the Game \n" +
                                                                  "    Return to main menu  \n" +
                                                                  "    Exit Game"
                                                                  , _dest, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
                        break;
                    case Menu.ReturnMenu:
                        spriteBatch.DrawString(_font, "          Pause   \n" +
                                                                  "    Сontinue the Game \n" +
                                                                  " -> Return to main menu  \n" +
                                                                  "    Exit Game"
                                                                  , _dest, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
                        break;
                    case Menu.Exit:
                        spriteBatch.DrawString(_font, "          Pause   \n" +
                                                                  "    Сontinue the Game \n" +
                                                                  "    Return to main menu  \n" +
                                                                  " -> Exit Game"
                                                                  , _dest, Color.Red, 0f, Vector2.Zero, _scale, SpriteEffects.None, 0f);
                        break;
                }
            }
        }
        private string str = "    Pause   \n -> Сontinue the Game \n    Return to main menu  \n    Exit Game";
        private enum Menu
        {
            Continue,
            ReturnMenu,
            Exit
        }
    }
}

