using Animation;
using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;


namespace FX
{
    public class LandingEffect : Effect
    {
        private AnimationController _effectAnimation;
        private AnimationId Landing;
        private Rectangle _dest; 
        private Color _color = Color.Gray;
        public Color SetColor { set { _color = value; } } 
        public LandingEffect(Rectangle RectangleObject, ContentManager Content)
        {
            _effectAnimation = new AnimationController();
            _dest = new Rectangle(RectangleObject.X -50,RectangleObject.Y,150,150);
            _color.A = 200;
            LoadContent(Content);
            EventManager.Instance.Subscribe<LandingEffectEvent>(StartEffect);
        }
        private void StartEffect(LandingEffectEvent e)
        {
            IsActive = true;
            _effectAnimation.Play(Landing);
        }
        public override void StartEffect()
        {
            IsActive = true;
            _effectAnimation.Play(Landing);
        }
        private void LoadContent(ContentManager Content)
        {

            var texture = Content.Load<Texture2D>("Player/Effects/Landing");
            Landing = new AnimationId("Landing");
            _effectAnimation.Add(Landing, new Animation.Animation(texture,0,10,180,85, false, 0.1f));
            _effectAnimation.Play(Landing); // нужно для обновления структуры....
        }
        public void Update(float dt, Rectangle rectangleObject, Vector2 vectorMov)
        {
            if (!IsActive) return;

            _dest = new Rectangle(rectangleObject.X - 25, rectangleObject.Y + 25, 100, 50);
            _effectAnimation.Update(dt);
            if (_effectAnimation.IsAnimationPlayed)
                IsActive = false;
        }
        public override void Update(GameTime gameTime, Rectangle rectangleObject, Vector2 vectorMov)
        {
            if (!IsActive) return;

            _dest = new Rectangle(rectangleObject.X - 25, rectangleObject.Y+25, 100, 50);
            _effectAnimation.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
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
            EventManager.Instance.Unsubscribe<LandingEffectEvent>(StartEffect);
        }
    }
}

