namespace MiniTactics.Lesson10
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
