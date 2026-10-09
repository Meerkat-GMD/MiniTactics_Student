namespace MiniTactics.Lesson12
{
    public interface ISaveService
    {
        CampaignSave Load();
        void Save(CampaignSave state);
    }
}
