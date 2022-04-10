using System.Collections.Generic;

namespace Tetris
{
    // TODO component 如何使用数组类型，并且api易用？
    public struct LineClearDelayRequest
    {
        public List<int> lineToClear;

        public override string ToString()
        {
            return $"{nameof(LineClearRequest)} {string.Join(",", lineToClear)}";
        }
    }
}
