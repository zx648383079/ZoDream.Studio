namespace ZoDream.Studio.Commands
{
    public interface IBackableCommand: ICommand
    {
        public void Undo();
    }
}
