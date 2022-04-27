namespace ZoDream.Studio.Commands
{
    public interface ICommandManager
    {
        public void ExecuteCommand(ICommand command);
        public void Undo();
        public void ReverseUndo(); // 反撤销

        // 以下事件可用于控制撤销与反撤销图标的启用
        public event CommandStateChangedEventHandler? UndoStateChanged;  // bool参数表明当前是否有可撤销的操作
        public event CommandStateChangedEventHandler? ReverseUndoStateChanged;  // bool参数表明当前是否有可反撤销的操作
    }

    public delegate void CommandStateChangedEventHandler(bool value);
}
