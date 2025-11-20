using Animation;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FX
{
    public class DustEffect : Effect
    {
        private AnimationController _effectAnimation;
        private AnimationId Dust;
        private Rectangle _dest;
        private bool _rotation = false;
        private Color _color = Color.Gray;
        public DustEffect(Rectangle RectangleObject, ContentManager Content)
        {
            _effectAnimation = new AnimationController();
            _dest = RectangleObject;
            _color.A = 100;
            LoadContent(Content);
        }
        private void LoadContent(ContentManager Content)
        {

            var texture = Content.Load<Texture2D>("Player/Effects/Dust");
            Dust = new AnimationId("Dust");
            _effectAnimation.Add(Dust, new Animation.Animation(texture,true,0.1f));
            _effectAnimation.Play(Dust); // нужно для обновления структуры....
        }
        public override void Update(GameTime gameTime, Rectangle rectangleObject, Vector2 vectorMov)
        {
            if(vectorMov == Vector2.Zero || rectangleObject == Rectangle.Empty)
            {
                IsActive = false; return;
            }
            if (vectorMov.X == 0)
            {
                IsActive = false;
                return;
            }
            _dest = rectangleObject;
            if (vectorMov.X > 0)
            {
                _rotation = true;
                _dest.X -= 50;
                _dest.Width += 50;
                IsActive = true;
                if (_effectAnimation.IsAnimationPlayed)
                {
                    _effectAnimation.Play(Dust);
                }
            }
            else if (vectorMov.X < 0)
            {
                _rotation = false;
                _dest.Width += 50;
                IsActive = true;
                if (_effectAnimation.IsAnimationPlayed)
                {
                    _effectAnimation.Play(Dust);
                }
            }

            _effectAnimation.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            if (IsActive)
                _effectAnimation.Draw(spriteBatch, _dest,_color , _rotation);
        }
        public override void Unload()
        {
        }

        public override void StartEffect()
        {
            throw new NotImplementedException();
        }
    }
}

