using OpenTK.Graphics.OpenGL4;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

public sealed class ComputeProgram : IDisposable
{
    public int Handle { get; }

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
        return GL.GetUniformLocation(Handle, name);
    }

    public void Dispose()
    {
        GL.DeleteProgram(Handle);
    }

    private static string NumberedSource(string source)
    {
        string[] lines = source.Split('\n');
        return string.Join("\n", lines.Select((line, i) => $"{i + 1,4}: {line}"));
    }
}
