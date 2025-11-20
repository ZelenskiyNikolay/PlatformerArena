using Core;
using Entity;
using Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Core
{
    public class GameScene : Scene
    {
        public override void LoadContent()
        {
            PlayerData pd = LevelManager.Instance.PlayerDataGlobal;
            if (pd == null)
            {
                pd = new PlayerData();
                pd.Score = 0;
                pd.Lives = 1;
            }
            LevelManager.Instance.LoadNextLevel(GameManager.Instance.ServiceProvider, pd);

        }

        public override void Update(GameTime gameTime)
        {
            var input = InputManager.Instance;

            if (!LevelManager.Instance.GamePauseMemu.PauseMemu)
            {
                if (input.IsKeyPressed(Keys.Escape))
                    LevelManager.Instance.GamePauseMemu.PauseMemu = true;

                if (input.IsKeyPressed(Keys.N))
                {
                    LevelManager.Instance.LevelComplete();
                }

                LevelManager.Instance.Updete(gameTime);

                return;

            }
            if (LevelManager.Instance.GamePauseMemu.PauseMemu)
            {
                LevelManager.Instance.GamePauseMemu.Update();
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            LevelManager.Instance.Draw(spriteBatch);

        }

        public override void Unload()
        {
            LevelManager.Instance?.Dispose();
        }
    }
}
