using Entity;
using Levels;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Input;
using System;


namespace Core
{
    public class LevelUI
    {
        private static LevelUI _instance;
        public static LevelUI Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new LevelUI();
                return _instance;
            }
        }
        private int _score;
        private SpriteFont _font;
        public bool IsUiPrinted {  get; set; }
        private LevelUI() 
        {
            _score = 0;
            IsUiPrinted = false;
            _font = GameManager.Instance.CoreFont;
            EventManager.Instance.Subscribe<UpdateScoreEvent>(UpdateScore);
        }
        public void SetFont(SpriteFont Font) =>_font = Font;
        public void SetScore(int Score) => _score = Score;
        private void UpdateScore(UpdateScoreEvent e)
        {
            _score = e.Score;
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, $"SCORE: {_score}  ", 
                Vector2.Zero, Color.Red, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
        }
    }
}

