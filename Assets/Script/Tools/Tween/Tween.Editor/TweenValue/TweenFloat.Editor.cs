#if UNITY_EDITOR
using UnityEditor;

namespace Saro {
    public abstract partial class TweenFloat : TweenFormTo<float> {

        protected new abstract class Editor<T> : TweenFormTo<float>.Editor<T> where T : TweenFloat {
            protected override void OnPropertiesGUI (Tween tween) {
                EditorGUILayout.Space ();

                FromToFieldLayout ("Value", fromProp, toProp);
            }
        }
    }
}
#endif