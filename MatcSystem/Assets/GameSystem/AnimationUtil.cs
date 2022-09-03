using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationUtil : MonoBehaviour
{
    public static void MoveTo(GameObject obj, Vector3 NewPos, float duration) {
        MoveTo(obj, obj.transform.localPosition, NewPos, duration);
    }
    public static void MoveTo(GameObject obj, Vector3 startpos, Vector3 NewPos, float duration) {
        obj.SetActive(true);
        Animation anim = obj.GetComponent<Animation>() ? obj.GetComponent<Animation>() : obj.AddComponent<Animation>();
        anim.enabled = true;
        AnimationClip animclip = new AnimationClip();
        animclip.legacy = true;

        AnimationCurve curvex = AnimationCurve.Linear(0, startpos.x, duration, NewPos.x);
        AnimationCurve curvey = AnimationCurve.Linear(0, startpos.y, duration, NewPos.y);
        AnimationCurve curvez = AnimationCurve.Linear(0, startpos.z, duration, NewPos.z);
        AnimationCurve curvenable = AnimationCurve.Linear(0, 1, duration, 0);

        animclip.SetCurve("", typeof(Transform), "localPosition.x", curvex);
        animclip.SetCurve("", typeof(Transform), "localPosition.y", curvey);
        animclip.SetCurve("", typeof(Transform), "localPosition.z", curvez);
        animclip.SetCurve("", typeof(Animation), "m_Enabled", curvenable);

        anim.AddClip(animclip, "Moveto");
        anim.Play("Moveto");
        Destroy(animclip, duration);
    }
}
