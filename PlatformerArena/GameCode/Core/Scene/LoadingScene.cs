using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Threading.Tasks;

namespace Core
{
    public class LoadingScene : Scene
    {
        private Scene _nextScene;
        private bool _ready;
        private string _message = "Loading...";

        public LoadingScene(Func<Scene> sceneFactory)
        {
            Task.Run(() =>
            {
                _nextScene = sceneFactory();
                _nextScene.LoadContent();
                _ready = true;
            });
        }

        public override void Update(GameTime gameTime)
        {
            //if (_ready)
            //    GameManager.Instance.SetScene = _nextScene;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(GameManager.Instance.CoreFont, _message, new Vector2(100, 100), Color.White);
        }

        public override void LoadContent()
        {
            throw new NotImplementedException();
        }

        public override void Unload()
        {
            throw new NotImplementedException();
        }
    }
}

