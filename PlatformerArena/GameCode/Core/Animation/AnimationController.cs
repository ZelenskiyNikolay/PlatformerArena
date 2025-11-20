using Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Reflection.Metadata;

namespace Animation
{
    public class AnimationController
    {
        private Dictionary<AnimationId, Animation> _animations = new();

        private Animation _currentAnimation;
        private AnimationId _currentId;

        private float _timer;
        private int _frame;

        public bool IsAnimationPlayed { get { return _currentAnimation.AnimationPlayed; } }
        public void Add(AnimationId id, Animation anim)
        {
            _animations[id] = anim;
        }

        public void Play(AnimationId id)
        {
            if (_currentAnimation != null)
                if (_currentAnimation.Loop == false)
                {
                    _currentAnimation.AnimationPlayed = false;
                }
                else if (_currentId.Equals(id)) return;

            _currentId = id;
            _currentAnimation = _animations[id];

            _timer = 0;
            _frame = 0;
        }

        public void Update(float dt)
        {
            if (_currentAnimation == null) return;

            _timer += dt;
            if (_timer >= _currentAnimation.FrameTime)
            {
                _timer -= _currentAnimation.FrameTime;
                _frame++;

                if (_frame >= _currentAnimation.Frames.Length)
                {
                    _frame = _currentAnimation.Loop ? 0 : _currentAnimation.Frames.Length - 1;
                    if (_currentAnimation.Loop == false)
                        _currentAnimation.AnimationPlayed = true;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 pos, Color color)
        {
            spriteBatch.Draw(_currentAnimation.Texture, pos, _currentAnimation.Frames[_frame], color);
        }
        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
        {
            spriteBatch.Draw(_currentAnimation.Texture, rectangle, _currentAnimation.Frames[_frame], color);
        }
        public void Draw(SpriteBatch spriteBatch, Rectangle rectangle, Color color, bool rotate)
        {
            if (rotate)
                spriteBatch.Draw(_currentAnimation.Texture, rectangle, _currentAnimation.Frames[_frame], color,
                    0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 0f);
            else
            {
                spriteBatch.Draw(_currentAnimation.Texture, rectangle, _currentAnimation.Frames[_frame], color);
            }
        }
    }

}

