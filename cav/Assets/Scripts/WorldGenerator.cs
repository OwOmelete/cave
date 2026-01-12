using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private CaveMeshBuilder cave;
    [SerializeField] private int valueForBloc;

    public VoxelComputeRunner VoxelCompute;
    public Chunks chunkPrefab;
    public int iterations = 4;

    private int[,,] worldGrid;

    private void Start()
    {
        valueForBloc = cave.valueForBloc;
        VoxelCompute.Generate();
        StartCoroutine(GenerateChunks());
    }

    private void GenerateWorldGrid()
    {
        worldGrid = new int[cave.size, cave.size, cave.size];
        
        for (int x = 0; x < cave.size; x++)
        for (int y = 0; y < cave.size; y++)
        for (int z = 0; z < cave.size; z++)
        {
            worldGrid[x,y,z] = Random.Range(0, cave.maxValue);
        }
    }

    IEnumerator GenerateChunks()
    {
        int chunkSize = chunkPrefab.size;
        int chunksPerAxis = VoxelCompute.size / chunkSize;
        
        for (int x = 0; x < chunksPerAxis; x++)
        for (int y = 0; y < chunksPerAxis; y++)
        for (int z = 0; z < chunksPerAxis; z++)
        {
            Chunks chunks = Instantiate(
                chunkPrefab,
                new Vector3(x, y, z) * chunkPrefab.size,
                Quaternion.identity,
                transform);
            
            chunks.valueForBloc = valueForBloc;
            chunks.Init(VoxelCompute, new Vector3Int(x,y,z), chunkSize);
            
            yield return null;
        }
    }
}
