using ZoDream.Shared.Models;

namespace ZoDream.Studio.Controls
{
    public enum MoveStatus
    {
        None,
        Move,
        SizeRight,
        SizeLeft,
    }

    public delegate void PianoKeyEventHandler(object sender, PianoKeyEventArgs e);

    public delegate void PreviewUpdatedEventHandler();

    public class PianoKeyEventArgs
    {
        public PianoKeyEventArgs(PianoKey key, bool isPressed)
        {
            Key = key;
            IsPressed = isPressed;
        }

        public PianoKey Key { get; private set; }

        public bool IsPressed { get; private set; }
        public bool Handle { get; set; } = false;
    }

    public class TrackActionEventArgs
    {
        public TrackActionEventArgs(ProjectTrackItem data, bool isEdit)
        {
            Data = data;
            IsEdit = isEdit;
        }

        public ProjectTrackItem Data { get; private set; }

        public bool IsEdit { get; private set; }
        public bool Handle { get; set; } = false;
    }
}
