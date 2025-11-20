
using Entity;
using Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.IO;
using System.Reflection.Emit;

namespace Core
{
    public class LevelManager
    {
        private static LevelManager _instance;
        public static LevelManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LevelManager();
                return _instance;
            }
        }
        private PlayerData _playerDataGlobal;

        public GamePauseMemu GamePauseMemu;

        public Camera Camera;
        public PlayerData PlayerDataGlobal { get { return _playerDataGlobal; } }
        /// <summary>
        /// текущй уровень
        /// </summary>
        private int levelIndex = 0;
        /// <summary>
        /// Колличество доступных уровней.
        /// </summary>
        private const int numberOfLevels = 2;

        private IServiceProvider _serviceProvider;

        private Level _currentLevel;
        public Level Level { get { return _currentLevel; } }

        public bool Vin { get { return (levelIndex == numberOfLevels - 1); } }
        private LevelManager()
        {
            _playerDataGlobal = new PlayerData();
            GamePauseMemu = new GamePauseMemu(GameManager.Instance.CoreFont, GameManager.Instance.ScreenDept, GameManager.Instance.Blank);
            EventManager.Instance.Subscribe<LevelСompletedEvent>(LevelСompleted);
        }
        public void SaveScore(int score) => _playerDataGlobal.Score = score;
        public void LevelComplete()
        {
            _currentLevel.LevelComplete();
        }
        public void LevelСompleted(LevelСompletedEvent e)
        {

            _playerDataGlobal.Score = e.Score;
            LoadNextLevel(_serviceProvider, _playerDataGlobal);

        }
        public void LoadNextLevel(IServiceProvider ServiceProvider, PlayerData playerData)
        {
            if (_serviceProvider == null)
                _serviceProvider = ServiceProvider;

            // move to the next level
            levelIndex = (levelIndex + 1) % numberOfLevels;

            if (levelIndex == 0)
                levelIndex++;

            System.Diagnostics.Debug.WriteLine(levelIndex);

            if (_currentLevel != null)
                _currentLevel.Dispose();

            string levelPath = "Content/Levels";


            _currentLevel = new Level(ServiceProvider, levelPath, levelIndex, playerData);

        }
        public void Updete(GameTime gameTime)
        {
            _currentLevel.Updete(gameTime);
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            _currentLevel.Draw(spriteBatch);
        }
        public void Dispose()
        {
            if (Instance.Camera != null)
                Instance.Camera.SetZeroPosition();
            levelIndex = 0;
            _currentLevel.Dispose();
        }
    }
}

