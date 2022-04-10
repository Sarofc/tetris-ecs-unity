using Saro.Events;

namespace Tetris
{
    [XLua.LuaCallCSharp]
    public sealed class TetrisScoreEventArgs : GameEventArgs, IReference
    {
        public override int ID => s_EventID;
        public readonly static int s_EventID = typeof(TetrisScoreEventArgs).GetHashCode();

        public int line;
        public int score;
        public int level;

        // properties

        public static TetrisScoreEventArgs Create()
        {
            var args = SharedPool.Rent<TetrisScoreEventArgs>();
            // init
            return args;
        }


        public override void IReferenceClear()
        {
            // clear
            line = 0;
            score = 0;
            level = 0;
        }
    }
}