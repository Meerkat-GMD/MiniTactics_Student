namespace MiniTactics.Lesson06
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
