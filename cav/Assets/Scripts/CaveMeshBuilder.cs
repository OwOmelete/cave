using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CaveMeshBuilder : MonoBehaviour
{
    public int[,,] grid;
    private List<GameObject> l = new() ;
    [SerializeField] private MeshBuilder _meshBuilder;
    [SerializeField] public int size;
    [SerializeField] private int overcrowed;
    [SerializeField] private int undercrowed;
    [SerializeField] private int iterations;
    [SerializeField] public int maxValue;
    [SerializeField] public int valueForBloc;
    [SerializeField] private GameObject bloc;
    [SerializeField] private bool toggle;

    public static CaveMeshBuilder INSTANCE;

    private Mesh mesh;

    private void Awake()
    {
        INSTANCE = this;
        mesh = new Mesh();
    }

    void Start()
    {
        GenerateGrid();
        
    }

    void Update()
    {
        
    }

    [ContextMenu("GenerateGrid")]
    void GenerateGrid()
    {
        grid = new int[size,size,size];
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(0); y++)
            {
                for (int z = 0; z < grid.GetLength(0); z++)
                {
                    grid[x, y, z] = Random.Range(0, maxValue);
                }
            }
        }
        ShowGrid();
    }

    void ShowGrid()
    {
        return;
        foreach (var v in l)
        {
            Destroy(v);
        }
        l.Clear();
        for (int x = 0; x < grid.GetLength(0); x++)
        {
            for (int y = 0; y < grid.GetLength(1); y++)
            {
                for (int z = 0; z < grid.GetLength(2); z++)
                {
                    if (grid[x, y, z] > valueForBloc)
                    {
                        
                        bool check = false;
                        var l2 = getNeighborsCross(x,y,z);
                        if (l2.Count < 6)
                        {
                            check = true;
                        }
                        else
                        {
                            foreach (var elt in l2)
                            {
                                if (grid[elt.Item1, elt.Item2, elt.Item3] <= valueForBloc)
                                {
                                    check = true;
                                    break;
                                }
                            }
                        }

                        if (check && !toggle)
                        {
                            l.Add(Instantiate(bloc, new Vector3(x,y,z), Quaternion.identity));
                        }
                        else if (toggle)
                        {
                            l.Add(Instantiate(bloc, new Vector3(x,y,z), Quaternion.identity));
                        }
                    }
                }
            }
        }
    }

    List<Tuple<int, int, int>> getNeighbors(int x, int y, int z)
    {
        List<Tuple<int, int, int>> l = new();

        for (int i = -1; i <= 1; i ++)
        {
            for (int j = -1; j <= 1; j ++)
            {
                for (int k = -1; k <= 1; k ++)
                {
                    if (isTileInGrid(x + i, y + j, z + k) && !(i == 0 && j == 0 && k == 0))
                    {
                        l.Add(new Tuple<int, int, int>(x + i, y + j, z + k));
                    }
                }
            }
        }

        return l;
    }

    List<Tuple<int, int, int>> getNeighborsCross(int x, int y, int z)
    {
        List<Tuple<int, int, int>> l = new();
        for (int i = -1; i <= 1; i ++)
        {
            for (int j = -1; j <= 1; j ++)
            {
                for (int k = -1; k <= 1; k ++)
                {
                    if (isTileInGrid(x + i, y + j, z + k) && !(i == 0 && j == 0 && k == 0))
                    {
                        if ((i == 0 && j == 0) || (i == 0 && k == 0) || (j == 0 && k == 0))
                        {
                            l.Add(new Tuple<int, int, int>(x + i, y + j, z + k));
                        }
                    }
                }
            }
        }

        return l;
    }
    
    bool isTileInGrid(int x, int y, int z)
    {
        if (y < 0 || y > grid.GetLength(0) - 1 || x < 0 || x > grid.GetLength(1) - 1 || z < 0 || z > grid.GetLength(2)-1)
        {
            return false;
        }
        return true;
    }

    
    [ContextMenu("ReloadCave")]
    public void ReloadCave()
    {
        for (int i = 0; i < iterations; i++)
        {
            int[,,] newGrid = new int[size,size,size];
            for (int x = 0; x < grid.GetLength(0); x++)
            {
                for (int y = 0; y < grid.GetLength(0); y++)
                {
                    for (int z = 0; z < grid.GetLength(0); z++)
                    {
                        List <Tuple<int, int, int>> l = getNeighbors(x, y, z);
                        int count = 0;
                        foreach (var neighbor in l)
                        {
                            count += grid[neighbor.Item1, neighbor.Item2, neighbor.Item3];
                        }
                        newGrid[x, y, z] = count/l.Count;
                    }
                }
            }

            grid = newGrid;
        }
        ShowGrid();
    }
    
}