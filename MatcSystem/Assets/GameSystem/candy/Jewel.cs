using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum JewelType { 
    ONE,
    TWO,
    THREE,
    FOUR,
    FIVE,
    SIX,
    COUNT,

    FOUR_V,
    FOUR_H,
    FIVE_S,
    CROSS,

    
}

public class Jewel 
{
    public JewelType type;
    public int column;
    public int row;
    public GameObject gameObject;
  

    public Jewel(int c, int r, JewelType t,GameObject gameObject,Transform p=null) {
        this.column = c;
        this.row = r;
        this.type = t;
        this.gameObject = gameObject;
        this.gameObject.transform.parent = p;
        gameObject.name = "("+column+","+row+")"+t;
        UpdatePosition();
    }
    public void UpdatePosition() {
        gameObject.transform.localPosition = new Vector2(column, row);
    }
}
