#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {
    public class BaseEditor<T> : Editor where T : Object {
        protected new T target { get { return base.target as T; } }
        public override void OnInspectorGUI () {
            // base.OnInspectorGUI ();
            DrawDefaultInspector ();
        }
    }
}
#endif