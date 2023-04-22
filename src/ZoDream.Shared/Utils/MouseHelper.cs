using System;

namespace ZoDream.Shared.Utils
{
    public class MouseHelper
    {

        public double BeginX { get; private set; }
        public double BeginY { get; private set; }
        public double LastY { get; private set; }
        public double LastX { get; private set; }
        public double CurrentY { get; private set; }
        public double CurrentX { get; private set; }
        public bool IsBegin { get; private set; }
        public bool IsEnd { get; private set; }
        public int EventTag { get; private set; }

        public bool IsTouchMove { get; private set; }

        public double OffsetX => IsBegin ? CurrentX - LastX : 0;
        public double OffsetY => IsBegin ? CurrentY - LastY : 0;
        public double TotalOffsetY => IsBegin ? CurrentY - BeginY : 0;
        public double TotalOffsetX => IsBegin ? CurrentX - BeginX : 0;

        public bool IsTop => IsBegin && CurrentY < LastY;
        public bool IsBottom => IsBegin && CurrentY > LastY;
        public bool IsLeft => IsBegin && CurrentX < LastX;
        public bool IsRight => IsBegin && CurrentX > LastX;

        public bool IsGlobeTop => IsBegin && CurrentY < BeginY;
        public bool IsGlobeBottom => IsBegin && CurrentY > BeginY;
        public bool IsGlobeLeft => IsBegin && CurrentX < BeginX;
        public bool IsGlobeRight => IsBegin && CurrentX > BeginX;


        public event MouseMoveEventHandler? OnMouseMove;
        public event MouseMoveEventHandler? OnMouseUp;
        public event MouseMoveEventHandler? OnMouseDown;

        public void MouseDown(double x, double y)
        {
            MouseDown(0, x, y);
        }

        public void MouseDown(int tag, double x, double y)
        {
            if (!IsEnd)
            {
                // 结束上一次的
                OnMouseMove?.Invoke(this, EventTag);
            }
            EventTag = tag;
            CurrentX = LastX = BeginX = x;
            CurrentY = LastY = BeginY = y;
            IsBegin = true;
            IsEnd = false;
            IsTouchMove = false;
            OnMouseDown?.Invoke(this, EventTag);
        }

        public void MouseMove(double x, double y)
        {
            if (!IsBegin || IsEnd)
            {
                return;
            }
            IsTouchMove = true;
            LastX = CurrentX;
            LastY = CurrentY;
            CurrentX = x;
            CurrentY = y;
            OnMouseMove?.Invoke(this, EventTag);
        }

        public void MouseUp(double x, double y)
        {
            if (!IsBegin)
            {
                return;
            }
            IsEnd = true;
            if (CurrentX != x || CurrentY != y)
            {
                LastX = CurrentX;
                LastY = CurrentY;
                CurrentX = x;
                CurrentY = y;
            }
            OnMouseMove?.Invoke(this, EventTag);
            OnMouseUp?.Invoke(this, EventTag);
        }

        
    }

    public delegate void MouseMoveEventHandler(MouseHelper sender, int tag);
}
