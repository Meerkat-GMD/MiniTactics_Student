namespace MiniTactics.Lesson03
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
