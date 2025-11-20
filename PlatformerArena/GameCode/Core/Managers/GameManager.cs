using Input;
using Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;


namespace Core
{
    public enum GameState
    {
        Start,
        Menu,
        Play,
        GameOver,
        Win
    }
    public class GameManager
    {
        private static GameManager _instance;
        public IServiceProvider ServiceProvider;
        public GraphicsDeviceManager Graphics;

        public SpriteFont CoreFont;
        public Texture2D Blank;

        private Scene _currentScene;

        private GameState _state;

        public Point ScreenDept
        {
            get
            {
                return new Point(Graphics.PreferredBackBufferWidth, Graphics.PreferredBackBufferHeight);
            }
        }
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new GameManager();
                return _instance;
            }
        }
        private GameManager()
        {
            _state = GameState.Start;
            EventManager.Instance.Subscribe<ChangeSceneEvent>(ChangeScene);
        }
        public void CreateBlancTexture(GraphicsDeviceManager graphics)
        {
            Blank = new Texture2D(graphics.GraphicsDevice, 1, 1);
            Blank.SetData(new[] { Color.White });
        }
        public void ChangeScene()
        {
            LevelUI.Instance.IsUiPrinted = false;
            switch (_state)
            {
                case GameState.Menu:

                    _currentScene?.Dispose();
                    _currentScene = new MenuScene(ServiceProvider);
                    _currentScene.LoadContent();

                    break;
                case GameState.Start:

                    _currentScene?.Dispose();
                    _currentScene = new GameScene();
                    _currentScene.LoadContent();

                    break;
                case GameState.Play:
                    LevelUI.Instance.IsUiPrinted = true;
                    _currentScene?.Dispose();
                    _currentScene = new GameScene();
                    _currentScene.LoadContent();

                    break;

            }
        }
        public void ChangeScene(ChangeSceneEvent e)
        {
            LevelUI.Instance.IsUiPrinted = false;
            if (e.SceneName == "Start")
            {
                LevelUI.Instance.IsUiPrinted = true;
                _currentScene?.Dispose();
                _currentScene = new GameScene();
                _currentScene.LoadContent();
                _state = GameState.Play;
            }
            if (e.SceneName == "Menu")
            {
                _currentScene?.Dispose();
                _currentScene = new MenuScene(ServiceProvider);
                _currentScene.LoadContent();
                _state = GameState.Menu;
            }
            if (e.SceneName == "GameOver")
            {
                _currentScene?.Dispose();
                _currentScene = new GameOverScene(ServiceProvider);
                _currentScene.LoadContent();
                _state = GameState.GameOver;
            }
            if (e.SceneName == "Win")
            {
                _currentScene?.Dispose();
                _currentScene = new WinScene(ServiceProvider);
                _currentScene.LoadContent();
                _state = GameState.Win;
            }
        }
        public void Update(GameTime gameTime)
        {
            var input = InputManager.Instance;

            _currentScene.Update(gameTime);
        }
        public void RenderGame(SpriteBatch spriteBatch)
        {
            _currentScene.Draw(spriteBatch);
        }
        public void LoadGame()
        {
            _currentScene = new MenuScene(ServiceProvider);
            _currentScene.LoadContent();
        }
        public void Unload()
        {
            EventManager.Instance.Unsubscribe<ChangeSceneEvent>(ChangeScene);
        }
    }
}


