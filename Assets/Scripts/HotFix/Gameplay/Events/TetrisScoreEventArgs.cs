using Saro.Events;

namespace Tetris
{
    public sealed class TetrisScoreEventArgs : GameEventArgs, IReference
    {
        public static readonly int EventID = typeof(TetrisScoreEventArgs).GetHashCode();
        public int level;

        public int line;
        public int score;
        public override int ID => EventID;


        public override void IReferenceClear()
        {
            // clear
            line = 0;
            score = 0;
            level = 0;
        }

        // properties

        public static TetrisScoreEventArgs Create()
        {
            var args = SharedPool.Rent<TetrisScoreEventArgs>();
            // init
            return args;
        }
    }
}