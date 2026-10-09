using UnityEngine;

namespace MiniTactics.Lesson12
{
    public static class Services
    {
        public static IAudioService Audio { get; set; } = new NullAudioService();
        public static ISaveService Save { get; set; } = new NullSaveService();

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset()
        {
            Audio = new NullAudioService();
            Save = new NullSaveService();
        }
    }
}
