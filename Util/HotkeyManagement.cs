using System.Runtime.InteropServices;
using System.Windows.Input;

namespace autoclicker.Util;

public class HotkeyManagement : IDisposable
{
    // Registers a hot key with Windows.
    [DllImport("user32.dll")]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    // Unregisters the hot key with Windows.
    [DllImport("user32.dll")]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    private IntPtr _hWnd;
    private int _hotkeyId;
    private bool _disposed = false;

    public HotkeyManagement(IntPtr hWnd, int hotkeyId)
    {
        _hWnd = hWnd;
        _hotkeyId = hotkeyId;
    }

    public bool SetHotkey(Key key, ModifierKeys modifier)
    {
        return RegisterHotKey(_hWnd, _hotkeyId, (uint)modifier, (uint)key);
    }

    public bool RemoveHotkey()
    {
        return UnregisterHotKey(_hWnd, _hotkeyId);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            // Dispose managed resources if any
        }

        // Dispose unmanaged resources
        RemoveHotkey();
        _disposed = true;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~HotkeyManagement()
    {
        Dispose(false);
    }
}