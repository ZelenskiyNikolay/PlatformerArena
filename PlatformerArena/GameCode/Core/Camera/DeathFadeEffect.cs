using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Core
{
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using System;

    public class DeathFadeEffect
    {
        private readonly Texture2D blankTexture;
        private float fade = 0f;
        private bool isFadingOut = true;
        private float speed = 1f;

        public bool IsActive { get; private set; }

        public DeathFadeEffect()
        {
            blankTexture = GameManager.Instance.Blank;
            blankTexture.SetData(new[] { Color.White });
        }

        public void Start()
        {
            IsActive = true;
            fade = 0f;
            isFadingOut = true;
        }

        public void Update(float dt)
        {
            if (!IsActive) return;

            if (isFadingOut)
            {
                fade += dt * speed;
                if (fade >= 1f)
                {
                    fade = 1f;
                    isFadingOut = false;
                }
            }
            else
            {
                fade -= dt * speed;
                if (fade <= 0f)
                {
                    fade = 0f;
                    IsActive = false;

                }
            }
        }

        public void Draw(SpriteBatch spriteBatch, Point screenDept)
        {
            spriteBatch.Draw(
                blankTexture,
                new Rectangle(0, 0, screenDept.X, screenDept.Y),
                new Color(255, 0, 0) * fade
            );
        }
    }

}



