using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace FX
{
    public class EffectManager
    {
        private List<Effect> _effects = new();

        public void Add(Effect effect)
        {
            _effects.Add(effect);
        }

        public void Update(GameTime gameTime, Rectangle rectangleObject, Vector2 vectorMov)
        {
            foreach (var e in _effects)
                if (e != null)
                    e.Update(gameTime, rectangleObject, vectorMov);          
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var e in _effects)
                if (e != null)
                    e.Draw(spriteBatch);
        }
        public void Unload()
        {
            foreach (var e in _effects)
                if (e != null && e.IsActive)
                    e.Unload();
        }
    }
}

