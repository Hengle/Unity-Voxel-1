﻿using UnityEngine;
using System.Collections;
using System;

public class SimpleMeshBuilder : ChunkMeshBuilder
{
	/// <summary>
	/// Build one or more mesh from a chunk
	/// </summary>
	/// <param name="chunk">chunk containing the blocks to render</param>
	/// <returns></returns>
	public override MeshData[] BuildMeshes(Chunk chunk)
	{
		MeshBuilder meshBuilder = new MeshBuilder();
		
		for (int x = 0; x < chunk.SizeX; x++)
			for (int y = 0; y < chunk.SizeY; y++)
				for (int z = 0; z < chunk.SizeZ; z++)
				{
					Block block = chunk.GetBlock(x, y, z);
					if (block.Type != Block.BlockTypes.Air)
					{
						GridPosition blockPosition = new GridPosition(x, y, z);
                        ProcessFace(block, blockPosition, Direction.Up,			chunk, meshBuilder);
						ProcessFace(block, blockPosition, Direction.Down,		chunk, meshBuilder);
						ProcessFace(block, blockPosition, Direction.Right,		chunk, meshBuilder);
						ProcessFace(block, blockPosition, Direction.Left,		chunk, meshBuilder);
						ProcessFace(block, blockPosition, Direction.Forward,	chunk, meshBuilder);
						ProcessFace(block, blockPosition, Direction.Backward,	chunk, meshBuilder);
					}
				}

		return meshBuilder.BuildMesh();
	}

	private	void ProcessFace(Block block, GridPosition blockPosition, Direction direction, Chunk chunk, MeshBuilder meshBuilder)
	{
		GridPosition otherBlockPosition = blockPosition + direction.ToPositionOffset();
		Block otherBlock = chunk.GetBlock(otherBlockPosition.x, otherBlockPosition.y, otherBlockPosition.z);

		if (block.IsFaceVisible(direction.Opposite(), otherBlock))
			BuildFace(blockPosition, block, direction, chunk, meshBuilder, chunk.BlockScale);
	}

	private void BuildFace(GridPosition blockPosition, Block block, Direction direction, Chunk chunk, MeshBuilder meshBuilder, Vector3 blockScale)
	{
		Vector3[] vertices = new Vector3[4];

		Vector3 finalOrigin = Vector3.Scale(blockPosition, blockScale) - chunk.ChunkOriginPoint - chunk.BlockOriginPoint;
		float px = finalOrigin.x;
		float py = finalOrigin.y;
		float pz = finalOrigin.z;


		/* Vertex list :
					4      8
					+------+
				  .'|    .'|		y
			  3	+---+--+' 7|		|
				|   |  |   |		|   .z
				| 2 +--+---+ 6		| .'
				| .'   | .'			+------x
			  1	+------+' 5
		*/

		switch (direction)
		{
			case Direction.Up:
				vertices[0] = new Vector3(px,					py + blockScale.y,	pz);				// 3
				vertices[1] = new Vector3(px,					py + blockScale.y,	pz + blockScale.z);	// 4
				vertices[2] = new Vector3(px + blockScale.x,	py + blockScale.y,	pz + blockScale.z);	// 8
				vertices[3] = new Vector3(px + blockScale.x,	py + blockScale.y,	pz);				// 7
				break;
			case Direction.Down:
				vertices[0] = new Vector3(px,					py,					pz + blockScale.z);	// 2
				vertices[1] = new Vector3(px,					py,					pz);				// 1
				vertices[2] = new Vector3(px + blockScale.x,	py,					pz);				// 5
				vertices[3] = new Vector3(px + blockScale.x,	py,					pz + blockScale.z);	// 6
				break;
			case Direction.Right:
				vertices[0] = new Vector3(px + blockScale.x,	py + blockScale.y,	pz);				// 7
				vertices[1] = new Vector3(px + blockScale.x,	py + blockScale.y,	pz + blockScale.z);	// 8
				vertices[2] = new Vector3(px + blockScale.x,	py,					pz + blockScale.z);	// 6
				vertices[3] = new Vector3(px + blockScale.x,	py,					pz);				// 5
				break;
			case Direction.Left:
				vertices[0] = new Vector3(px,					py + blockScale.y,	pz + blockScale.z);	// 4
				vertices[1] = new Vector3(px,					py + blockScale.y,	pz);				// 3
				vertices[2] = new Vector3(px,					py,					pz);				// 1
				vertices[3] = new Vector3(px,					py,					pz + blockScale.z);	// 2
				break;
			case Direction.Forward:
				vertices[0] = new Vector3(px + blockScale.x,	py + blockScale.y,	pz + blockScale.z);	// 8
				vertices[1] = new Vector3(px,					py + blockScale.y,	pz + blockScale.z);	// 4
				vertices[2] = new Vector3(px,					py,					pz + blockScale.z);	// 2
				vertices[3] = new Vector3(px + blockScale.x,	py,					pz + blockScale.z);	// 6
				break;
			case Direction.Backward:
				vertices[0] = new Vector3(px,					py + blockScale.y,	pz);				// 3
				vertices[1] = new Vector3(px + blockScale.x,	py + blockScale.y,	pz);				// 7
				vertices[2] = new Vector3(px + blockScale.x,	py,					pz);				// 5
				vertices[3] = new Vector3(px,					py,					pz);				// 1
				break;
		}

		meshBuilder.AddQuad(vertices, block.Color, block.GetSubMesh(), direction.ToUnitVector());
	}
}
