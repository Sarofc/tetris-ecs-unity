using System;
using System.Collections;
using UnityEngine;

public class SquaresTransition : ISceneTransition
{
    public Color squareColor = Color.black;
    public float duration = 1.0f;
    public float fadedDelay = 0f;
    public Vector2 squareSize = new Vector2(13f, 9f);
    public float smoothness = 0.5f;


    public Shader GetShader()
    {
        return Shader.Find("Hidden/SceneTransitions/Squares");
    }


    public IEnumerator OnScreenObscured(SceneTransitionMgr transitionMgr)
    {
        transitionMgr.Material.color = squareColor;
        transitionMgr.Material.SetFloat("_Smoothness", smoothness);
        transitionMgr.Material.SetVector("_Size", squareSize);

        yield return transitionMgr.StartCoroutine(transitionMgr.ProcessTransiton(duration));

        if(fadedDelay > 0)
        {
            yield return new WaitForSeconds(fadedDelay);
        }
        
        yield return transitionMgr.StartCoroutine(transitionMgr.WaitForLevelToLoad());


        yield return transitionMgr.StartCoroutine(transitionMgr.ProcessTransiton(duration,true));
    }
}

