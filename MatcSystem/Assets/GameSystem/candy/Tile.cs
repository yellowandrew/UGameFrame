using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile 
{
    public int id;
    public Tile(int id)  {
        this.id = id;
    }

    public bool IsEmpty => id == 0;
}
