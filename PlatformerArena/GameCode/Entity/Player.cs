using Core;
using Input;
using Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using Animation;
using FX;

namespace Entity
{
    public class Player : IEntity
    {
        private PlayerData _playerData;
        private Vector2 _position;
        private Texture2D _temp;
        public Rectangle _dest;
        private Rectangle _visualization;
        private Texture2D _emotion1;

        public Collider Collider;
        /// <summary>
        /// Отображение коллайдера
        /// </summary>
        private bool IsColliderDraw = false;

        // Константы горизонтального движения 
        private const float MoveAcceleration = 400.0f;
        private const float MaxMoveSpeed = 500.0f;
        private const float GroundDragFactor = 0.48f;
        private const float AirDragFactor = 0.58f;

        // Константы вертикального двиижениия
        private const float MaxJumpTime = 0.35f;
        private const float JumpLaunchVelocity = -100.0f;
        private const float GravityAcceleration = 150.0f;
        private const float MaxFallSpeed = 50.0f;
        private const float JumpControlPower = 0.14f;

        /// <summary>
        /// Новая система анимации
        /// </summary>
        public static class PlayerAnim
        {
            public static readonly AnimationId Idle = new("Idle");
            public static readonly AnimationId Blink = new("Blink");
        }

        private AnimationController _animationController;

        private EffectManager _effect;

        public int Health { get; private set; } = 20;
        private float _damageCooldown; // таймер неуязвимости

        /// <summary>
        /// Переменная ввода.
        /// </summary>
        private float _movement;

        /// <summary>
        /// Вектор движения
        /// </summary>
        public Vector2 Velocity;


        /// <summary>
        /// переменные прыжка
        /// </summary>
        private bool _isJumping;
        private bool _wasJumping;
        private float _jumpTime;

        public bool IsOnGround;

        public bool IsJumping = false;
        private bool _isFall = false;

        public Player(PlayerData playerData, Vector2 spavnPosition, ContentManager Contetnt, Texture2D texture2D = null)
        {
            _playerData = playerData;
            _position = spavnPosition;
            if (texture2D == null)
            {
                _temp = GameManager.Instance.Blank;
            }
            else
            {
                _temp = texture2D;
            }

            _dest = new Rectangle((int)spavnPosition.X, (int)spavnPosition.Y, 50, 50);
            _visualization = _dest;

            Collider = new Collider(new Vector2(5, 0), new Vector2(40, 50), _dest);

            EventManager.Instance.Subscribe<CoinColectEvent>(CoinColect);
            EventManager.Instance.Subscribe<ScoreColectEvent>(ScoreColect);

            LoadContent(Contetnt);
        }
        public void ScoreColect(ScoreColectEvent e)
        {
            _playerData.Score += e.Points;
            EventManager.Instance.Trigger(new UpdateScoreEvent(_playerData.Score));
        }
        private void CoinColect(CoinColectEvent e)
        {
            _playerData.Score += e.Points;
            EventManager.Instance.Trigger(new UpdateScoreEvent(_playerData.Score));
        }
        public void LoadContent() { }

        public void LoadContent(ContentManager Content)
        {
            _emotion1 = Content.Load<Texture2D>("Player/emotion1");

            _animationController = new();

            _animationController.Add(PlayerAnim.Blink, new Animation.Animation(_emotion1, true, 0.15f));

            _animationController.Play(PlayerAnim.Blink);
            _effect = new EffectManager();
            _effect.Add(new DustEffect(_dest, Content));
            _effect.Add(new LandingEffect(_dest, Content));

        }
        public void TakeDamage(int amount, Vector2 knockbackDirection)
        {
            if (_damageCooldown > 0) return; // ещё не прошло время защиты

            Health -= amount;

            if (knockbackDirection.X > 10) { knockbackDirection.X = 30f; knockbackDirection.Y = -20f; }
            else if (knockbackDirection.X < -10) { knockbackDirection.X = -30f; knockbackDirection.Y = -20f; }
            // отброс ударом игрока
            //Velocity.X = 0;

            Velocity = new Vector2(knockbackDirection.X, knockbackDirection.Y);

            // проверка здоровья
            if (Health <= 0)
            {
                Unload();
                LevelManager.Instance.Dispose();
                LevelManager.Instance.PlayerDataGlobal.Score = _playerData.Score;
                EventManager.Instance.Trigger(new ChangeSceneEvent("GameOver"));
            }
            _damageCooldown = 0.4f;
        }

