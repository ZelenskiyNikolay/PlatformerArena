using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class SoundManager
    {
        private static SoundManager _instance;
        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SoundManager();
                return _instance;
            }
        }


        private readonly Dictionary<string, SoundEffect> _sfx = new();
        private readonly Dictionary<string, SoundEffect> _music = new();

        private SoundEffectInstance _currentMusic;

        public float SfxVolume { get; set; } = 0.5f;
        public float MusicVolume { get; set; } = 0.35f;

        private ContentManager _content;
        public void Init(ContentManager content)
        {
            _content = content;
            LoadAllSounds();
        }
        private void LoadAllSounds()
        {
            // Подгружаем все SFX
            //LoadSFX("player_hit");
            //LoadSFX("mob_hit");
            //LoadSFX("jump");
            //LoadSFX("button");
            LoadSFX("1");
            LoadSFX("2");
            // Музыка
            LoadMusic("BosFite");
            LoadMusic("Single");

            // Примечание:
            // Просто добавляй новые методы LoadSFX() / LoadMusic(), когда положишь файлы в Content
        }
        public void LoadSFX(string name)
        {
            try
            {
                var sfx = _content.Load<SoundEffect>($"Sound/SFX/{name}");
                _sfx[name] = sfx;
            }
            catch { }
        }

        public void LoadMusic(string name)
        {
            try
            {
                var music = _content.Load<SoundEffect>($"Sound/Music/{name}");
                _music[name] = music;
            }
            catch { }
        }
        public void Update() { }
        public void PlaySFX(string name)
        {
            if (_sfx.TryGetValue(name, out var sfx))
            {
                var inst = sfx.CreateInstance();
                inst.Volume = SfxVolume;
                inst.Play();
            }
        }

        public void PlayMusic(string name, bool loop = true)
        {
            if (_music.TryGetValue(name, out var mus))
            {
                _currentMusic?.Stop();

                _currentMusic = mus.CreateInstance();
                _currentMusic.Volume = MusicVolume;
                _currentMusic.IsLooped = loop;
                _currentMusic.Play();
            }
        }

        public void StopMusic()
        {
            _currentMusic?.Stop();
            _currentMusic = null;
        }

    }
}
