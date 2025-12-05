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

namespace Entity
{
    public enum Boss1State
    {
        Idel,
        Chase,
        Stage2,
        Dying,
        Dead
    }
    public class Boss1 : Enemys
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
        public Boss1State State { get; set; }
        public bool IsGoingLeft { get; private set; }
        public bool IsAttack { get; private set; } = false;

        private float _attackCooldown = 3f;  // таймер кулдауна
        private const float _attackDelay = 3f;   // задержка между атаками (регулируешь)

        private bool _showColiider = false;
        private bool _rotate = false;
        public int Damage { get; set; } = 5;
        public int Health { get; set; } = 50;

        private const float AggroDistance = 7 * 50; // 5 тайлов по 50px

        //protected virtual int DefaultHealth => 10; //Поле для наследников

        private float _forseSpeed = 2f;

        private float _dx, _dy;
        private float _player_dx;

        private Exploded _effect;

        private Rectangle _dyeRect;

        private Rectangle _tempRect = new();
        private Rectangle _tempRect2 = new();

        private Rectangle _visionRect = new();
        private const int AttackFrame = 7;

        private bool IsHitFrame => IsAttack && _animation.GetCurrFrame == AttackFrame;
        public static class Boss1Animation
        {
            public static readonly AnimationId Idel = new("Idel");
            public static readonly AnimationId Run = new("Run");
            public static readonly AnimationId Dying = new("Dying");
            public static readonly AnimationId Attack = new("Attack");
        }

        private AnimationController _animation;
        public Boss1(ContentManager Content, Rectangle rect, Rectangle srect, int TileSize)
            : base(Content.Load<Texture2D>("Enemy/Enemy1"), rect, srect)
        {
            OnGround = false;
            EventManager.Instance.Subscribe<ShowColliderEvent>(ShowCollider);

            State = Boss1State.Chase;
            LoadContent(Content);
        }
        private void ShowCollider(ShowColliderEvent e)
        {
            _showColiider = e.ShowCollider;
        }
        private void LoadContent(ContentManager content)
        {
            _animation = new AnimationController();

            // Idle
            var idle = content.Load<Texture2D>("Enemy/Boss1/Orc-Idle-Sheet");
            _animation.Add(Boss1Animation.Idel, new Animation.Animation(idle, 0, 6, 22, 16));

            // Run
            var run = content.Load<Texture2D>("Enemy/Boss1/Orc-Walk-Sheet");
            _animation.Add(Boss1Animation.Run, new Animation.Animation(run, 0, 8, 22, 16));

            // Dying
            var dying = content.Load<Texture2D>("Enemy/Boss1/Orc-Death-Sheet");
            _animation.Add(Boss1Animation.Dying, new Animation.Animation(dying, 0, 10, 30, 18, false, 0.2f));

            var attack = content.Load<Texture2D>("Enemy/Boss1/Orc-Attack-Sheet");
            _animation.Add(Boss1Animation.Attack, new Animation.Animation(attack, 0, 9, 40, 33, false, 0.05f));

            _animation.Play(Boss1Animation.Run);

            _effect = new Exploded(new Rectangle(0, 0, 600, 300), content,
                "Enemy/Boss1/Effects/FX_Dying", 11, new Point(64, 64), Color.White);

        }

