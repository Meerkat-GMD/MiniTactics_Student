namespace MiniTactics.Lesson12
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
