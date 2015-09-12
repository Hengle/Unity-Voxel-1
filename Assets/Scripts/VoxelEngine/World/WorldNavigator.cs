﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldNavigator : MonoBehaviour
{
	public World World;
	public static GridPosition[] ChunkLoadOrder;
	public static int RenderDistance = 20;
	public int MaxLoadPerFrame = 1;

	private int _LastI = 0;
	private bool _Done = false;
	private GridPosition _LastChunkPosition;

	private void Start ()
	{
		World.RegisterNavigator(this);
	}

	private void Update()
	{
		GridPosition currentChunkPosition = GetChunkPositionFromRealPosition();
        if (_Done == true && !currentChunkPosition.Equals(_LastChunkPosition))
		{
			_LastChunkPosition = currentChunkPosition;
			_Done = false;
		}

		if (!_Done)
		{
			LoadChunksInRange(currentChunkPosition);
		}
	}

	static WorldNavigator()
	{
		var chunkLoads = new List<GridPosition>();
		for (int x = -RenderDistance; x <= RenderDistance; x++)
		{
			for (int z = -RenderDistance; z <= RenderDistance; z++)
			{
				chunkLoads.Add(new GridPosition(x, 0, z));
			}
		}

		// limit how far away the blocks can be to achieve a circular loading pattern
		float maxRadius = RenderDistance * 1.55f;

		//sort 2d vectors by closeness to center
		ChunkLoadOrder = chunkLoads
							.Where(pos => Mathf.Abs(pos.x) + Mathf.Abs(pos.z) < maxRadius)
							.OrderBy(pos => Mathf.Abs(pos.x) + Mathf.Abs(pos.z)) //smallest magnitude vectors first
							.ThenBy(pos => Mathf.Abs(pos.x)) //make sure not to process e.g (-10,0) before (5,5)
							.ThenBy(pos => Mathf.Abs(pos.z))
							.ToArray();
	}

	private void LoadChunksInRange(GridPosition currentChunkPosition)
	{
		int cpt = 0;
		for (int i = _LastI; i < ChunkLoadOrder.Length; i++)
		{
			_LastI = i;
			if (++cpt > MaxLoadPerFrame)
			{
				return;
			}

			GridPosition newChunkPosition = currentChunkPosition + ChunkLoadOrder[i];
			newChunkPosition.y = 0;

			Chunk newChunk = World.GetChunk(newChunkPosition);

			if (newChunk != null && newChunk.ColumnLoaded)
				continue;

			World.LoadChunkColumn(newChunkPosition.x, newChunkPosition.z);
		}
		_LastI = 0;
		_Done = true;
	}

	private GridPosition GetChunkPositionFromRealPosition()
	{
		return new GridPosition(
			Mathf.FloorToInt((transform.position.x / World.BlockScale + World.BlockOrigin.x + World.ChunkOrigin.x) / (float)World.ChunkSizeX),
			Mathf.FloorToInt((transform.position.y / World.BlockScale + World.BlockOrigin.y + World.ChunkOrigin.y) / (float)World.ChunkSizeY),
			Mathf.FloorToInt((transform.position.z / World.BlockScale + World.BlockOrigin.z + World.ChunkOrigin.z) / (float)World.ChunkSizeZ)
			) + World.WorldOrigin;
	}
}
