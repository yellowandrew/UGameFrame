using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int _turn = 3;
    public int _maxturn = 3;

    public float _hp = 100;
    public float _maxhp = 100;

    public float _attackPoint = 10;

    public Action OnFinish;

    private bool overridePlaying = true;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
