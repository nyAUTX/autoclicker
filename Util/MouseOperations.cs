using System.Runtime.InteropServices;

namespace autoclicker.Util;

public class MouseOperations
{
    public enum MouseButtons
    {
        LEFT,
        MIDDLE,
        RIGHT
    }
    
    
    [Flags]
    public enum MouseEventFlags
    {
        LeftDown = 0x00000002,
        LeftUp = 0x00000004,
        MiddleDown = 0x00000020,
        MiddleUp = 0x00000040,
        Move = 0x00000001,
        Absolute = 0x00008000,
        RightDown = 0x00000008,
        RightUp = 0x00000010
    }
    
    public static void PressMouseButton(MouseButtons button)
    {
        switch (button)
        {
            case MouseButtons.LEFT:
                MouseEvent(MouseEventFlags.LeftDown);
                MouseEvent(MouseEventFlags.LeftUp);
                break;
            case MouseButtons.MIDDLE:
                MouseEvent(MouseEventFlags.MiddleDown);
                MouseEvent(MouseEventFlags.MiddleUp);
                break;
            case MouseButtons.RIGHT:
                MouseEvent(MouseEventFlags.RightDown);
                MouseEvent(MouseEventFlags.RightUp);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(button), button, null);
        }
    }
    
    [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool SetCursorPos(int x, int y);      

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetCursorPos(out MousePoint lpMousePoint);

    [DllImport("user32.dll")]
    private static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

    public static void SetCursorPosition(int x, int y) 
    {
        SetCursorPos(x, y);
    }

    public static void SetCursorPosition(MousePoint point)
    {
        SetCursorPos(point.X, point.Y);
    }

    public static MousePoint GetCursorPosition()
    {
        MousePoint currentMousePoint;
        var gotPoint = GetCursorPos(out currentMousePoint);
        if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
        return currentMousePoint;
    }
    
   
    public static void MouseEvent(MouseEventFlags value)
    {
        MousePoint position = GetCursorPosition();

        mouse_event
            ((int)value,
                position.X,
                position.Y,
                0,
                0)
            ;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MousePoint
    {
        public int X;
        public int Y;

        public MousePoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
}