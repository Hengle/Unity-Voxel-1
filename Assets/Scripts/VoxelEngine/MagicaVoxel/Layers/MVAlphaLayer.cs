﻿using UnityEngine;
using System.Collections;
using System;

public class MVAlphaLayer : MVLayer
{
	public MVAlphaLayer(string filePath)
		:base(filePath)
	{

	}

	public override void ReadPalette(MVChunk chunk, int index, float r, float g, float b, float a)
	{
	}

	public override void ReadSize(MVChunk chunk, int sizeX, int sizeY, int sizeZ)
	{
	}

	public override void ReadVolxel(MVChunk chunk, int x, int y, int z, byte index)
	{
		if (chunk.Blocks[x, y, z] == null)
			chunk.Blocks[x, y, z] = new MVBlock(chunk);

		((MVBlock)chunk.Blocks[x, y, z]).Alpha = index;
	}

	public override void InitPalette(MVChunk chunk)
	{
	}

	public override void InitDefaultPalette(MVChunk chunk)
	{
	}

	public override void ReadVersion(MVChunk chunk, byte[] version)
	{
	}
}