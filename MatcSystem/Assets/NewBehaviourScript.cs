using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public GameObject cube;

   public int[,] baord;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            AnimationUtil.MoveTo(cube, Vector3.one, 2);
        }
    }
}
