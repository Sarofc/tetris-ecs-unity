//using UnityEngine;

//public class Tetris
//{
//    public const int Height = 22;
//    public const int Width = 10;

//    public static int[,] grid;

//    private Square m_square;

//    public const float DeltaNormal = 1f;
//    public const float DeltaSoft = .1f;

//    public int score = 0;

//    public int level = 1;


//    public float FallDeltaTime
//    {
//        get
//        {
//            switch (moveDelta)
//            {
//                case MoveDelda.Normal:
//                    return DeltaNormal / level;
//                case MoveDelda.SoftDown:
//                    return DeltaSoft / level;
//            }

//            return 0f;
//        }
//    }
//    private float lastTime;
//    private MoveDelda moveDelta = MoveDelda.Normal;

//    enum MoveDelda
//    {
//        Normal,
//        SoftDown,
//        HardDown
//    }


//    public Tetris()
//    {
//        grid = new int[Width, Height];
//        m_square = new Square();
//    }

//    public void NextBlock()
//    {
//       m_square.GenerateNext(BlockType.T);
//    }

//    public void Fall(float deltaTime)
//    {
//        if (moveDelta == MoveDelda.HardDown)
//        {
//            m_square.MoveDown();
//        }
//        else
//        {
//            if (FallDeltaTime > lastTime)
//            {
//                lastTime += deltaTime;
//            }
//            else
//            {
//                m_square.MoveDown();
//                lastTime = 0;
//            }
//        }

//        NextBlock();
//    }

//    public void MoveLeft()
//    {
//        m_square.MoveLeft();
//    }

//    public void MoveRight()
//    {
//        m_square.MoveRight();
//    }

//    public void ClockwiseRotate()
//    {
//        m_square.ClockwiseRotate();
//    }

//    public void AntiClockwiseRotate()
//    {
//        m_square.AntiClockwiseRotate();
//    }

//    public void SoftDown()
//    {
//        moveDelta = MoveDelda.SoftDown;
//    }

//    public void HardDown()
//    {
//        moveDelta = MoveDelda.HardDown;
//    }

//    public void DrawGizmos()
//    {
//        if (m_square != null)
//        {
//            Gizmos.color = Color.red;
//            foreach (var pos in m_square.GetCurrentShape())
//            {
//                Gizmos.DrawCube((Vector2)pos, new Vector2(.9f, .9f));
//            }
//        }
//    }
//}