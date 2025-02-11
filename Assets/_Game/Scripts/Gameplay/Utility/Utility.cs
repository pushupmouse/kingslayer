using System.Collections.Generic;
using MEC;
using UnityEngine;

public class Utility : MonoBehaviour
{
    public static IEnumerator<float> EmulateUpdate(System.Action func, MonoBehaviour scr)
    {
        yield return Timing.WaitForOneFrame;
        while (scr.gameObject != null)
        {
            if (scr.gameObject.activeInHierarchy && scr.enabled)
                func();
 
            yield return Timing.WaitForOneFrame;
        }
    }
}
