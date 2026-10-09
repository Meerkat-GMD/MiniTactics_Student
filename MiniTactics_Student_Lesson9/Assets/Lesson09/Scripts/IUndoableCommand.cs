namespace MiniTactics.Lesson09
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
