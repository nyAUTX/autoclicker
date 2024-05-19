using System.ComponentModel;
using static autoclicker.Util.MouseOperations;

namespace autoclicker.Model;

public class AutoclickerViewModel : INotifyPropertyChanged
{
    
    public static AutoclickerViewModel Instance { get; } = new();

    private AutoclickerViewModel() { }
    
    private int _milliseconds = 10;
    private int _seconds;
    private int _minutes;
    private int _hours;

    private int _repetitions;
    private int _repetitionsDone;
    
    private bool _isRunning;
    
    #region Properties

    public bool IsRunning
    {
        get => _isRunning;
        set
        {
            _isRunning = value;
            NotifyPropertyChanged(nameof(IsRunning));
            NotifyPropertyChanged(nameof(IsNotRunning));
        }
    }
    
    public bool IsNotRunning => !IsRunning;
    
    public MouseButtons SelectedMouseButton { get; set; }
    
    public int Milliseconds
    {
        get => _milliseconds;
        set
        {
            if (value >= 1000)
            {
                Hours = 0;
                Minutes = 0;
                Seconds = value / 1000;
                _milliseconds = value % 1000;
            }
            else _milliseconds = value;
            NotifyPropertyChanged(nameof(Milliseconds));
        }
    }
    
    public int Seconds
    {
        get => _seconds;
        set
        {
            if(value >= 60)
            {
                Hours = 0;
                Minutes = value / 60;
                _seconds = value % 60;
            }
            else _seconds = value;
            NotifyPropertyChanged(nameof(Seconds));
        }
    }
    
    public int Minutes
    {
        get => _minutes;
        set
        {
            if (value >= 60)
            {
                Hours = value / 60;
                _minutes = value % 60;
            }
            else _minutes = value;
            NotifyPropertyChanged(nameof(Minutes));
        }
    }
    
    public int Hours
    {
        get => _hours;
        set
        {
            _hours = value;
            NotifyPropertyChanged(nameof(Hours));
        }
    }
    
    public int Repetitions
    {
        get => _repetitions;
        set
        {
            _repetitions = value;
            NotifyPropertyChanged(nameof(Repetitions));
            NotifyPropertyChanged(nameof(RepetitionText));
            NotifyPropertyChanged(nameof(IsRepetitionsDisabled));
        }
    }
    
    public bool IsRepetitionsDisabled => _repetitions == 0;

    public string RepetitionText => _repetitions == 0 ? "RUNNING UNTIL STOPPED" : "REPETITIONS";

    public int RepetitionsPercentageDone => _repetitions == 0 ? 0 : _repetitionsDone * 100 / _repetitions;

    // Property Changed Event Handler
    public event PropertyChangedEventHandler? PropertyChanged;

    internal void NotifyPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    #endregion
    
    #region Autoclicker
    private CancellationTokenSource _cancellationTokenClicker = new();
    
    public void StartClicker()
    {
        IsRunning = true;
        _cancellationTokenClicker = new CancellationTokenSource();
        
        var sleepTime = (Hours * 3600 + Minutes * 60 + Seconds) * 1000 + Milliseconds;
        
        Task.Run(() =>
        {
            if (_repetitions == 0)
            {
                while (!_cancellationTokenClicker.Token.IsCancellationRequested)
                {
                    PressMouseButton(SelectedMouseButton);
                    _repetitionsDone++;
                    NotifyPropertyChanged(nameof(RepetitionsPercentageDone));
                    Thread.Sleep(sleepTime);
                }
            }
            else
            {
                while (!_cancellationTokenClicker.Token.IsCancellationRequested && _repetitionsDone < _repetitions)
                {
                    PressMouseButton(SelectedMouseButton);
                    _repetitionsDone++;
                    NotifyPropertyChanged(nameof(RepetitionsPercentageDone));
                    Thread.Sleep(sleepTime);
                }
            }

            IsRunning = false;
            _repetitionsDone = 0;
        }, _cancellationTokenClicker.Token);
    }
    
    public void StopClicker()
    {
        _cancellationTokenClicker.Cancel();
        IsRunning = false;
        _repetitionsDone = 0;
    }
    #endregion
}