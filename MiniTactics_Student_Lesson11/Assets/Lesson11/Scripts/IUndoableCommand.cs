namespace MiniTactics.Lesson11
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
