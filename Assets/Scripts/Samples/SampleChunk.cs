﻿using UnityEngine;
using System.Collections;
using System;

public class SampleChunk : Chunk
{
	protected override Block GetExternalBlock(int x, int y, int z)
	{
		return null;
	}
}