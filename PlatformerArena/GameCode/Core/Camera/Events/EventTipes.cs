using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class ChangeSceneEvent
    {
        public readonly string SceneName;
        public ChangeSceneEvent(string sceneName) => SceneName = sceneName;
    }
    public class ExitGameEvent { }
    public class LevelСompletedEvent
    {
        public readonly int Score;
        public LevelСompletedEvent(int score) => Score = score;
    }
    public class CoinColectEvent
    {
        public readonly int Points;
        public CoinColectEvent(int points) => Points = points;
    }
    public class ScoreColectEvent
    {
        public readonly int Points;
        public ScoreColectEvent(int points) => Points = points;
    }
    public class UpdateScoreEvent
    {
        public readonly int Score;
        public UpdateScoreEvent(int score) => Score = score;
    }
    public class SavePlayerDataEvent
    {
        public readonly int Score;
        public SavePlayerDataEvent(int score) => Score = score;
    }
    public class UpdateHealthEvent
    {
        public readonly int Health;
        public UpdateHealthEvent(int health) => Health = health;
    }
    public class ShowColliderEvent
    {
        public readonly bool ShowCollider;
        public ShowColliderEvent(bool shoCollider) => ShowCollider = shoCollider;
    }
    public class LandingEffectEvent { }
    public class DeathFadeEffectEvent { }
}


