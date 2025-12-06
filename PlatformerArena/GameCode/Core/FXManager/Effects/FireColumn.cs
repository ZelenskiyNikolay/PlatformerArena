using Animation;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Reflection.Metadata;

namespace FX
{
    public class FireColumn : Effect
    {
        private AnimationController _effectAnimation;
        private AnimationId Fire;
        private Rectangle _dest;
        private float _y;

        public Rectangle GetCollider { get { return _dest; } }
        public FireColumn(Rectangle RectangleDest, ContentManager Content, string TextureName, int NumFrames, Point SizeFrame)
        {
            Init(RectangleDest, Content, TextureName, NumFrames, SizeFrame);
        }
        private void Init(Rectangle RectangleDest, ContentManager Content, string TextureName, int NumFrames, Point SizeFrame)
        {
            _effectAnimation = new AnimationController();
            _dest = RectangleDest;
            //_size = new Point(RectangleDest.Width, RectangleDest.Height);
            LoadContent(Content, TextureName, NumFrames, SizeFrame);
        }
        private void LoadContent(ContentManager Content, string Name, int NumFrames, Point SizeFrame)
        {
            var texture = Content.Load<Texture2D>(Name);
            Fire = new AnimationId("FireColumn");
            _effectAnimation.Add(Fire, 
                new Animation.Animation(texture, 0, NumFrames, SizeFrame.X, SizeFrame.Y, true, 0.2f));
            _effectAnimation.Play(Fire); // нужно для обновления структуры....
        }
        public void StartEffect(float yStart)
        {
            _y = yStart;
            IsActive = true;
            _effectAnimation.Play(Fire);
        }
        public void Update(float dt, Rectangle rectangleObject, Vector2 vectorMov)
        {
            if (!IsActive) return;

            _y -=500 * dt;

            if (_y < -_dest.Height)
            {
                IsActive = false;
                return;
            }
            _dest.Y = (int)_y;
            _dest.X = (int)rectangleObject.X + rectangleObject.Width / 2;

            _effectAnimation.Update(dt);
        }
        
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
                _effectAnimation.Draw(spriteBatch, _dest, Color.White);
        }
        public override void StartEffect() { }
        public override void Unload() { }
        public override void Update(GameTime gameTime, Rectangle rectangleObject, Vector2 vectorMov) { }
    }
}
