//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using System;

//public class BlockPool : Singleton<BlockPool>
//{
//    public Block[] blocks;

//    private Dictionary<BlockType, Saro.Pool<Block>> m_blockLookup = new Dictionary<BlockType, Saro.Pool<Block>>();

//    void Start()
//    {
//        for (int i = 0; i < blocks.Length; i++)
//        {
//            var pool = new Saro.Pool<Block>(blocks[i], OnPooled, 10, false);
//            m_blockLookup.Add(blocks[i].blockType, pool);
//        }
//    }

//    public Block TryGetBlock(BlockType type)
//    {
//        Block res = null;
//        m_blockLookup.TryGetValue(type, out Saro.Pool<Block> pool);
//        if (pool != null)
//        {
//            res = pool.New();
//        }
//        return res;
//    }

//    public void Free(Block block)
//    {
//        if (block != null)
//        {
//            block.Pool.Free(block);
//        }
//    }

//    private void OnPooled(Block block)
//    {
//        block.gameObject.SetActive(false);
//    }
//}

