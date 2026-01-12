using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshBuilder))]
public class Chunks : MonoBehaviour
{
    public int size = 16;
    public int padding = 1;
    public int[,,] grid;
    public Vector3Int chunkCoord;

    private MeshBuilder builder;
    public int valueForBloc;

    private void Awake()
    {
        builder = GetComponent<MeshBuilder>();
    }

    public void Init(VoxelComputeRunner world, Vector3Int coord, int size)
    {
        int[,,] localGrid = new int[size, size, size];
        this.size = size;
        chunkCoord = coord;

        int paddedSize = size + 2 * padding;
        grid = new int[paddedSize, paddedSize, paddedSize];
        
        for (int x = 0; x < paddedSize; x++)
        for (int y = 0; y < paddedSize; y++)
        for (int z = 0; z < paddedSize; z++)
        {
            int wx = coord.x * size + x - padding;
            int wy = coord.y * size + y - padding;
            int wz = coord.z * size + z - padding;
            
            grid[x, y, z] = world.Get(wx, wy, wz);
        }

        builder.valueForBloc = world.valueForBloc;
        builder.BuildMesh(grid);
    }
    
    public void ApplyAutomata(int iterations)
    {
        int paddedSize = size + 2 * padding;

        for (int i = 0; i < iterations; i++)
        {
            int[,,] newGrid = new int[paddedSize, paddedSize, paddedSize];

            for (int x = 0; x < paddedSize; x++)
            for (int y = 0; y < paddedSize; y++)
            for (int z = 0; z < paddedSize; z++)
            {
                int sum = 0;
                int count = 0;

                foreach (var n in GetNeighbors(x, y, z))
                {
                    int value = grid[n.Item1, n.Item2, n.Item3];
                    if (value == -1)
                    {
                        
                    }
                    else
                    {
                        sum += value;
                        count++;
                    }
                }
                newGrid[x, y, z] = (count > 0) ? sum / count : grid[x, y, z];
            }
            grid = newGrid;
        }
    }
    
    private List<Tuple<int,int,int>> GetNeighbors(int x, int y, int z)
    {
        List<Tuple<int,int,int>> l = new List<Tuple<int,int,int>>();
        int paddedSize = size + 2 * padding;

        for (int i=-1; i<=1; i++)
        for (int j=-1; j<=1; j++)
        for (int k=-1; k<=1; k++)
        {
            if (i==0 && j==0 && k==0) continue;
            int nx = x + i;
            int ny = y + j;
            int nz = z + k;

            if (nx>=0 && ny>=0 && nz>=0 &&
                nx<paddedSize && ny<paddedSize && nz<paddedSize)
            {
                l.Add(new Tuple<int,int,int>(nx, ny, nz));
            }
        }

        return l;
    }
    
    public void BuildMesh()
    {
        int[,,] meshGrid = new int[size, size, size];
        for (int x = 0; x < size; x++)
        for (int y = 0; y < size; y++)
        for (int z = 0; z < size; z++)
        {
            meshGrid[x, y, z] = grid[x + padding, y + padding, z + padding];
        }

        builder.valueForBloc = valueForBloc;
        builder.BuildMesh(meshGrid);
    }
}
