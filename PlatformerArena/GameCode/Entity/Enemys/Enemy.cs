using Animation;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using FX;

namespace Entity
{
    public enum EnemyState
    {
        Live,
        Dying,
        Dead
    }
    public class Enemy : Enemys
    {

        public int Damage { get; set; } = 5;
        public int Health { get; set; } = 30;

        private bool _showColiider = false;


        private float _jumpTimer;
        private int _jumpCount;
        private bool _isJumping;

        private float _damageCooldown; // таймер неуязвимости

        private int _bounceCount;
        private float _bounceTime;
        private float _originalY;

        private Vector2 _knockbackForce;
        private float _knockbackTimer;
        private bool _isHit;

        private bool _rotate = false;

        private Rectangle _saveSizeRect;

        private Exploded _effect;

        public EnemyState State { get; set; }
        public static class EnemyAnimation
        {
            public static readonly AnimationId Run = new("Run");
            public static readonly AnimationId Dying = new("Dying");
        }
        private AnimationController _animationController;

        public Enemy(ContentManager Content, Rectangle rect, Rectangle srect) :
            base(Content.Load<Texture2D>("Enemy/Enemy1"), rect, srect)
        {
            OnGround = false;
            EventManager.Instance.Subscribe<ShowColliderEvent>(ShowCollider);
            Texture2D texture = Content.Load<Texture2D>("Enemy/Enemy1RUN");
            _animationController = new();
            _animationController.Add(EnemyAnimation.Run, new Animation.Animation(texture, true));
            texture = Content.Load<Texture2D>("Enemy/Enemy1Dying");
            _animationController.Add(EnemyAnimation.Dying, new Animation.Animation(texture, 0, 11, 55, 33, false));
            _animationController.Play(EnemyAnimation.Run);
            State = EnemyState.Live;
            _effect = new Exploded(Rect, Content);
        }

        private void ShowCollider(ShowColliderEvent e)
        {
            _showColiider = e.ShowCollider;
        }
        public override void Update(float dt, Rectangle PlayerPosition)
        {
            _effect.Update(dt, _saveSizeRect, Vector2.Zero);
            
            if (_knockbackTimer > 0)
            {
                _knockbackTimer -= 0.9f * dt;
            }
            else if (_knockbackTimer <= 0)
            {
                Active = true;
                State = EnemyState.Live;
            }

            if (!Active)
                return;

            if (_damageCooldown > 0)
                _damageCooldown -= dt;

            _animationController.Update(dt);

            // Гравитация
            Velocity.Y += 0.5f; // ускорение вниз
            if (Velocity.Y > 5) Velocity.Y = 5; // лимит скорости падения

            if (Velocity.X == 0 && OnGround && State == EnemyState.Live)
            {
                Velocity.X = Random.Shared.Next(2) == 0 ? -1.0f : 1.0f;
            }

            if (State == EnemyState.Dying)
                Velocity.X = 0;

            if (Velocity.X != 0)
                _rotate = Velocity.X > 0;//Направление текстуры

            if (ActiveCollider)
                CollisionManeger.Instance.UpdateCollision(this);

            if (OnGround)
            {
                // если упёрся в стену — разворачиваемся
                if (CollisionManeger.Instance.TileCollision.HasWallAhead(this.Rect, Velocity))
                    Velocity.X *= -1;

                // если впереди обрыв — тоже разворот
                if (!CollisionManeger.Instance.TileCollision.HasGroundBelow(this.Rect, Velocity))
                    Velocity.X *= -1;
            }

            if (State == EnemyState.Dying)
            {
                if (_animationController.IsAnimationPlayed)
                {
                    _effect.StartEffect();
                    State = EnemyState.Dead;
                    Respawn();
                }
            }
        }
        public void TakeDamage(int damage, Vector2 knockback)
        {
            //if (_damageCooldown > 0) return; // ещё не прошло время защиты

            Health -= damage;
            if (Health < 0) Health = 0;

            // Оставляем только горизонтальное смещение
            _knockbackForce = new Vector2(knockback.X, 0);
            _knockbackTimer = 2f;//0.5f;
            _isHit = true;

            // "дрожь" или легкие прыжки на месте
            _bounceTime = 0.1f;
            _bounceCount = 3;

            _damageCooldown = 0.3f; // 300 мс неуязвимости — можно подстроить

            State = EnemyState.Dying;
            _saveSizeRect = Rect;

            ActiveCollider = false;

            _animationController.Play(EnemyAnimation.Dying);
            Rect.X -= 22;
            Rect.Width = 55;
        }
        public void Respawn()
        {
            OnGround = false;
            Rect.X = Random.Shared.Next(0, (int)LevelManager.Instance.Level.TileMap.ReturnSizeMapX());
            Rect.Y = 0;
            Health = 30;
            Velocity.X = 0;
            _knockbackForce.X = 0;
            _knockbackTimer = Random.Shared.Next(10, 60);
            Active = false;
            ActiveCollider = true;
            _animationController.Play(EnemyAnimation.Run);
            Rect.Width = _saveSizeRect.Width;
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (Active)
            {
                _animationController.Draw(spriteBatch, Rect, Color.White, _rotate);

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



