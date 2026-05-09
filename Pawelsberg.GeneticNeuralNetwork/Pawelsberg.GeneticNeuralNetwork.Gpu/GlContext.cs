using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

/// <summary>
/// Owns the GLFW window + OpenGL context. GLFW requires window creation and destruction
/// on the main thread, but glfwMakeContextCurrent is thread-safe — so for off-main GL
/// work the main thread constructs the context, calls ReleaseCurrent to unbind, and the
/// worker thread calls MakeCurrent to take ownership. NativeWindow's constructor auto-
/// binds the context to the calling thread; that's left in place for callers that build
/// + use it on the same thread (e.g. GpuRunCommand on the synchronous --gpurun path).
/// </summary>
public sealed class GlContext : IDisposable
{
    private readonly NativeWindow _window;

    public GlContext()
    {
        GpuSelector.ForceDedicatedGpu();

        NativeWindowSettings settings = NativeWindowSettings.Default;
        settings.ClientSize = new Vector2i(1, 1);
        settings.StartVisible = false;
        settings.StartFocused = false;
        settings.WindowState = WindowState.Minimized;
        settings.APIVersion = new Version(4, 3);
        settings.Profile = ContextProfile.Core;
        settings.Title = "GpuRunner";

        _window = new NativeWindow(settings);
        _window.MakeCurrent();
    }

    public void MakeCurrent() => _window.MakeCurrent();

    /// <summary>Detach the OpenGL context from the calling thread so another thread can
    /// MakeCurrent. Safe to call from any thread that currently owns the context.</summary>
    public unsafe void ReleaseCurrent()
    {
        GLFW.MakeContextCurrent(null);
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
