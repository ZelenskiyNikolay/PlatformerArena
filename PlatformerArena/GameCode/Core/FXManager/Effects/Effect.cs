using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FX
{
    public abstract class Effect
    {
        public bool IsActive;
        public abstract void StartEffect();
        public abstract void Update(GameTime gameTime, Rectangle rectangleObject, Vector2 vectorMov);
        public abstract void Draw(SpriteBatch spriteBatch);
        public abstract void Unload();
    }
}

