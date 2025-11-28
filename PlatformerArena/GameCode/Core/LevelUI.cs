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
        public Texture2D _heart;
        private int _playerHeart;
        private int _score;
        private SpriteFont _font;
        public bool IsUiPrinted {  get; set; }
        public int Setheart { set { _playerHeart =value / 5; } }
        private LevelUI() 
        {
            _score = 0;
            IsUiPrinted = false;
            _font = GameManager.Instance.CoreFont;
            EventManager.Instance.Subscribe<UpdateScoreEvent>(UpdateScore);
            EventManager.Instance.Subscribe<UpdateHealthEvent>(UpdateHealth);
        }
        private void UpdateHealth(UpdateHealthEvent e) => Setheart = e.Health;
        
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
            if (_heart != null)
                for (int i = 0; i < _playerHeart; i++)
                {
                    spriteBatch.Draw(_heart, new Vector2(400+_heart.Width*i+20, 0), _heart.Bounds, Color.White);
                }
        }
    }
}

