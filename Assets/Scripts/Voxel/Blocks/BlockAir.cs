﻿using UnityEngine;
using System;

[Serializable]
public class BlockAir : Block
{
    public BlockAir(Chunk parent)
        : base(parent)
    {

    }

    public override MeshData BlockData
        (Chunk chunk, Vector3i position, MeshData meshData)
    {
        return meshData;
    }

    public override bool IsSolid(Direction direction)
    {
        return false;
    }
}
