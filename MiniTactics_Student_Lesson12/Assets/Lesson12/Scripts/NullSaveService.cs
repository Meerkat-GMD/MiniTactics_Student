namespace MiniTactics.Lesson12
{
    public sealed class NullSaveService : ISaveService
    {
        public CampaignSave Load() => default;
        public void Save(CampaignSave state) { }
    }
}
