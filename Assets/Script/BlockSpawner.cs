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
        m_next.RemoveFirst();
        block.transform.position = new Vector3(5, 21);
        return block;
    }

    private void RandomGenerator()
    {
        Shuffle();
        for (int i = 0; i < m_bag.Length; i++)
        {
            m_next.AddLast(m_bag[i]);
        }
    }

    #region Next Preview
    private int distance = 4;
    public void InitNextChainSlot(int count = 5)
    {
        LinkedListNode<Block> head = m_next.First;
        for (int i = 0; i < count; i++)
        {
            var viewGO = GameObject.Instantiate(head.Value);
            viewGO.transform.position = new Vector3(13, 18 - distance * i);
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
        viewGO.transform.position = new Vector3(13, 18 - distance * (count - 1));
        m_next_view.AddLast(viewGO);
    }
    #endregion

    private void Shuffle()
    {
        for (int i = 0; i < m_bag.Length - 1; i++)
        {
            Swap(ref m_bag[i],ref m_bag[RandomInRange(i, m_bag.Length)]);
        }
    }

    System.Random rnd = new System.Random();
    private int RandomInRange(int min, int max)
    {
        return rnd.Next(min, max);
    }

    private void Swap(ref Block a,ref Block b)
    {
        var tmp = a;
        a = b;
        b = tmp;
    }

}
