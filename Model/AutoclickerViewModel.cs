using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using autoclicker.Util;
using static autoclicker.Util.MouseOperations;

namespace autoclicker.Model;

public class AutoclickerViewModel : INotifyPropertyChanged
{
    // Interval as clicks per unit of time
    private int _cpm;
    private int _cps;
    private int _hours;

    private bool _isRunning;

    // Interval 
    private int _milliseconds;
    private int _minutes;

    private int _repetitions;
    private int _repetitionsDone;
    private int _seconds;

    private AutoclickerViewModel()
    {
        SelectedMouseButton = MouseButton.Left;
        Milliseconds = 10;
    }

    public static AutoclickerViewModel Instance { get; } = new();

    #region Properties

    #region Running state

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

    #endregion

    public MouseButton SelectedMouseButton { get; set; }

    public int Milliseconds
    {
        get => _milliseconds;
        set
        {
            _milliseconds = value;
            NotifyPropertyChanged(nameof(Milliseconds));
            NotifyPropertyChanged(nameof(IntervalSafetyWarningVisibility));
        }
    }

    public int Seconds
    {
        get => _seconds;
        set
        {
            _seconds = value;
            NotifyPropertyChanged(nameof(Seconds));
            NotifyPropertyChanged(nameof(IntervalSafetyWarningVisibility));
        }
    }

    public int Minutes
    {
        get => _minutes;
        set
        {
            _minutes = value;
            NotifyPropertyChanged(nameof(Minutes));
            NotifyPropertyChanged(nameof(IntervalSafetyWarningVisibility));
        }
    }

    public int Hours
    {
        get => _hours;
        set
        {
            _hours = value;
            NotifyPropertyChanged(nameof(Hours));
            NotifyPropertyChanged(nameof(IntervalSafetyWarningVisibility));
        }
    }
    
    public Visibility IntervalSafetyWarningVisibility => Milliseconds < 2 && Seconds == 0 && Minutes == 0 && Hours == 0 && CpmInactive && CpsInactive ? Visibility.Visible : Visibility.Collapsed;

    public int Cpm
    {
        get => _cpm;
        set
        {
            _cpm = value;
            NotifyPropertyChanged(nameof(Cpm));
            NotifyPropertyChanged(nameof(CpmActive));
            NotifyPropertyChanged(nameof(CpmInactive));
            NotifyPropertyChanged(nameof(CpmText));
            NotifyPropertyChanged(nameof(IsIntervalEnabled));
            NotifyPropertyChanged(nameof(HighCpmWarningVisibility));
            NotifyPropertyChanged(nameof(IntervalSafetyWarningVisibility));
            
        }
    }

    public bool CpmActive => _cpm != 0;
    public bool CpmInactive => !CpmActive;
    public string CpmText => CpmActive ? "CPM (120000 MAX)" : "SET CPM";
    
    public Visibility HighCpmWarningVisibility => Cpm > 42000 ? Visibility.Visible : Visibility.Collapsed;

    public int Cps
    {
        get => _cps;
        set
        {
            _cps = value;
            NotifyPropertyChanged(nameof(Cps));
            NotifyPropertyChanged(nameof(CpsActive));
            NotifyPropertyChanged(nameof(CpsInactive));
            NotifyPropertyChanged(nameof(CpsText));
            NotifyPropertyChanged(nameof(IsIntervalEnabled));
            NotifyPropertyChanged(nameof(HighCpsWarningVisibility));
            NotifyPropertyChanged(nameof(IntervalSafetyWarningVisibility));
        }
    }

    public bool CpsActive => _cps != 0;
    public bool CpsInactive => !CpsActive;

    public string CpsText => CpsActive ? "CPS (2000 MAX)" : "SET CPS";
    
    public Visibility HighCpsWarningVisibility => Cps > 700 ? Visibility.Visible : Visibility.Collapsed;

    public bool IsIntervalEnabled => !(CpmActive || CpsActive);

    #region Repetitions

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

    public int RepetitionsDone
    {
        get => _repetitionsDone;
        set
        {
            _repetitionsDone = value;
            NotifyPropertyChanged(nameof(RepetitionsDone));
        }
    }

    public int RepetitionsPercentageDone => _repetitions == 0 ? 0 : _repetitionsDone * 100 / _repetitions;

    #endregion

    #region Property Changed

    public event PropertyChangedEventHandler? PropertyChanged;

    internal void NotifyPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #endregion

    #region Autoclicker

    private CancellationTokenSource _cancellationTokenClicker = new();

    public void StartClicker()
    {
        var sleepTime = new TimeSpan(0, Hours, Minutes, Seconds, Milliseconds);
        if (CpsActive) sleepTime = new TimeSpan(0, 0, 0, 0, 0, 1000000 / Cps);
        else if (CpmActive) sleepTime = new TimeSpan(0, 0, 0, 0, 0, 60 * 1000000 / Cpm);
        
        // Safeguard to not allow 0 Milliseconds 
        if (sleepTime == TimeSpan.Zero) sleepTime = new TimeSpan(0, 0, 0, 0, 500);

        IsRunning = true;
        _cancellationTokenClicker = new CancellationTokenSource();

        Task.Run(() =>
        {
            if (_repetitions == 0)
                while (!_cancellationTokenClicker.Token.IsCancellationRequested)
                {
                    var clickStartTime = DateTime.UtcNow;

                    PressMouseButton(SelectedMouseButton);
                    Interlocked.Increment(ref _repetitionsDone);
                    NotifyPropertyChanged(nameof(RepetitionsPercentageDone));

                    var elapsedTime = DateTime.UtcNow - clickStartTime;

                    var remainingTime = sleepTime - elapsedTime;

                    
                    if (remainingTime > TimeSpan.Zero)
                        Thread.Sleep(remainingTime);
                }
            else
                while (!_cancellationTokenClicker.Token.IsCancellationRequested && RepetitionsDone < _repetitions)
                {
                    var clickStartTime = DateTime.UtcNow;


                    PressMouseButton(SelectedMouseButton);
                    Interlocked.Increment(ref _repetitionsDone);
                    NotifyPropertyChanged(nameof(RepetitionsPercentageDone));

                    var elapsedTime = DateTime.UtcNow - clickStartTime;

                    var remainingTime = sleepTime - elapsedTime;

                    if (remainingTime > TimeSpan.Zero)
                        Thread.Sleep(remainingTime);
                }

            IsRunning = false;
            RepetitionsDone = 0;
        }, _cancellationTokenClicker.Token);
    }

    public void StopClicker()
    {
        _cancellationTokenClicker.Cancel();
        IsRunning = false;
        RepetitionsDone = 0;
    }

    #endregion
}