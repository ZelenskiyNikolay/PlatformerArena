using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;


namespace Entity
{
    public class Enemys
    {
        public Texture2D Texture;
        public Rectangle Rect;
        public Rectangle Srect;

        public Vector2 Velocity;
        public bool OnGround;
        public bool Active { get; set; } = true;
        public bool ActiveCollider { get; set; } = true;

        public Enemys(Texture2D texture, Rectangle rect, Rectangle srect)
        {
            Texture = texture;
            Rect = rect;
            Srect = srect;
            Velocity = new();
        }
        public virtual void Update(float dt, Rectangle PlayerPosition) { }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(Texture, Rect, Srect, Color.White);
        }
        public virtual void Unload() { }
        
    }
}




