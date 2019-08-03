//using UnityEngine;
//using System.Collections;

//public class View : MonoBehaviour
//{
//    private Tetris m_tetris;
//    void Start()
//    {
//        m_tetris = new Tetris();

//        m_tetris.NextBlock();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (Input.GetKeyDown(KeyCode.LeftArrow))
//        {
//            m_tetris.MoveLeft();
//        }
//        else if (Input.GetKeyDown(KeyCode.RightArrow))
//        {
//            m_tetris.MoveRight();
//        }
//        else if (Input.GetKeyDown(KeyCode.Z))
//        {
//            m_tetris.ClockwiseRotate();
//        }
//        else if (Input.GetKeyDown(KeyCode.X))
//        {
//            m_tetris.AntiClockwiseRotate();
//        }
//        else if (Input.GetKeyDown(KeyCode.Space))
//        {
//            m_tetris.HardDown();
//        }
//        else if (Input.GetKeyDown(KeyCode.DownArrow))
//        {
//            m_tetris.SoftDown();
//        }

//        m_tetris.Fall(Time.deltaTime);
//    }

//    private void OnDrawGizmos()
//    {
//        if (m_tetris != null)
//        {
//            m_tetris.DrawGizmos();
//        }
//    }
//}
