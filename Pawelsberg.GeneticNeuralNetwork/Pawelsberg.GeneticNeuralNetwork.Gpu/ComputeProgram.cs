using OpenTK.Graphics.OpenGL4;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

public sealed class ComputeProgram : IDisposable
{
    public int Handle { get; private set; }

    // Cache uniform locations: glGetUniformLocation does a string lookup against the
    // program's introspection table. Hot loops (StepGeneration runs many times per second)
    // were calling it once per dispatch, which adds measurable driver-thread CPU.
    private readonly Dictionary<string, int> _uniformLocationCache = new Dictionary<string, int>();

    public ComputeProgram(string source, string debugName)
    {
        int shader = GL.CreateShader(ShaderType.ComputeShader);
        GL.ShaderSource(shader, source);
        GL.CompileShader(shader);
        GL.GetShader(shader, ShaderParameter.CompileStatus, out int compileStatus);
        if (compileStatus == 0)
        {
            string log = GL.GetShaderInfoLog(shader);
            GL.DeleteShader(shader);
            throw new Exception($"Compute shader '{debugName}' failed to compile: {log}\n--- SOURCE ---\n{NumberedSource(source)}");
        }

        Handle = GL.CreateProgram();
        GL.AttachShader(Handle, shader);
        GL.LinkProgram(Handle);
        GL.GetProgram(Handle, GetProgramParameterName.LinkStatus, out int linkStatus);
        GL.DetachShader(Handle, shader);
        GL.DeleteShader(shader);
        if (linkStatus == 0)
        {
            string log = GL.GetProgramInfoLog(Handle);
            GL.DeleteProgram(Handle);
            throw new Exception($"Compute shader '{debugName}' failed to link: {log}");
        }
    }

    public void Use()
    {
        GL.UseProgram(Handle);
    }

    public void Dispatch(int groupsX, int groupsY = 1, int groupsZ = 1)
    {
        GL.DispatchCompute(groupsX, groupsY, groupsZ);
    }

    public int GetUniformLocation(string name)
    {
        if (_uniformLocationCache.TryGetValue(name, out int cached))
            return cached;
        int location = GL.GetUniformLocation(Handle, name);
        _uniformLocationCache[name] = location;
        return location;
    }

    public void Dispose()
    {
        if (Handle == 0) return;
        GL.DeleteProgram(Handle);
        Handle = 0;
    }

    private static string NumberedSource(string source)
    {
        string[] lines = source.Split('\n');
        return string.Join("\n", lines.Select((line, i) => $"{i + 1,4}: {line}"));
    }
}