        public override void Update(float dt, Rectangle PlayerPosition)
        {
            _effect.Update(dt, _dyeRect, Vector2.Zero);

            if (!Active)
                return;

            _dx = Math.Abs(PlayerPosition.Center.X - Rect.Center.X);
            _dy = Math.Abs(PlayerPosition.Center.Y - Rect.Center.Y);



            switch (State)
            {

                case Boss1State.Idel:
                    UpdateIdle(dt, PlayerPosition);
                    break;

                case Boss1State.Chase:
                    UpdateChase(dt, PlayerPosition);
                    break;

                case Boss1State.Dying:
                    UpdateDying(dt);
                    break;

                case Boss1State.Stage2:
                    UpdateStage2(dt, PlayerPosition);
                    break;
            }

            Velocity.Y = DoJump(Velocity.Y, dt);

            // Гравитация
            Velocity.Y += 0.5f; // ускорение вниз
            if (Velocity.Y > 5) Velocity.Y = 5; // лимит скорости падения

            if (State == Boss1State.Dying)
                Velocity.X = 0;

            if (!IsAttack)
                CollisionManeger.Instance.UpdateCollision(this);

            _animation.Update(dt);


        }
        private void UpdateStage2(float dt, Rectangle PlayerPosition)
        {
            if (_attackCooldown > 0)
                _attackCooldown -= dt;

            if (_isJumping && OnGround)
            {
                _isJumping = false;
            }

            if (Velocity.X != 0)
                _rotate = Velocity.X < 0;//Направление текстуры

            if (_dx > AggroDistance * 2 || _dy > AggroDistance)
            {
                Velocity.X = 0;
                _animation.Play(Boss1Animation.Run);
                IsAttack = false;
            }
            if (OnGround)
            {
                if (PlayerPosition.X < Rect.X)
                    Velocity.X = -1f;
                if (PlayerPosition.X >= Rect.X)
                    Velocity.X = 1f;

                _player_dx = PlayerPosition.X - Rect.X;

                if (Math.Abs(_player_dx) > 8f)
                    Velocity.X = Math.Sign(_player_dx) * _forseSpeed;
                else
                    Velocity.X = 0f;
            }
            FindPath();

            if (!IsAttack && _attackCooldown <= 0)
                if (_dx < Rect.Width * 2 || _dy < Rect.Height * 2)
                {
                    IsAttack = true;

                    _visionRect = Rect;

                    _visionRect.Y -= Rect.Height;
                    _visionRect.Width = Rect.Width * 2;
                    _visionRect.Height = Rect.Height * 2;

                    Velocity.X = 0;
                    _animation.Play(Boss1Animation.Attack);
                    _attackCooldown = _attackDelay;
                }

            if (IsAttack)
            {
                if (IsHitFrame)
                {
                    if (_visionRect.Intersects(PlayerPosition))
                    {
                        EventManager.Instance.Trigger(new TakeDamagePlayerEvent(5,
                            new Vector2(0, -20)));
                    }
                    EventManager.Instance.Trigger(new LandingEffectEvent());
                }
                if (_animation.IsAnimationPlayed)
                {

                    IsAttack = false;
                    _animation.Play(Boss1Animation.Run);
                }
                Velocity.X = 0;
            }
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
                State = Boss1State.Idel;
                Velocity.X = 0;
                _animation.Play(Boss1Animation.Idel);
                return;
            }
            if (OnGround)
            {
                if (PlayerPosition.X < Rect.X)
                    Velocity.X = -1f;
                if (PlayerPosition.X >= Rect.X)
                    Velocity.X = 1f;

                _player_dx = PlayerPosition.X - Rect.X;

                if (Math.Abs(_player_dx) > 8f)
                    Velocity.X = Math.Sign(_player_dx) * _forseSpeed;
                else
                    Velocity.X = 0f;
            }
            FindPath();
        }

        private void FindPath()
        {
            if (OnGround)
            {
                // если упёрся в стену — разворачиваемся
                if (CollisionManeger.Instance.TileCollision.HasWallAhead(this.Rect, Velocity))
                {

                    int tileSize = LevelManager.Instance.Level.TileMap.TILESIZE;
                    _tempRect = this.Rect;
                    _tempRect.Height = tileSize;
                    _tempRect.Y += tileSize * 3;

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
        public void TakeDamage(int damage, Vector2 knockback)
        {
            //if (_damageCooldown > 0) return; // ещё не прошло время защиты

            Health -= damage;

            if (Health <= 25)
            {
                State = Boss1State.Stage2;
                _forseSpeed = 4.0f;
            }

            if (Health <= 0)
            {
                Health = 0;

                State = Boss1State.Dying;

                ActiveCollider = false;

                _animation.Play(Boss1Animation.Dying);

                _dyeRect = Rect;

                Rect.Width += 32;
                Rect.Height += 8;
            }
        }
        private void UpdateDying(float dt)
        {
            if (State == Boss1State.Dying)
            {
                if (_animation.IsAnimationPlayed)
                {
                    _dyeRect.Y = Rect.Y;
                    _effect.StartEffect();
                    State = Boss1State.Dead;
                    Active = false;
                }
            }
        }

        private void UpdateIdle(float dt, Rectangle PlayerPosition)
        {
            Velocity.X = 0;

            if (_dx < AggroDistance && _dy < AggroDistance * 0.5f)
            {
                State = Boss1State.Chase;
                _animation.Play(Boss1Animation.Run);
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                if (IsAttack)
                    _animation.Draw(spriteBatch, _visionRect, Color.White, _rotate);
                else
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