        // Поле класса Player
        private float _idleTime;
        private float _idleOffsetY;
        private const float _idleAmplitude = 10.0f;   // амплитуда покачивания
        private const float _idleSpeed = 5.0f;       // скорость дыхания
        public void Update(GameTime gameTime)
        {
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;
            _position.X = _dest.X;
            _position.Y = _dest.Y;

            _animationController.Update(dt);

            // если гиря стоит на земле и не движется — "прыгает на месте"
            if (IsOnGround && Math.Abs(Velocity.X) < 0.1f)
            {
                _idleTime -= 10 * dt;
                if (_idleTime <= 0)
                {

                    _idleTime = _idleSpeed;
                    _idleOffsetY = _idleAmplitude;
                }
                else if (_idleOffsetY > 0)
                {
                    _idleOffsetY -= GravityAcceleration * dt;
                }
            }
            else
            {
                // сброс при движении или прыжке
                _idleTime = _idleSpeed;
                _idleOffsetY = 0f;
                _dest.Height = _temp.Height;
            }


            if (_damageCooldown > 0)
                _damageCooldown -= dt;

            Input();

            ApplyPhysics(dt);

            
            if(IsJumping && IsOnGround || IsOnGround && _isFall)
            {
                _isFall = false;
                IsJumping = false;
                EventManager.Instance.Trigger(new LandingEffectEvent());
            }
            if (_isJumping)
                IsJumping = true;

            if (IsOnGround)
                _effect.Update(gameTime, _visualization, new Vector2(_movement, 0));
            else
                _effect.Update(gameTime,Rectangle.Empty,Vector2.Zero);

            _movement = 0.0f;
            _isJumping = false;

            //упал ниже карты, убиваем игрока
            if (_dest.Y > LevelManager.Instance.Level.TileMap.ReturnSizeMapY() + 300)
                TakeDamage(Health, Vector2.Zero);

            _dest = Collider.UpdateSpriteRect(_dest);
            _visualization = _dest;
            _visualization.Y = (int)(_visualization.Y - _idleOffsetY);
        }
        private void ApplyPhysics(float dt)
        {
            Vector2 previousPosition = _position;

            // движене и гравтация
            Velocity.X += _movement * MoveAcceleration * dt;
            Velocity.Y = MathHelper.Clamp(Velocity.Y + GravityAcceleration * dt, -MaxFallSpeed, MaxFallSpeed);
            
            if (Velocity.Y > 30)
                _isFall = true;

            Velocity.Y = DoJump(Velocity.Y, dt);
            // трение 
            if (IsOnGround)
                Velocity.X *= GroundDragFactor;
            else
                Velocity.X *= AirDragFactor;

            Velocity.X = MathHelper.Clamp(Velocity.X, -MaxMoveSpeed, MaxMoveSpeed);

            _position = new Vector2((float)Math.Round(_position.X), (float)Math.Round(_position.Y));

        }
        private float DoJump(float velocityY, float dt)
        {
            // If the player wants to jump
            if (_isJumping)
            {
                // Begin or continue a jump
                if ((!_wasJumping && IsOnGround) || _jumpTime > 0.0f)
                {

                    _jumpTime += dt;

                }

                if (0.0f < _jumpTime && _jumpTime <= MaxJumpTime)
                {
                    velocityY = JumpLaunchVelocity * (1.0f - (float)Math.Pow(_jumpTime / MaxJumpTime, JumpControlPower));
                }
                else
                {
                    // Reached the apex of the jump
                    _jumpTime = 0.0f;
                }
            }
            else
            {
                // Continues not jumping or cancels a jump in progress
                _jumpTime = 0.0f;
            }
            _wasJumping = _isJumping;

            return velocityY;
        }

        private void Input()
        {
            var input = InputManager.Instance;

            if (Math.Abs(_movement) < 0.5f)
                _movement = 0.0f;

            if (input.IsKeyHeldDown(Keys.Left) || input.IsKeyHeldDown(Keys.A))
                _movement = -1.0f;

            if (input.IsKeyHeldDown(Keys.Right) || input.IsKeyHeldDown(Keys.D))
                _movement = 1.0f;

            if (input.IsKeyPressed(Keys.C))
            {
                IsColliderDraw = !IsColliderDraw;
                EventManager.Instance.Trigger(new ShowColliderEvent(IsColliderDraw));
            }

            _isJumping = (input.IsKeyPressed(Keys.Space) || input.IsKeyPressed(Keys.Up));
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            _effect.Draw(spriteBatch);

            spriteBatch.Draw(_temp, _visualization, Color.White);

            _animationController.Draw(spriteBatch, _visualization, Color.White);


            if (IsColliderDraw)
                spriteBatch.Draw(CollisionManeger.Instance.ColliderTexture, Collider.ColliderRectangle, Color.White);
        }


        public void Unload()
        {
            EventManager.Instance.Unsubscribe<CoinColectEvent>(CoinColect);
            EventManager.Instance.Unsubscribe<ScoreColectEvent>(ScoreColect);
        }
    }
}

