namespace MiniTactics.Lesson05
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
