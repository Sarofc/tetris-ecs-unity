#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Saro {
    public abstract partial class TweenFormTo<T> : TweenAnimation where T : struct {

        protected T m_Cache;

        public override void Record () {
            m_Cache = Current;
        }

        public override void Restore () {
            Current = m_Cache;
        }

        public override void Reset () {
            base.Reset ();
            from = Current;
            to = Current;
        }

        protected new abstract class Editor<U> : TweenAnimation.Editor<U> where U : TweenFormTo<T> {
            protected SerializedProperty fromProp;
            protected SerializedProperty toProp;

            protected override void OnEnable () {
                base.OnEnable ();

                fromProp = serializedObject.FindProperty ("from");
                toProp = serializedObject.FindProperty ("to");
            }

            protected override void InitOptionsMenu (GenericMenu menu, Tween tween) {
                base.InitOptionsMenu (menu, tween);

                menu.AddSeparator (string.Empty);

                menu.AddItem (new GUIContent ("Swap From and To"), false, () => {
                    Undo.RecordObject (target, "Swap From and To");
                    var tmp = target.from;
                    target.from = target.to;
                    target.to = tmp;
                });

                menu.AddItem (new GUIContent ("Set From to Current"), false, () => {
                    Undo.RecordObject (target, "Set From to Current");
                    target.from = target.Current;
                });

                menu.AddItem (new GUIContent ("Set To to Current"), false, () => {
                    Undo.RecordObject (target, "Set To to Current");
                    target.to = target.Current;
                });

                menu.AddSeparator (string.Empty);

                menu.AddItem (new GUIContent ("Set Current to From"), false, () => {
                    target.OnInterpolate (0);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (target.gameObject.scene);
                });

                menu.AddItem (new GUIContent ("Set Current to To"), false, () => {
                    target.OnInterpolate (1);
                    UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty (target.gameObject.scene);
                });
            }

        }
    }
}
#endif