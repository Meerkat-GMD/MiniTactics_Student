using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class ClipAudioService : IAudioService
    {
        private readonly AudioSource _source;
        private readonly AudioClip[] _clips;

        public ClipAudioService(AudioSource source, AudioClip[] clips)
        {
            _source = source;
            _clips = clips;
        }

        public void Play(SoundCue cue) => _source.PlayOneShot(_clips[(int)cue]);
    }
}
