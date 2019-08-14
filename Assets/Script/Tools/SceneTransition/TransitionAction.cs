using UnityEngine;
using System.Collections;

public interface ISceneTransition
{
    Shader GetShader();
    IEnumerator OnScreenObscured(SceneTransitionMgr transitionMgr);

}

