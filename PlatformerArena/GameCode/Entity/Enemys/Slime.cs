
using Animation;
using Core;
using FX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Entity
{
    public enum SlimeState
    {
        Idle,
        Chase,
        Dying,
        Dead
    }
    public class Slime : Enemys
    {
        private float _respTimer;
        public SlimeState State { get; set; }

        private bool _showColiider = false;
        private bool _rotate = false;
        public int Damage { get; set; } = 5;
        public int Health { get; set; } = 30;

        private const float AggroDistance = 5 * 50; // 5 тайлов по 50px

        private float _dx, _dy;

        private Exploded _effect;
        private Rectangle _dyeRect;
        public static class SlimeAnimation
        {
            public static readonly AnimationId Idle = new("Idle");
            public static readonly AnimationId Run = new("Run");
            public static readonly AnimationId Dying = new("Dying");
        }
        private AnimationController _animation;
        public Slime(ContentManager Content, Rectangle rect, Rectangle srect)
            : base(Content.Load<Texture2D>("Enemy/Enemy1"), rect, srect)
        {
            OnGround = false;
            EventManager.Instance.Subscribe<ShowColliderEvent>(ShowCollider);
            LoadContent(Content);
            State = SlimeState.Idle;
        }
        private void ShowCollider(ShowColliderEvent e)
        {
            _showColiider = e.ShowCollider;
        }
        private void LoadContent(ContentManager content)
        {
            _animation = new AnimationController();

            // Idle
            var idle = content.Load<Texture2D>("Enemy/Slime/Idel");
            _animation.Add(SlimeAnimation.Idle, new Animation.Animation(idle, true, 1f));

            // Run
            var run = content.Load<Texture2D>("Enemy/Slime/Run");
            _animation.Add(SlimeAnimation.Run, new Animation.Animation(run, true, 0.5f));

            // Dying
            var dying = content.Load<Texture2D>("Enemy/Slime/Dying");
            _animation.Add(SlimeAnimation.Dying, new Animation.Animation(dying, false, 0.2f));

            _animation.Play(SlimeAnimation.Idle);

            _effect = new Exploded(new Rectangle(0,0,170,75), content, "Enemy/Slime/Effects/FX_Dying", 10,new Point(170,75));
        }

        public override void Update(float dt, Rectangle PlayerPosition)
        {
            _effect.Update(dt, _dyeRect , Vector2.Zero);

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
            // Гравитация
            Velocity.Y += 0.5f; // ускорение вниз
            if (Velocity.Y > 5) Velocity.Y = 5; // лимит скорости падения

            switch (State)
            {
                case SlimeState.Idle:
                    UpdateIdle(dt, PlayerPosition);
                    break;

                case SlimeState.Chase:
                    UpdateChase(dt, PlayerPosition);
                    break;

                case SlimeState.Dying:
                    UpdateDying(dt);
                    break;
            }
            
            if (State == SlimeState.Dying)
                Velocity.X = 0;

            CollisionManeger.Instance.UpdateCollision(this);

            _animation.Update(dt);
        }
        private void UpdateDying(float dt) 
        {
            if (State == SlimeState.Dying)
            {
                if (_animation.IsAnimationPlayed)
                {
                    _effect.StartEffect();
                    State = SlimeState.Dead;
                    Respawn();
                }
            }
        }
        private void Respawn()
        {
            OnGround = false;
            Rect.X = Random.Shared.Next(0, (int)LevelManager.Instance.Level.TileMap.ReturnSizeMapX());
            Rect.Y = 0;
   
            Velocity.X = 0;

            _respTimer = Random.Shared.Next(5, 30);
            Active = false;
            ActiveCollider = true;
            
            _animation.Play(SlimeAnimation.Idle);
            State = SlimeState.Idle;
        }
        private void UpdateChase(float dt, Rectangle PlayerPosition)
        {
            if (Velocity.X != 0)
                _rotate = Velocity.X < 0;//Направление текстуры

            if (_dx > AggroDistance * 2 || _dy > AggroDistance)
            {
                State = SlimeState.Idle;
                Velocity.X = 0;
                _animation.Play(SlimeAnimation.Idle);
                return;
            }
            if (OnGround)
            {
                if (PlayerPosition.X < Rect.X)
                    Velocity.X = -1f;
                if (PlayerPosition.X >= Rect.X)
                    Velocity.X = 1f;
            }
        }
        private void UpdateIdle(float dt, Rectangle PlayerPosition)
        {
            Velocity.X = 0;

            if (_dx < AggroDistance && _dy < AggroDistance * 0.5f)
            {
                State = SlimeState.Chase;
                _animation.Play(SlimeAnimation.Run);
            }
        }
        public void TakeDamage(int damage, Vector2 knockback)
        {
            //if (_damageCooldown > 0) return; // ещё не прошло время защиты

            Health -= damage;
            if (Health < 0) Health = 0;

            State = SlimeState.Dying;

            ActiveCollider = false;

            _animation.Play(SlimeAnimation.Dying);

            _dyeRect = Rect;
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
