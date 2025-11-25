using Animation;
using Core;
using FX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Entity.Slime;


namespace Entity
{
    public enum KnightState
    {
        Idle,
        Chase,
        Dying,
        Dead,
        Patrol
    }
    public class Knight : Enemys
    {
        //// Константы вертикального двиижениия
        private const float MaxJumpTime = 0.4f;
        private const float JumpLaunchVelocity = -30.0f;
        private const float GravityAcceleration = 50.0f;
        private const float MaxFallSpeed = 50.0f;
        private const float JumpControlPower = 0.3f;
        // Константы вертикального двиижениия

        /// <summary>
        /// переменные прыжка
        /// </summary>
        private bool _isJumping;
        private bool _wasJumping;
        private float _jumpTime;

        private float _randTimerIdel;

        private int PatrolLeftX;
        private int PatrolRightX;

        private float _respTimer;
        public KnightState State { get; set; }
        public bool IsGoingLeft { get; private set; }

        private bool _showColiider = false;
        private bool _rotate = false;
        public int Damage { get; set; } = 5;
        public int Health { get; set; } = 30;

        private const float AggroDistance = 5 * 50; // 5 тайлов по 50px

        private float _dx, _dy;

        private Exploded _effect;
        private Rectangle _dyeRect;

        private Rectangle _tempRect = new();
        private Rectangle _tempRect2 = new();
        public static class KnightAnimation
        {
            //public static readonly AnimationId Idle = new("Idle");
            public static readonly AnimationId Run = new("Run");
            //public static readonly AnimationId Dying = new("Dying");
        }
        private AnimationController _animation;
        public Knight(ContentManager Content, Rectangle rect, Rectangle srect, int TileSize)
            : base(Content.Load<Texture2D>("Enemy/Enemy1"), rect, srect)
        {
            OnGround = false;
            EventManager.Instance.Subscribe<ShowColliderEvent>(ShowCollider);

            State = KnightState.Patrol;
            LoadContent(Content);

            PatrolRightX = (320 / 16) * TileSize;
            PatrolLeftX = (60 / 16) * TileSize;
            if (rect.X > PatrolLeftX)
                IsGoingLeft = true;
            else
                IsGoingLeft = false;

        }
        private void ShowCollider(ShowColliderEvent e)
        {
            _showColiider = e.ShowCollider;
        }
        private void LoadContent(ContentManager content)
        {
            _animation = new AnimationController();

            // Idle
            var idle = content.Load<Texture2D>("Enemy/Knight/Idel");
            _animation.Add(SlimeAnimation.Idle, new Animation.Animation(idle, 0, 4, 15, 20));

            // Run
            var run = content.Load<Texture2D>("Enemy/Knight/Run");
            _animation.Add(KnightAnimation.Run, new Animation.Animation(run, 0, 16, 15, 20));

            // Dying
            var dying = content.Load<Texture2D>("Enemy/Knight/Dying");
            _animation.Add(SlimeAnimation.Dying, new Animation.Animation(dying, false, 0.2f));

            _animation.Play(KnightAnimation.Run);




            _effect = new Exploded(new Rectangle(0, 0, 150, 150), content,
                "Enemy/Knight/Effects/FX_Dying", 11, new Point(64, 64),Color.White);
        }

        public override void Update(float dt, Rectangle PlayerPosition)
        {
            _effect.Update(dt, _dyeRect, Vector2.Zero);

            if (_respTimer > 0.0f)
            {
                _respTimer -= 0.9f * dt;
            }
            else if (_respTimer <= 0)
            {
                Active = true;
            }

            if (!Active)
                return;

            _dx = Math.Abs(PlayerPosition.Center.X - Rect.Center.X);
            _dy = Math.Abs(PlayerPosition.Center.Y - Rect.Center.Y);



            switch (State)
            {
                case KnightState.Patrol:
                    UpdatePatrol(dt, PlayerPosition);
                    break;

                case KnightState.Idle:
                    UpdateIdle(dt, PlayerPosition);
                    break;

                case KnightState.Chase:
                    UpdateChase(dt, PlayerPosition);
                    break;

                case KnightState.Dying:
                    UpdateDying(dt);
                    break;
            }

            Velocity.Y = DoJump(Velocity.Y, dt);

            // Гравитация
            Velocity.Y += 0.5f; // ускорение вниз
            if (Velocity.Y > 5) Velocity.Y = 5; // лимит скорости падения

            if (State == KnightState.Dying)
                Velocity.X = 0;

            CollisionManeger.Instance.UpdateCollision(this);

            _animation.Update(dt);


        }

