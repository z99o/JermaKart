using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class CoroutineUtils
{

    /**
     * Usage: StartCoroutine(CoroutineUtils.Chain(...))
     * For example:
     *     StartCoroutine(CoroutineUtils.Chain(
     *         CoroutineUtils.Do(() => Debug.Log("A")),
     *         CoroutineUtils.WaitForSeconds(2),
     *         CoroutineUtils.Do(() => Debug.Log("B"))));
     */
    public static IEnumerator Chain(params IEnumerator[] actions)
    {
        foreach (IEnumerator action in actions)
        {
            yield return Game_Manager._Instance.StartCoroutine(action);
        }
    }

    /**
     * Usage: StartCoroutine(CoroutineUtils.DelaySeconds(action, delay))
     * For example:
     *     StartCoroutine(CoroutineUtils.DelaySeconds(
     *         () => DebugUtils.Log("2 seconds past"),
     *         2);
     */
    public static IEnumerator DelaySeconds(Action action, float delay)
    {
        yield return new WaitForSeconds(delay);
        action();
    }

    public static IEnumerator DoOnSoundFinished(Action action, AudioSource source)
    {
        string clip = source?.clip?.name;
         yield return new WaitUntil(
            () => 
                !source.isPlaying 
            
            );
        action();
    }

    public static IEnumerator WaitForSeconds(float time)
    {
        yield return new WaitForSeconds(time);
    }

    public static IEnumerator Do(Action action)
    {
        action();
        yield return 0;
    }
}