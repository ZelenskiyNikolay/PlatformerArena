using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Core
{ 
    public class Camera
    {
        public Matrix Transform { get; private set; }
        private Vector2 _position;
        private Vector2 _targetPosition;

        public Vector2 Position { get { return _position; } }

        private readonly int _screenWidth;
        private readonly int _screenHeight;
        private readonly int _halfScreenHeight;
        private readonly int _halfScreenWidth;

        private readonly float _smoothSpeed;
        private readonly float _mapSizeX, _mapSizeY;

        private readonly float _maxX;
        private readonly float _maxY;

        public CameraShakeEffect Shake = new();

        public Camera(int ScreenWidth, int ScreenHeight, float MapSizeX, float MapSizeY, float SmoothSpeed = 0.01f)
        {
            _screenWidth = ScreenWidth;
            _screenHeight = ScreenHeight;
            _halfScreenHeight = ScreenHeight / 2;
            _halfScreenWidth = ScreenWidth / 2;
            _smoothSpeed = SmoothSpeed;
            _mapSizeX = MapSizeX;
            _mapSizeY = MapSizeY;
            _maxX = _mapSizeX - _screenWidth;
            _maxY = _mapSizeY - _screenHeight;
            _targetPosition = new();
            EventManager.Instance.Subscribe<LandingEffectEvent>(e =>
            {
                Shake.Start(0.3f, 4f);
            });
        }
        public void SetZeroPosition() => _position = Vector2.Zero;
        public void Update(Rectangle target, float dt)
        {
            _targetPosition.X = target.X - _halfScreenWidth;
            _targetPosition.Y = target.Y - _halfScreenHeight;

            _position.X += (_targetPosition.X - _position.X) * _smoothSpeed;
            _position.Y += (_targetPosition.Y - _position.Y) * _smoothSpeed;

            _position.X = MathHelper.Clamp(_position.X, 0, _maxX);
            _position.Y = MathHelper.Clamp(_position.Y, 0, _maxY);

            _position.X = (float)Math.Round(_position.X);
            _position.Y = (float)Math.Round(_position.Y);

            Transform = Matrix.CreateTranslation(-_position.X + Shake.Offset.X, -_position.Y + Shake.Offset.Y, 0);
                
            Shake.Update(dt);

        }
    }
}




