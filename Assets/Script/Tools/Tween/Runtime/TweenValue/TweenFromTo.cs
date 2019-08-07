using UnityEditor;
using UnityEngine;

namespace Saro {
    public abstract partial class TweenFormTo<T> : TweenAnimation where T : struct {
        public T from;
        public T to;

        public abstract T Current {
            get;
            set;
        }


    }
}