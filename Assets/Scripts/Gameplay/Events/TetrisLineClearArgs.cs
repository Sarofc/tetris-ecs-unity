using Saro.Events;

namespace Tetris
{
    public sealed class TetrisLineClearArgs : GameEventArgs, IReference
    {
        public override int ID => s_EventID;
        public readonly static int s_EventID = typeof(TetrisLineClearArgs).GetHashCode();

        // properties
        public bool isTSpin;
        public bool isMini;
        public int line;
        public bool isB2B;
        public int ren;

        public static TetrisLineClearArgs Create()
        {
            var args = SharedPool.Rent<TetrisLineClearArgs>();
            // init
            return args;
        }


        public override void IReferenceClear()
        {
            // clear
            isTSpin = false;
            isMini = false;
            line = 0;
            isB2B = false;
            ren = 0;
        }
    }
}