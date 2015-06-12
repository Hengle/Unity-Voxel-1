﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(MeshCollider))]
public class Chunk : MonoBehaviour
{
    #region Fields
    public const int CHUNK_SIZE = 16;

    public World World;
    public Vector3i Position;
    public Block[, ,] Blocks;

    private bool UpdateNeeded = true;

    private MeshFilter Filter;
    private MeshCollider Collider;
    #endregion Fields

    public Chunk()
    {
        Blocks = new Block[CHUNK_SIZE, CHUNK_SIZE, CHUNK_SIZE];
    }

    #region MonoBehaviour
    // Use this for initialization
	void Start ()
    {
        Filter = gameObject.GetComponent<MeshFilter>();
        Collider = gameObject.GetComponent<MeshCollider>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (UpdateNeeded)
        {
            UpdateNeeded = false;
            UpdateChunk();
        }
	}
    #endregion MonoBehaviour

    #region Mesh management
    //Updates the chunk based on its contents
    private void UpdateChunk()
    {
        MeshData meshData = new MeshData();

        for (int x = 0; x < CHUNK_SIZE; x++)
            for (int y = 0; y < CHUNK_SIZE; y++)
                for (int z = 0; z < CHUNK_SIZE; z++)
                {
                    meshData = Blocks[x, y, z].BlockData(this, new Vector3i(x, y, z), meshData);
                }

        RenderMesh(meshData);
    }

    //Sends the calculated mesh information
    //to the mesh and collision components
    private void RenderMesh(MeshData meshData)
    {
        Filter.mesh.Clear();
        Filter.mesh.vertices = meshData.Vertices.ToArray();
        Filter.mesh.triangles = meshData.Triangles.ToArray();
        Filter.mesh.colors32 = meshData.Colors.ToArray();
        Filter.mesh.RecalculateNormals();

        Collider.sharedMesh = null;
        Mesh mesh = new Mesh();
        mesh.vertices = meshData.ColVertices.ToArray();
        mesh.triangles = meshData.ColTriangles.ToArray();
        mesh.RecalculateNormals();

        Collider.sharedMesh = mesh;
    }
    #endregion Mesh management

    #region Block Management
    public void SetBlock(Vector3i blockPosition, Block block)
    {
        if (InRange(blockPosition.x) && InRange(blockPosition.y) && InRange(blockPosition.z))
        {
            Blocks[blockPosition.x, blockPosition.y, blockPosition.z] = block;
            UpdateNeeded = true;
        }
        else
        {
            World.SetBlock(Position + blockPosition, block);
        }
    }

    public Block GetBlock(Vector3i blockPosition)
    {
        if (InRange(blockPosition.x) && InRange(blockPosition.y) && InRange(blockPosition.z))
            return Blocks[blockPosition.x, blockPosition.y, blockPosition.z];

        Block ret = World.GetBlock(Position + blockPosition);

        return ret;
    }
    #endregion Block Management

    public static bool InRange(int index)
    {
        if (index < 0 || index >= CHUNK_SIZE)
            return false;

        return true;
    }

    public override string ToString()
    {
        return string.Format("Chunk({0},{1},{2})", Position.x, Position.y, Position.z);
    }

    public string FileName()
    {
        return string.Format("{0},{1},{2}.chunk", Position.x, Position.y, Position.z);
    }

    public void Invalidate()
    {
        UpdateNeeded = true;
    }

    public void SetBlocksUnmodified()
    {
        foreach (Block block in Blocks)
            block.changed = false;
    }
}