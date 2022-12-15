using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundFader : MonoBehaviour
{
    void Start()
    {
        //renderer.color
        LeanTween.alpha(gameObject, 0f, 5f).setEaseInOutBounce().setLoopPingPong();
    }
}
