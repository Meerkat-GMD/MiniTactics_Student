namespace MiniTactics.Lesson08
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
