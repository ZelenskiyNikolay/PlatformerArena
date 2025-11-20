using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Animation
{
    public class Animation
    {

        public Texture2D Texture;
        public Rectangle[] Frames;
        public float FrameTime = 0.1f;// по умолчанию 10 fps
        public bool Loop;

        public bool AnimationPlayed = false;

        public Animation(Texture2D texture,bool loop = false,float frameTime = 0.1f)
        {
            Texture = texture;
            Frames = AutoSliceRow(texture);
            Loop = loop;
            FrameTime = frameTime;
        }
        public Animation(Texture2D texture, Rectangle[] frames, float frameTime, bool loop)
        {
            Texture = texture;
            Frames = frames;
            FrameTime = frameTime;
            Loop = loop;
        }
        public Animation(Texture2D texture, int row, int frameCount, int frameWidth, int frameHeight, bool loop = true, float frameTime = 0.1f)
        {
            Texture = texture;
            Frames = SliceRow(row, frameCount, frameWidth, frameHeight);
            Loop = loop;
            FrameTime = frameTime;
        }

        public static Rectangle[] SliceRow(
        int row, int frameCount, int frameWidth, int frameHeight)
        {
            Rectangle[] frames = new Rectangle[frameCount];

            for (int i = 0; i < frameCount; i++)
                frames[i] = new Rectangle(
                    i * frameWidth,
                    row * frameHeight,
                    frameWidth,
                    frameHeight);

            return frames;
        }
        public static Rectangle[] AutoSliceRow(Texture2D texture2D)
        {
            int frameCount = texture2D.Width / texture2D.Height;
            int frameSize = texture2D.Height;
            Rectangle[] frames = new Rectangle[frameCount];

            for (int i = 0; i < frameCount; i++)
                frames[i] = new Rectangle(i * frameSize,0,frameSize,frameSize);

            return frames;
        }

    }
}

