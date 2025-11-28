using Core;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity
{
    internal class HeavyKnight : Knight
    {
        public HeavyKnight(ContentManager Content, Rectangle rect, Rectangle srect, int TileSize) : base(Content, rect, srect, TileSize)
        {
            Health = 20;
        }
        public override void Update(float dt, Rectangle PlayerPosition)
        {
            base.Update(dt, PlayerPosition);
        }


        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }
        public override void Unload()
        {
           base.Unload();
        }
    }
}