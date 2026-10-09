using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class PlayerPrefsSaveService : ISaveService
    {
        public CampaignSave Load()
        {
            return new CampaignSave
            {
                HasProgress = PlayerPrefs.GetInt("MiniTactics11.Progress") == 1,
                StartingStageIndex = PlayerPrefs.GetInt("MiniTactics11.Stage"),
                Completed = PlayerPrefs.GetInt("MiniTactics11.Completed") == 1,
                Muted = PlayerPrefs.GetInt("MiniTactics11.Muted") == 1
            };
        }

        public void Save(CampaignSave state)
        {
            PlayerPrefs.SetInt("MiniTactics11.Progress", state.HasProgress ? 1 : 0);
            PlayerPrefs.SetInt("MiniTactics11.Stage", state.StartingStageIndex);
            PlayerPrefs.SetInt("MiniTactics11.Completed", state.Completed ? 1 : 0);
            PlayerPrefs.SetInt("MiniTactics11.Muted", state.Muted ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
