using Core;
using Levels;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Entity
{
    public class Coin
    {
        private Texture2D _texture;
        private Rectangle _dest;
        private Animation _animatiom;
        private const int _speed = 10;
        private const int _numberFrames = 4;
        private int _timeAnim;
        private int _frameIndex;

        public bool IsActive { get; set;}
        public Rectangle Dest { get {  return _dest; } }
        public Coin(Texture2D texture, List<Rectangle> animation,Point resp)
        {
            _texture = texture;
            _dest = new Rectangle(resp.X, resp.Y, 32, 32);
            _animatiom = new Animation(animation);
            _timeAnim = _speed;
            _frameIndex = 0;
            IsActive = true;
        }
        public void Update(GameTime gameTime)
        {
            if (!IsActive)return;

            _timeAnim--;
            if (_timeAnim < 0)
            {
                _timeAnim = _speed;
                _frameIndex ++;
                if( _frameIndex >= _numberFrames )
                    _frameIndex = 0;
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (!IsActive) return;
            spriteBatch.Draw(_texture, _dest,_animatiom.GetFrame(_frameIndex), Color.White);
        }

        private struct Animation
        {
            private List<Rectangle> _anim;
            public Animation(List<Rectangle> animation)
            {
                _anim = animation;
            }
            public Rectangle GetFrame(int index)
            {
                return _anim[index];
            }
        }
    }
    
}

