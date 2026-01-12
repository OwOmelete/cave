using System;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class VoxelComputeRunner : MonoBehaviour
{
    public ComputeShader shader;
    public int size = 128;
    public int iterations = 4;
    public int valueForBloc = 22;

    public int[] WorldGrid { get; private set; }

    private int kernel;

    private ComputeBuffer inBuffer;
    private ComputeBuffer outBuffer;

    
    
    public void Generate()
    {
        int count = size * size * size;

        WorldGrid = new int[count];
        
        for (int i = 0; i < count; i++)
            WorldGrid[i] = Random.Range(0, 50);

        inBuffer = new ComputeBuffer(count, sizeof(int));
        outBuffer = new ComputeBuffer(count, sizeof(int));
        
        inBuffer.SetData(WorldGrid);

        int kernel = shader.FindKernel("CSMain");
        shader.SetInt("size", size);
        

        int groups = Mathf.CeilToInt(size / 8);

        for (int i = 0; i < iterations; i++)
        {
            shader.SetBuffer(kernel, "inputGrid", inBuffer);
            shader.SetBuffer(kernel, "outputGrid", outBuffer);
            shader.Dispatch(kernel, groups, groups, groups);
            
            (inBuffer, outBuffer) = (outBuffer, inBuffer);
        }
        
        
        inBuffer.GetData(WorldGrid);
        
        inBuffer.Release();
        outBuffer.Release();
    }

    public int Get(int x, int y, int z)
    {
        if (x < 0 || y < 0 || z < 0 || x >= size || y >= size || z >= size)
        {
            return 0;
        }
        
        return WorldGrid[x + y * size + z * size * size];
    }
}
