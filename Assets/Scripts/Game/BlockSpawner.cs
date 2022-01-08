using UnityEngine;
using System.Collections.Generic;
using Saro.Utility;

namespace Tetris
{
    /// <summary>
    /// 7-bag block random spawner
    /// </summary>
    public class BlockSpawner
    {
        private readonly List<Block> m_Queue;
        private int m_CurrentIndex;

        public BlockSpawner(Block[] blocks)
        {
            m_Queue = new List<Block>();

            for (int i = 0; i < blocks.Length; i++)
            {
                m_Queue.Add(GameObject.Instantiate(blocks[i]));
            }

            for (int i = 0; i < blocks.Length; i++)
            {
                m_Queue.Add(GameObject.Instantiate(blocks[i]));
            }

            RandomLeft();
            RandomRight();
        }

        public Block NextBlock()
        {
            if (m_CurrentIndex >= 7)
            {
                SwapLeftRight();
                RandomRight();
                m_CurrentIndex = 0;
            }


            var block = m_Queue[0];
            m_Queue.RemoveAt(0);
            m_Queue.Add(block);
            block.transform.position = new Vector3(Tetris.k_Width / 2, Tetris.k_Height);

            var ret = GameObject.Instantiate(block);

            ret.transform.localScale = Vector3.one;

            return ret;
        }

        private void RandomLeft()
        {
            RandomUtility.Shuffle(m_Queue, 0, 7);
        }

        private void RandomRight()
        {
            RandomUtility.Shuffle(m_Queue, 7, 7);
        }

        private void SwapLeftRight()
        {
            var halfLen = m_Queue.Count / 2;
            for (int i = 0; i < halfLen; i++)
            {
                RandomUtility.Swap(m_Queue, i, halfLen + i);
            }
        }

        #region Next Preview

        private float startPosX = 11.3f;
        private float startPosY = 16.5f;
        private float leftOffset = -.25f;
        private int distance = 2;
        private float sizeScale = .5f;

        public void UpdateNextChainSlot(int count = 5)
        {
            if (count > m_Queue.Count) throw new System.ArgumentOutOfRangeException();

            int i = 0;
            for (; i < count; i++)
            {
                var viewGO = m_Queue[i];
                viewGO.gameObject.SetActive(true);
                if (viewGO is BlockO || viewGO is BlockI)
                {
                    viewGO.transform.position = new Vector3(startPosX + leftOffset, startPosY - distance * i);
                }
                else
                {
                    viewGO.transform.position = new Vector3(startPosX, startPosY - distance * i);
                }
                viewGO.transform.localScale = new Vector3(sizeScale, sizeScale);
            }

            for (; i < m_Queue.Count; i++)
            {
                var viewGo = m_Queue[i];
                viewGo.gameObject.SetActive(false);
            }
        }

        #endregion
    }

}