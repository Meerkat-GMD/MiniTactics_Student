namespace MiniTactics.Lesson04
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
