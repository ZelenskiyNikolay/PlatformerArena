using Animation;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace FX
{
    public class Exploded : Effect
    {
        private AnimationController _effectAnimation;
        private AnimationId Explooded;
        private Rectangle _dest;
        private Point _size;
        private Color _color = Color.Green;

        public Exploded(Rectangle RectangleDest, ContentManager Content, string TextureName, int NumFrames,Point SizeFrame)
        {
            _effectAnimation = new AnimationController();
            _dest = new Rectangle();// RectangleDest.X - 50, RectangleDest.Y, RectangleDest.Width, RectangleDest.Height);
            //_size = new Point(RectangleDest.Width, RectangleDest.Height);
            _size = new Point(200, 120);
            _color.A = 200;
            LoadContent(Content, TextureName, NumFrames,SizeFrame);
        }
        public Exploded(Rectangle RectangleObject, ContentManager Content)
        {
            _effectAnimation = new AnimationController();
            _dest = new Rectangle(RectangleObject.X - 50, RectangleObject.Y, 150, 150);
            _size = new Point(150, 100);
            _color.A = 200;
            LoadContent(Content);
        }
        private void LoadContent(ContentManager Content, string Name, int NumFrames, Point SizeFrame)
        {
            var texture = Content.Load<Texture2D>(Name);
            Explooded = new AnimationId("Explooded");
            _effectAnimation.Add(Explooded, new Animation.Animation(texture, 0, NumFrames, SizeFrame.X, SizeFrame.Y, false, 0.2f));
            _effectAnimation.Play(Explooded); // нужно для обновления структуры....
        }
        private void LoadContent(ContentManager Content)
        {

            var texture = Content.Load<Texture2D>("Player/Effects/Landing2");
            Explooded = new AnimationId("Explooded");
            _effectAnimation.Add(Explooded, new Animation.Animation(texture, 0, 16, 200, 120, false, 0.2f));
            _effectAnimation.Play(Explooded); // нужно для обновления структуры....
        }
        public new void Update(float dt, Rectangle rectangleObject, Vector2 vectorMov)
        {
            if (!IsActive) return;

            _color.A -= (byte)(150*dt);

            _dest = rectangleObject;

            _dest.X -= (_size.X - _dest.Width) / 2;
            _dest.Y -=(int)(_size.Y*0.8f - _dest.Height);
            _dest.Width = _size.X;
            _dest.Height = _size.Y;

            _effectAnimation.Update(dt);
            if (_effectAnimation.IsAnimationPlayed)
                IsActive = false;
        }
        public override void Draw(Microsoft.Xna.Framework.Graphics.SpriteBatch spriteBatch)
        {
            if (IsActive)
                _effectAnimation.Draw(spriteBatch, _dest, _color);
        }
        public override void Unload()
        {
        }

        public override void StartEffect()
        {
            IsActive = true;
            _effectAnimation.Play(Explooded);
        }

        public override void Update(GameTime gameTime, Rectangle rectangleObject, Vector2 vectorMov)
        {
            throw new NotImplementedException();
        }
    }
}

