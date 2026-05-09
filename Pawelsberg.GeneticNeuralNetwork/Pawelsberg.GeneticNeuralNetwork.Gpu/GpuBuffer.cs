using OpenTK.Graphics.OpenGL4;

namespace Pawelsberg.GeneticNeuralNetwork.Gpu;

public sealed class GpuBuffer : IDisposable
{
    public int Handle { get; private set; }
    public int SizeBytes { get; }
    public string Name { get; }

    public GpuBuffer(string name, int sizeBytes, BufferUsageHint usage = BufferUsageHint.DynamicDraw)
    {
        Name = name;
        SizeBytes = sizeBytes;
        Handle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Handle);
        GL.BufferData(BufferTarget.ShaderStorageBuffer, sizeBytes, IntPtr.Zero, usage);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
    }

    public void Bind(int bindingIndex)
    {
        GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer, bindingIndex, Handle);
    }

    public void UploadInts(int[] data)
    {
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Handle);
        GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, data.Length * sizeof(int), data);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
    }

    public void UploadFloats(float[] data)
    {
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Handle);
        GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, data.Length * sizeof(float), data);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
    }

    public void UploadDoubles(double[] data)
    {
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Handle);
        GL.BufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, data.Length * sizeof(double), data);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
    }

    public int[] DownloadInts(int count)
    {
        int[] data = new int[count];
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Handle);
        GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, count * sizeof(int), data);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        return data;
    }

    public float[] DownloadFloats(int count)
    {
        float[] data = new float[count];
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Handle);
        GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, count * sizeof(float), data);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        return data;
    }

    public double[] DownloadDoubles(int count)
    {
        double[] data = new double[count];
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, Handle);
        GL.GetBufferSubData(BufferTarget.ShaderStorageBuffer, IntPtr.Zero, count * sizeof(double), data);
        GL.BindBuffer(BufferTarget.ShaderStorageBuffer, 0);
        return data;
    }

    public void Dispose()
    {
        if (Handle == 0) return;
        GL.DeleteBuffer(Handle);
        Handle = 0;
    }
}
