using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

public sealed class GlContext : IDisposable
{
    private readonly NativeWindow _window;

    public GlContext()
    {
        NativeWindowSettings settings = NativeWindowSettings.Default;
        settings.ClientSize = new Vector2i(1, 1);
        settings.StartVisible = false;
        settings.WindowState = WindowState.Minimized;
        settings.APIVersion = new Version(4, 3);
        settings.Profile = ContextProfile.Core;
        settings.Title = "GpuRunner";

        _window = new NativeWindow(settings);
        _window.MakeCurrent();
    }

    public string GetDeviceInfo()
    {
        string vendor = GL.GetString(StringName.Vendor) ?? string.Empty;
        string renderer = GL.GetString(StringName.Renderer) ?? string.Empty;
        string version = GL.GetString(StringName.Version) ?? string.Empty;
        return $"{vendor} | {renderer} | GL {version}";
    }

    public void Dispose()
    {
        _window.Dispose();
    }
}
