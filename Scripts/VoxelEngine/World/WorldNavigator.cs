﻿using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WorldNavigator : MonoBehaviour
{
	public World World;
	public static GridPosition[] ChunkLoadOrder;
	public int RenderDistance = 16;
	public int MaxLoadPerFrame = 1;
	public GridPosition Position;

	private int _LastI = 0;
	private bool _Done = false;

	private GUIStyle _Style;
	private Rect _LabelPosition;

	private void Start ()
	{
		Profiler.maxNumberOfSamplesPerFrame = 8000000;
        World.RegisterNavigator(this);
		Position = GetChunkPositionFromRealPosition();

		_Style = new GUIStyle { normal = new GUIStyleState { textColor = Color.black } };
		_LabelPosition = new Rect(10f, 10f, 300f, 50f);
    }

	private void FixedUpdate()
	{
		if (World.MultiThreading)
			DoUpdate();
	}

	private void Update()
	{
		if (!World.MultiThreading)
			DoUpdate();
	}

	private void DoUpdate()
	{
		GridPosition currentChunkPosition = GetChunkPositionFromRealPosition();
        if (!currentChunkPosition.Equals(Position))
		{
			_LastI = 0;
			Position = currentChunkPosition;
			_Done = false;
		}

		if (!_Done)
		{
			LoadChunksInRange(Position);
		}
	}

	private void Awake()
	{
		var chunkLoads = new List<GridPosition>();
		for (int x = -RenderDistance; x <= RenderDistance; x++)
		{
			for (int z = -RenderDistance; z <= RenderDistance; z++)
			{
				chunkLoads.Add(new GridPosition(x, 0, z));
			}
		}

		//sort 2d vectors by closeness to center
		ChunkLoadOrder = chunkLoads
							.Where(pos => IsInRange(pos))
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

			GridPosition newChunkPosition = currentChunkPosition + ChunkLoadOrder[i];
			newChunkPosition.y = 0;

			Chunk newChunk = World.GetChunk(newChunkPosition);

			if (newChunk != null && newChunk.ColumnLoaded)
				continue;

			World.LoadChunkColumn(newChunkPosition);

			if (++cpt > MaxLoadPerFrame)
				return;
		}
		_LastI = 0;
		_Done = true;
	}

	private GridPosition GetChunkPositionFromRealPosition()
	{
		return new GridPosition(
			Mathf.FloorToInt(((transform.position.x + World.BlockOriginPoint.x + World.ChunkOriginPoint.x + World.WorldOriginPoint.x) / World.BlockScale.x) / (float)World.ChunkSizeX),
			Mathf.FloorToInt(((transform.position.y + World.BlockOriginPoint.y + World.ChunkOriginPoint.y + World.WorldOriginPoint.y) / World.BlockScale.y) / (float)World.ChunkSizeY),
			Mathf.FloorToInt(((transform.position.z + World.BlockOriginPoint.z + World.ChunkOriginPoint.z + World.WorldOriginPoint.z) / World.BlockScale.z) / (float)World.ChunkSizeZ)
			);
	}

	public bool IsInRange(GridPosition position)
	{
		return Mathf.Abs(position.x) + Mathf.Abs(position.z) < RenderDistance * 1.55f;
    }

	private void OnGUI()
	{
		
		GUI.Label(
			_LabelPosition,
			string.Format("Position({0:F2},{1:F2},{2:F2})\nChunk({3},{4},{5})",
				transform.position.x,
				transform.position.y,
				transform.position.z,
				Position.x,
				Position.y,
				Position.z),
			_Style);
	}
}
