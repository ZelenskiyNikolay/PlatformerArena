
using Core;
using Entity;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;



namespace Levels
{
    public class Level : IDisposable
    {
        private ContentManager _content;
        private Texture2D _textureFon;
        private TileMap _tileMap;

        public TileMap TileMap { get { return _tileMap; } }
        public int TileSize { get { return _tileMap.TILESIZE; } }

        private Camera _camera;

        private Player _player;
        private PlayerData _playerData;
        private List<Coin> _coins;
        private List<Enemys> _enemys;

        private Rectangle _colliderExit;

        private bool _startBaner;
        private NumberLevelScreen _baner;
        public ContentManager Content
        {
            get { return _content; }
        }
        public Level(IServiceProvider ServiceProvider, string ContentPas, int levelIndex, PlayerData playerData)
        {
            _content = new ContentManager(ServiceProvider, ContentPas);

            _playerData = playerData;
            _textureFon = Content.Load<Texture2D>("Map" + levelIndex + "/Image/Fon" + levelIndex);

            System.Diagnostics.Debug.WriteLine(_textureFon == null ? "фон не найден" : "фон найден");

            Texture2D temp = Content.Load<Texture2D>("world_tileset");

            System.Diagnostics.Debug.WriteLine(temp == null ? "тайлы не найден" : "тайлы найден");
            _tileMap = new();
            _tileMap.TextureAtlas = temp;

            CollisionManeger.Instance.ColliderTexture = Content.Load<Texture2D>("collider");

            System.Diagnostics.Debug.WriteLine(ContentPas + "/Map" + levelIndex + "/mg.csv");

            _tileMap.AddTileLayer(ContentPas + "/Map" + levelIndex + "/mg.csv");
            _tileMap.AddTileLayer(ContentPas + "/Map" + levelIndex + "/spaun.csv");
            _tileMap.AddCollisionLayer(ContentPas + "/Map" + levelIndex + "/collisions.csv");

            Dictionary<Point, int> tempLayer = _tileMap.SpaunLayer;
            if (tempLayer != null)
            {
                List<Rectangle> anim = new();

                for (int i = 0; i < 4; i++)
                {
                    Rectangle rect = new Rectangle(16 * i, 0, 16, 16);
                    anim.Add(rect);
                }
                _coins = new List<Coin>();
                temp = Content.Load<Texture2D>("Coin/Coin");
                _enemys = new List<Enemys>();


                foreach (var tile in tempLayer)
                {
                    if (tile.Value == 240)
                    {
                        Texture2D player = Content.Load<Texture2D>("Player/Player");
                        _player = new Player(playerData, new Vector2(tile.Key.X * _tileMap.TILESIZE,
                            tile.Key.Y * _tileMap.TILESIZE), Content, player);
                    }
                    else if (tile.Value == 241)
                    {
                        _coins.Add(new Coin(temp, anim, new Point(tile.Key.X * _tileMap.TILESIZE, tile.Key.Y * _tileMap.TILESIZE)));
                    }
                    else if (tile.Value == 165)
                    {
                        _colliderExit = new Rectangle(tile.Key.X * _tileMap.TILESIZE, tile.Key.Y * _tileMap.TILESIZE,
                            _tileMap.TILESIZE, _tileMap.TILESIZE);
                    }
                    else if (tile.Value == 242)
                    {
                        _enemys.Add(new Enemy(Content, new Rectangle(tile.Key.X * _tileMap.TILESIZE, tile.Key.Y * _tileMap.TILESIZE,
                            30, 30), new Rectangle(0, 0, 30, 30)));
                    }
                    else if (tile.Value == 243)
                    {
                        _enemys.Add(new Slime(Content, new Rectangle(tile.Key.X * _tileMap.TILESIZE, tile.Key.Y * _tileMap.TILESIZE,
                            30, 30), new Rectangle(0, 0, 30, 30)));
                    }
                    else if (tile.Value == 244)
                    {
                        _enemys.Add(new Knight(Content, new Rectangle(tile.Key.X * _tileMap.TILESIZE, tile.Key.Y * _tileMap.TILESIZE,
                            30, 30), new Rectangle(0, 0, 30, 30), _tileMap.TILESIZE));
                    }
                    else if (tile.Value == 245)
                    {
                        _enemys.Add(new HeavyKnight(Content, new Rectangle(tile.Key.X * _tileMap.TILESIZE, tile.Key.Y * _tileMap.TILESIZE,
                            60, 60), new Rectangle(0, 0, 30, 30), _tileMap.TILESIZE));
                    }
                    else if (tile.Value == 250)
                    {
                        _enemys.Add(new Boss1(Content, new Rectangle(tile.Key.X * _tileMap.TILESIZE, tile.Key.Y * _tileMap.TILESIZE,
                            100, 100), new Rectangle(0, 0, 30, 30), _tileMap.TILESIZE));
                    }
                }
            }
            CollisionManeger.Instance.InitColision(_tileMap.Collisions);

            LevelUI.Instance.SetScore(playerData.Score);
            temp = Content.Load<Texture2D>("UI/Heart");
            LevelUI.Instance._heart = temp;

            LevelManager.Instance.Camera = new Camera(GameManager.Instance.Graphics.PreferredBackBufferWidth,
                GameManager.Instance.Graphics.PreferredBackBufferHeight,
                _tileMap.ReturnSizeMapX(), _tileMap.ReturnSizeMapY());

            _baner = new NumberLevelScreen(levelIndex.ToString());
            _startBaner = true;
            LevelManager.Instance.Camera.Update(_player._dest, 0);
        }
        public void LevelComplete()
        {
            _player.Unload();
            if (LevelManager.Instance.Vin)
            {
                EventManager.Instance.Trigger(new ChangeSceneEvent("Win"));
            }
            else
            {
                EventManager.Instance.Trigger(new LevelСompletedEvent(_playerData.Score));
            }
        }
        public void Updete(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            LevelUI.Instance.Update(dt);
            if (_startBaner)
            {
                _startBaner = _baner.Update(dt);
                return;
            }

            _player.Update(gameTime);

            Vector2 deptIntersects = _player._dest.GetIntersectionDepth(_colliderExit);
            if ((deptIntersects.X > _player._dest.Width / 3 || deptIntersects.Y > _player._dest.Height / 3)
                && _player.IsOnGround)
            {
                LevelComplete();
            }

            foreach (Enemys enemy in _enemys)
            {

                enemy.Update(dt, _player._dest);
                if (enemy.Active && enemy.ActiveCollider)
                    CollisionManeger.Instance.UpdateCollision(_player, enemy);

            }
            foreach (var coin in _coins)
            {
                coin.Update(gameTime);
                CollisionManeger.Instance.UpdateCollision(_player._dest, coin);
            }
            CollisionManeger.Instance.UpdateCollision(_player);


            LevelManager.Instance.Camera.Update(_player._dest, dt);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (_startBaner)
            {
                _baner.Draw(spriteBatch);
                return;
            }

            spriteBatch.Draw(_textureFon, Vector2.Zero, Color.White);
            _tileMap.Draw(spriteBatch);
            _player.Draw(spriteBatch);

            foreach (var coin in _coins)
            {
                coin.Draw(spriteBatch);
            }
            foreach (Enemys enemy in _enemys)
            {
                enemy.Draw(spriteBatch);
            }
        }
        public void Dispose()
        {
            foreach (var enemy in _enemys)
            {
                enemy.Unload();
            }
            Content.Unload();
        }
    }
}