        private void UpdatePatrol(float dt, Rectangle PlayerPosition)
        {
            if (_isJumping && OnGround)
            {
                _isJumping = false;
            }

            //увидел игрока
            if (_dx < AggroDistance && _dy < AggroDistance * 0.5f)
            {
                State = KnightState.Chase;
                _animation.Play(SlimeAnimation.Run);
            }

            if (IsGoingLeft)
            {
                // Достиг левого края патруля?
                if (Rect.Center.X <= PatrolLeftX)
                {
                    State = KnightState.Idle;
                    _animation.Play(SlimeAnimation.Idle);
                    _randTimerIdel = Random.Shared.Next(3, 15);
                    IsGoingLeft = false; // Пошёл направо
                }
                else
                {
                    if (_isJumping)
                        Velocity.X = -2f;// форсируем
                    else
                        Velocity.X = -1f;  // Идём влево
                }
            }
            else // идём направо
            {
                if (Rect.Center.X >= PatrolRightX)
                {
                    State = KnightState.Idle;
                    _animation.Play(SlimeAnimation.Idle);
                    _randTimerIdel = Random.Shared.Next(3, 15);
                    IsGoingLeft = true;
                }
                else
                {
                    if (_isJumping)
                        Velocity.X = 2f;// форсируем
                    else
                        Velocity.X = 1f;
                }
            }


            FindPath();

            if (Velocity.X != 0)
                _rotate = Velocity.X < 0;//Направление текстуры
        }
        private void FindPath()
        {
            if (OnGround)
            {
                // если упёрся в стену — разворачиваемся
                if (CollisionManeger.Instance.TileCollision.HasWallAhead(this.Rect, Velocity))
                {
                    _tempRect = this.Rect;
                    _tempRect.Y += LevelManager.Instance.Level.TileMap.TILESIZE * 2;
                    if (!CollisionManeger.Instance.TileCollision.HasWallAhead(
                       _tempRect, Velocity))
                    {
                        _isJumping = true;
                        return;
                    }

                    Velocity.X *= -1;
                    IsGoingLeft = !IsGoingLeft;
                }


                // если впереди обрыв — тоже разворот
                if (!CollisionManeger.Instance.TileCollision.HasGroundBelow(this.Rect, Velocity))
                {
                    _tempRect = this.Rect;
                    if (IsGoingLeft)
                    {
                        _tempRect = this.Rect;
                        _tempRect.X -= LevelManager.Instance.Level.TileMap.TILESIZE * 3;
                        if (CollisionManeger.Instance.TileCollision.HasGroundBelow(
                           _tempRect, Velocity))
                        {
                            _isJumping = true;
                            return;
                        }
                        // случай если не высоко и можно сойти с платформы высота до 2х тайлов
                        _tempRect = this.Rect;
                        _tempRect.X -= LevelManager.Instance.Level.TileMap.TILESIZE;
                        _tempRect.Y += LevelManager.Instance.Level.TileMap.TILESIZE;
                        _tempRect2 = _tempRect;
                        _tempRect2.Y += LevelManager.Instance.Level.TileMap.TILESIZE;
                        if (CollisionManeger.Instance.TileCollision.HasGroundBelow(
                           _tempRect, Velocity) ||
                           CollisionManeger.Instance.TileCollision.HasGroundBelow(
                           _tempRect2, Velocity))
                            return;//Продолжать движение
                    }
                    else
                    {
                        _tempRect = this.Rect;
                        _tempRect.X += LevelManager.Instance.Level.TileMap.TILESIZE * 2;
                        if (CollisionManeger.Instance.TileCollision.HasGroundBelow(
                           _tempRect, Velocity))
                        {
                            _isJumping = true;
                            return;
                        }
                        // случай если не высоко и можно сойти с платформы высота до 2х тайлов
                        _tempRect = this.Rect;
                        _tempRect.X -= LevelManager.Instance.Level.TileMap.TILESIZE;
                        _tempRect.Y += LevelManager.Instance.Level.TileMap.TILESIZE;
                        _tempRect2 = _tempRect;
                        _tempRect2.Y += LevelManager.Instance.Level.TileMap.TILESIZE;
                        if (CollisionManeger.Instance.TileCollision.HasGroundBelow(
                           _tempRect, Velocity) ||
                           CollisionManeger.Instance.TileCollision.HasGroundBelow(
                           _tempRect2, Velocity))
                            return;//Продолжать движение
                    }
                    Velocity.X *= -1;
                    IsGoingLeft = !IsGoingLeft;
                }
            }
        }
        private float DoJump(float velocityY, float dt)
        {
            // If the player wants to jump
            if (_isJumping)
            {
                // Begin or continue a jump
                if ((!_wasJumping && OnGround) || _jumpTime > 0.0f)
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
        private void UpdateDying(float dt)
        {
            if (State == KnightState.Dying)
            {
                if (_animation.IsAnimationPlayed)
                {
                    _dyeRect.Y = Rect.Y;
                    _effect.StartEffect();
                    State = KnightState.Dead;
                    Respawn();
                }
            }
        }
        private void Respawn()
        {
            OnGround = false;
            Rect = _dyeRect;

            Rect.X = Random.Shared.Next(0, (int)LevelManager.Instance.Level.TileMap.ReturnSizeMapX());
            Rect.Y = 0;

            Velocity.X = 0;

            _respTimer = Random.Shared.Next(5, 30);
            Active = false;
            ActiveCollider = true;

            _animation.Play(SlimeAnimation.Run);
            State = KnightState.Idle;
        }
        private void UpdateChase(float dt, Rectangle PlayerPosition)
        {
            if (_isJumping && OnGround)
            {
                _isJumping = false;
            }

            if (Velocity.X != 0)
                _rotate = Velocity.X < 0;//Направление текстуры

            if (_dx > AggroDistance * 2 || _dy > AggroDistance)
            {
                State = KnightState.Patrol;
                Velocity.X = 0;
                //_animation.Play(SlimeAnimation.Idle);
                return;
            }
            if (OnGround)
            {
                if (PlayerPosition.X < Rect.X)
                    Velocity.X = -2f;
                if (PlayerPosition.X >= Rect.X)
                    Velocity.X = 2f;
            }
            FindPath();
        }
        private void UpdateIdle(float dt, Rectangle PlayerPosition)
        {
            _randTimerIdel -= dt;
            if (_randTimerIdel <= 0)
            {
                State = KnightState.Patrol;
                _animation.Play(SlimeAnimation.Run);
            }
            Velocity.X = 0;

            if (_dx < AggroDistance && _dy < AggroDistance * 0.5f)
            {
                State = KnightState.Chase;
                _animation.Play(SlimeAnimation.Run);
            }
        }
        public void TakeDamage(int damage, Vector2 knockback)
        {
            //if (_damageCooldown > 0) return; // ещё не прошло время защиты

            Health -= damage;
            if (Health < 0) Health = 0;

            State = KnightState.Dying;

            ActiveCollider = false;

            _animation.Play(SlimeAnimation.Dying);

            _dyeRect = Rect;

            Rect.Width += Rect.Width/3;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                _animation.Draw(spriteBatch, Rect, Color.White, _rotate);

                if (_showColiider)
                    spriteBatch.Draw(CollisionManeger.Instance.ColliderTexture, Rect, Color.White);
            }
            _effect.Draw(spriteBatch);
        }
        public override void Unload()
        {
            EventManager.Instance.Unsubscribe<ShowColliderEvent>(ShowCollider);
        }
    }
}
