namespace Pawelsberg.GeneticNeuralNetworkConsole.Model;

/// <summary>
/// Process-wide Ctrl+C handler shared by long-running commands (run, gpurun). The handler
/// suppresses the default app-termination behaviour (e.Cancel = true) and flips a flag the
/// command's polling loop can observe. Each long-running command must Reset() at start so
/// stale presses from earlier commands don't trigger an immediate exit.
/// </summary>
public static class Cancellation
{
    private static volatile bool _requested;
    private static bool _hooked;
    private static readonly object _hookLock = new object();

    public static void EnsureHooked()
    {
        lock (_hookLock)
        {
            if (_hooked) return;
            _hooked = true;
            Console.CancelKeyPress += (sender, e) =>
            {
                e.Cancel = true;
                _requested = true;
                Console.WriteLine();
                Console.WriteLine("(Ctrl+C: cancellation requested — finishing current step)");
            };
        }
    }

    public static void Reset()
    {
        EnsureHooked();
        _requested = false;
    }

    /// <summary>Programmatic equivalent of pressing Ctrl+C — used by --gpucanceltest.</summary>
    public static void Request()
    {
        _requested = true;
    }

    public static bool Requested => _requested;
}
