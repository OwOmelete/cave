using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class MeshBuilder : MonoBehaviour
{
    private Mesh mesh;
    private List<Vector3> vertices = new();
    private List<int> triangles = new();
    private List<Vector3> normals = new();
    public int valueForBloc;
    private int[,,] grid;
    
    private readonly Vector3[] cubeVerts =
    {
        new(0,0,0), new(1,0,0), new(1,1,0), new(0,1,0),
        new(0,1,1), new(1,1,1), new(1,0,1), new(0,0,1)
    };
    
    private readonly int[,] cubeFaces =
    {
        {0,3,2,1}, // Back
        {7,6,5,4}, // Front
        {3,4,5,2}, // Top
        {0,1,6,7}, // Bottom
        {1,2,5,6}, // Right
        {0,7,4,3}  // Left
    };

    private readonly Vector3[] faceNormals =
    {
        Vector3.back,
        Vector3.forward,
        Vector3.up,
        Vector3.down,
        Vector3.right,
        Vector3.left
    };

    private void Awake()
    {
        mesh = new Mesh();
        mesh.indexFormat = IndexFormat.UInt32;
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void BuildGreedyMesh(int[,,] grid)
    {
        this.grid = grid;
        vertices.Clear();
        triangles.Clear();
        normals.Clear();
        
        int nx = grid.GetLength(0);
        int ny = grid.GetLength(1);
        int nz = grid.GetLength(2);

        for (int z = 0; z < nz; z++)
        {
            bool[,] mask = new bool[nx, ny];
            for(int x = 0; x < nx; x++)
            for (int y = 0; y < ny; y++)
            {
                bool current = grid[x, y, z] > valueForBloc;
                bool behind = (z == nz - 1) ? false : grid[x, y, z + 1] > valueForBloc;
                mask[x, y] = current && !behind;
            }
            
            for (int y = 0; y < ny; y++)
            for (int x = 0; x < nx; x++)
            {
                if (!mask[x, y]) continue;

                int width = 1;
                while (x + width < nx && mask[x + width, y]) width++;

                int height = 1;
                bool done = false;
                while (y + height < ny && !done)
                {
                    for (int i = 0; i < width; i++)
                    {
                        if (!mask[x + i, y + height])
                        {
                            done = true;
                            break;
                        }
                    }

                    if (!done) height++;
                    
                    AddQuad(new Vector3(x,y,z), width, height, Vector3.forward);
                    
                    for (int i = 0; i < width; i++)
                    for (int j = 0; j < height; j++)
                        mask[x + i, y + j] = false;
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateBounds();
    }
    
    void AddQuad(Vector3 pos, int width, int height, Vector3 normal)
    {
        int vCount = vertices.Count;
        
        vertices.Add(pos);
        vertices.Add(pos + new Vector3(width, 0, 0));
        vertices.Add(pos + new Vector3(width, height, 0));
        vertices.Add(pos + new Vector3(0, height, 0));
        
        for (int i = 0; i < 4; i++) normals.Add(normal);
        
        triangles.Add(vCount + 0);
        triangles.Add(vCount + 1);
        triangles.Add(vCount + 2);

        triangles.Add(vCount + 0);
        triangles.Add(vCount + 2);
        triangles.Add(vCount + 3);
    }

    public void BuildMesh(int[,,] grid)
    {
        this.grid = grid;
        vertices.Clear();
        triangles.Clear();
        normals.Clear();
        
        
        for (int x = 0; x < grid.GetLength(0); x++)
        for (int y = 0; y < grid.GetLength(1); y++)
        for (int z = 0; z < grid.GetLength(2); z++)
        {
            if (grid[x, y, z] <= valueForBloc) continue;

            for (int face = 0; face < 6; face++)
            {
                Vector3Int dir = FaceDirection(face);
                int nx = x + dir.x;
                int ny = y + dir.y;
                int nz = z + dir.z;

                if (!isInside(nx, ny, nz) || grid[nx, ny, nz] <= valueForBloc)
                {
                    AddFace(face, new Vector3(x, y, z));
                }
            }
        }
        
        mesh.Clear();
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.RecalculateBounds();
    }

    void AddFace(int faceIndex, Vector3 pos)
    {
        int vCount = vertices.Count;

        for (int i = 0; i < 4; i++)
        {
            vertices.Add(cubeVerts[cubeFaces[faceIndex,i]] + pos);
            normals.Add(faceNormals[faceIndex]);
        }
        
        triangles.Add(vCount + 0);
        triangles.Add(vCount + 1);
        triangles.Add(vCount + 2);

        triangles.Add(vCount + 0);
        triangles.Add(vCount + 2);
        triangles.Add(vCount + 3);
    }

    Vector3Int FaceDirection(int face)
    {
        return face switch
        {
            0 => Vector3Int.back,
            1 => Vector3Int.forward,
            2 => Vector3Int.up,
            3 => Vector3Int.down,
            4 => Vector3Int.right,
            _ => Vector3Int.left,
        };
    }

    bool isInside(int x, int y, int z)
    {
        return x >= 0 && y >= 0&& z >= 0 &&
               x < grid.GetLength(0) &&
               y < grid.GetLength(1) &&
               z < grid.GetLength(2);
    }
    
}
