using Animation;
using FX;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    public enum Boss1State
    {
        Chase,
        Stage2,
        Dying,
        Dead,
        Patrol
    }
    internal class Boss1 : Enemys
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
        public int Health { get; set; } = 50;

        private const float AggroDistance = 5 * 50; // 5 тайлов по 50px

        //protected virtual int DefaultHealth => 10; //Поле для наследников

        private float _dx, _dy;
        private float _player_dx;

        private Exploded _effect;
        private Rectangle _dyeRect;

        private Rectangle _tempRect = new();
        private Rectangle _tempRect2 = new();
        public static class Boss1Animation
        {
            public static readonly AnimationId Run = new("Run");
            public static readonly AnimationId Dying = new("Dying");
        }

        private AnimationController _animation;
        public Boss1(Texture2D texture, Rectangle rect, Rectangle srect) : base(texture, rect, srect)
        {
        }
    }
}
