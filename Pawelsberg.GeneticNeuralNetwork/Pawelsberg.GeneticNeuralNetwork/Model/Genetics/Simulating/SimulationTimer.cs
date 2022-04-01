namespace Pawelsberg.GeneticNeuralNetwork.Model.Genetics.Simulating;

public class SimulationTimer
{
    // Timer related:
    private Timer _timer;
    public int DelayTimeMs { get; set; }
    private readonly object _callbackLock;
    public event EventHandler Ticked;
    public bool Running { get { return _timer != null; } }
    public SimulationTimer(int delayTimeMs)
    {
        DelayTimeMs = delayTimeMs;
        _callbackLock = new object();
    }
    public void Pause()
    {
        lock (_callbackLock)
        {
            _timer.Dispose();
            _timer = null;
        }
    }
    public void Start()
    {
        _timer = new Timer(_timer_Callback, null, DelayTimeMs, Timeout.Infinite);
    }
    private void _timer_Callback(object state)
    {
        lock (_callbackLock)
        {
            if (_timer == null)
                return;

            OnTicked();
            if (_timer != null)
                _timer.Change(DelayTimeMs, Timeout.Infinite);
        }
    }
    private void OnTicked()
    {
        if (Ticked != null)
            Ticked(this, new EventArgs());
    }
}
