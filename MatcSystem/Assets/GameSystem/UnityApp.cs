using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnityApp
{
    public MonoBehaviour monoCoroutine;

    public UnityApp()
    {
        
        GameObject obj = GameObject.Find("Canvas");
        CanvasScaler canvasScaler = obj.GetComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
       

    }

}
