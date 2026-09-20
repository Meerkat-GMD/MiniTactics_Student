namespace MiniTactics.Lesson07
{
    public interface IUndoableCommand : ICommand
    {
        void Undo();
    }
}
