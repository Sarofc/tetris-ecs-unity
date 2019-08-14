using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 7-bag random spawner
/// </summary>
public class BlockSpawner
{
    private Block[] m_bag;
    private LinkedList<Block> m_next = new LinkedList<Block>();
    private LinkedList<Block> m_next_view = new LinkedList<Block>();

    public BlockSpawner(Block[] blocks)
    {
        m_bag = blocks;
        // two bags
        RandomGenerator();
        RandomGenerator();
    }

    public Block NextBlock()
    {
        if (m_next.Count <= 7) RandomGenerator();

        var block = GameObject.Instantiate(m_next.First.Value);
        //var block = m_next.First.Value;
        m_next.RemoveFirst();
        block.transform.position = new Vector3(Tetris.Width / 2, Tetris.Height);
        return block;
    }

    private void RandomGenerator()
    {
        Shuffle();
        for (int i = 0; i < m_bag.Length; i++)
        {
            //var block = BlockPool.Instance.TryGetBlock(m_bag[i]);
            m_next.AddLast(m_bag[i]);
        }
    }

    #region Next Preview

    private float startPosX = 11.3f;
    private float startPosY = 16.5f;
    private float leftOffset = -.25f;
    private int distance = 2;
    private float sizeScale = .5f;

    public void InitNextChainSlot(int count = 5)
    {
        LinkedListNode<Block> head = m_next.First;
        for (int i = 0; i < count; i++)
        {
            var viewGO = GameObject.Instantiate(head.Value);
            if (viewGO is BlockO || viewGO is BlockI)
            {
                viewGO.transform.position = new Vector3(startPosX + leftOffset, startPosY - distance * i);
            }
            else
            {
                viewGO.transform.position = new Vector3(startPosX, startPosY - distance * i);
            }
            viewGO.transform.localScale = new Vector3(sizeScale, sizeScale);
            m_next_view.AddLast(viewGO);

            head = head.Next;
        }
    }
    public void UpdateNextChainSlot(int count = 5)
    {
        // view list - remove the first node
        GameObject.Destroy(m_next_view.First.Value.gameObject);
        m_next_view.RemoveFirst();

        // view list - head node
        LinkedListNode<Block> head2 = m_next_view.First;
        // next list - head node
        LinkedListNode<Block> head = m_next.First;

        while (head2 != null)
        {
            // move up the block
            head2.Value.SingleUp(distance);

            head = head.Next;
            head2 = head2.Next;
        }

        // add new block to end
        var viewGO = GameObject.Instantiate(head.Value);
        if (viewGO is BlockO || viewGO is BlockI)
        {
            viewGO.transform.position = new Vector3(startPosX + leftOffset, startPosY - distance * (count - 1));
        }
        else
        {
            viewGO.transform.position = new Vector3(startPosX, startPosY - distance * (count - 1));
        }

        viewGO.transform.localScale = new Vector3(sizeScale, sizeScale);
        m_next_view.AddLast(viewGO);
    }
    #endregion

    private void Shuffle()
    {
        for (int i = 0; i < m_bag.Length - 1; i++)
        {
            Swap(ref m_bag[i], ref m_bag[RandomInRange(i, m_bag.Length)]);
        }
    }

    System.Random rnd = new System.Random();
    private int RandomInRange(int min, int max)
    {
        return rnd.Next(min, max);
    }

    private void Swap<T>(ref T a, ref T b)
    {
        var tmp = a;
        a = b;
        b = tmp;
    }

}
