using Core;
using Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using static System.Net.Mime.MediaTypeNames;

namespace PlatformerArena
{
    public class GameCore : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public GameCore()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            GameManager.Instance.ServiceProvider = Services;
            GameManager.Instance.Graphics = _graphics;

        }

        protected override void Initialize()
        {
            EventManager.Instance.Subscribe<ExitGameEvent>(ExitGame);

            _graphics.PreferredBackBufferWidth = 1024;
            _graphics.PreferredBackBufferHeight = 768;
            _graphics.SynchronizeWithVerticalRetrace = true;
            //_graphics.IsFullScreen = true;
            _graphics.ApplyChanges();


            base.Initialize();
        }
        public void ExitGame(ExitGameEvent e)
        {
            GameManager.Instance.Unload();
            EventManager.Instance.Unsubscribe<ExitGameEvent>(ExitGame);
            Exit();
        }


        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            GameManager.Instance.CreateBlancTexture(_graphics);
            GameManager.Instance.CoreFont = Content.Load<SpriteFont>("Font/OldFont");
            GameManager.Instance.LoadGame();
            Fps.Font = GameManager.Instance.CoreFont;
            Fps.textPosition.Y = 50;
        }

        protected override void Update(GameTime gameTime)
        {
            //if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            //    Exit();

            InputManager.Instance.Update();
            GameManager.Instance.Update(gameTime);
            // TODO: Add your update logic here
            Fps.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            if (LevelManager.Instance.Camera == null)
                _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            else
                _spriteBatch.Begin(
                   samplerState: SamplerState.PointClamp,
                   transformMatrix: LevelManager.Instance.Camera.Transform);

            GameManager.Instance.RenderGame(_spriteBatch);

            _spriteBatch.End();


            if (LevelUI.Instance != null)
                if (LevelUI.Instance.IsUiPrinted)
                {
                    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

                    if (LevelManager.Instance.GamePauseMemu != null)
                        if (LevelManager.Instance.GamePauseMemu.PauseMemu)
                            LevelManager.Instance.GamePauseMemu.Draw(_spriteBatch);


                    LevelUI.Instance.Draw(_spriteBatch);

                    Fps.DrawFps(_spriteBatch);
                    _spriteBatch.End();

                }
            Fps.FramePrinted();

            base.Draw(gameTime);
        }
    }
}

