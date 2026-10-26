using UnityEngine;

namespace MiniTactics.Lesson12
{
    public sealed class PlayerPrefsSaveService : ISaveService
    {
        public CampaignSave Load()
        {
            return new CampaignSave
            {
                HasProgress = PlayerPrefs.GetInt("MiniTactics.Course.Campaign12.v1.Progress") == 1,
                StartingStageIndex = PlayerPrefs.GetInt("MiniTactics.Course.Campaign12.v1.Stage"),
                Completed = PlayerPrefs.GetInt("MiniTactics.Course.Campaign12.v1.Completed") == 1,
                Muted = PlayerPrefs.GetInt("MiniTactics.Course.Campaign12.v1.Muted") == 1
            };
        }

        public void Save(CampaignSave state)
        {
            PlayerPrefs.SetInt("MiniTactics.Course.Campaign12.v1.Progress", state.HasProgress ? 1 : 0);
            PlayerPrefs.SetInt("MiniTactics.Course.Campaign12.v1.Stage", state.StartingStageIndex);
            PlayerPrefs.SetInt("MiniTactics.Course.Campaign12.v1.Completed", state.Completed ? 1 : 0);
            PlayerPrefs.SetInt("MiniTactics.Course.Campaign12.v1.Muted", state.Muted ? 1 : 0);
            PlayerPrefs.Save();
        }
    }
}
