using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Core
{
    public class CameraShakeEffect
    {
        private float _duration;
        private float _intensity;
        private float _timer;
        private Vector2 _offset;
        private Random _rand = new();

        public Vector2 Offset => _offset;
        public bool IsActive => _timer > 0;

        public void Start(float duration, float intensity)
        {
            _duration = duration;
            _intensity = intensity;
            _timer = duration;
        }

        public void Update(float dt)
        {
            if (_timer <= 0)
            {
                _offset = Vector2.Zero;
                return;
            }

            _timer -= dt;

            _offset = new Vector2(
                (float)(_rand.NextDouble() * 2 - 1) * _intensity,
                (float)(_rand.NextDouble() * 2 - 1) * _intensity
            );

            // плавное затухание
            //_intensity *= 0.9f;
            _intensity = MathHelper.Lerp(_intensity, 0, dt * 5f);
        }
    }
}

