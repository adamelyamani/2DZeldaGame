using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;

namespace Project.App
{
    public class SoundManager
    {
        private static SoundManager _instance;
        private readonly Dictionary<SfxEnums, SoundEffect> _sfx = new();
        private readonly List<SoundEffectInstance> _currentSfx = new();
        private Song _music;

        public static SoundManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new SoundManager();
                }

                return _instance;
            }
        }

        public void Initialize(Game1 game)
        {
            foreach (SfxEnums sfx in Enum.GetValues(typeof(SfxEnums)))
            {   
                _sfx.Add(sfx, game.Content.Load<SoundEffect>(sfx.ToString()));
            }

            _music = game.Content.Load<Song>("Marching on Moss");

        }

        public void ClearSounds()
        {
            _sfx.Clear();
            _currentSfx.Clear();
            _music = null;
        }

        public void PlaySound(SfxEnums sfx)
        {
            var sfxInstance = _sfx[sfx].CreateInstance();
            _currentSfx.Add(sfxInstance);
            sfxInstance.Play();
        }

        public static void MusicPause(bool rewind = false)
        {
            if (rewind) MediaPlayer.Stop();
            else MediaPlayer.Pause();
        }

        public static void MusicResume()
        {
            MediaPlayer.Resume();
        }

        public void MusicPlay()
        {
            MediaPlayer.Play(_music);
            MediaPlayer.IsRepeating = true;
        }

        public void MusicToggle()
        {
            if (MediaPlayer.State == MediaState.Stopped)
            {
                MusicPlay();
            }
            else if (MediaPlayer.State == MediaState.Paused)
            {
                MusicResume();
            }
            else if (MediaPlayer.State == MediaState.Playing)
            {
                MusicPause();
            }
        }

        void PauseAllSounds(bool pauseMusic = true, bool stop = false)
        {
            if (pauseMusic)
            {
                if (stop)
                    MediaPlayer.Stop();
                else if (MediaPlayer.State == MediaState.Playing)
                    MediaPlayer.Pause();
            }

            foreach (var sfxInstance in _currentSfx)
            {

                if (stop)
                {
                    sfxInstance.Stop();
                }
                else if (sfxInstance.State == SoundState.Playing)
                {
                    sfxInstance.Pause();
                }
            }

        }

        public void ResumeAllSounds()
        {

            if (MediaPlayer.State == MediaState.Paused) MediaPlayer.Resume();
            else if (MediaPlayer.State == MediaState.Stopped) MediaPlayer.Play(_music);

            foreach (var sfxInstance in _currentSfx)
            {
                if (sfxInstance.State == SoundState.Paused) sfxInstance.Resume();
            }
        }

        public static void SetMusicVolume(float vol)
        {
            MediaPlayer.Volume = vol;
        }

        public void Update()
        {
            foreach (var sfxInstance in _currentSfx)
            {
                // For all sfx that have finished playing
                if (sfxInstance.State == SoundState.Stopped)
                {
                    // Cleans up the sound and sets the "IsDisposed" flag for deletion
                    sfxInstance.Dispose();
                }
            }

            // Delete finished sound effects
            _currentSfx.RemoveAll(sfxInstance => sfxInstance.IsDisposed);
        }
    }
}
