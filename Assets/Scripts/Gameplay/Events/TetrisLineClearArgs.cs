using Saro.Events;

namespace Tetris
{
    public sealed class TetrisLineClearArgs : GameEventArgs, IReference
    {
        public static readonly int EventID = typeof(TetrisLineClearArgs).GetHashCode();
        public bool isB2B;
        public bool isMini;

        // properties
        public bool isTSpin;
        public int line;
        public int ren;
        public override int ID => EventID;


        public override void IReferenceClear()
        {
            // clear
            isTSpin = false;
            isMini = false;
            line = 0;
            isB2B = false;
            ren = 0;
        }

        public static TetrisLineClearArgs Create()
        {
            var args = SharedPool.Rent<TetrisLineClearArgs>();
            // init
            return args;
        }
    }
}