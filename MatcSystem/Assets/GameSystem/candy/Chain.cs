using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum ChainType {
    horizontal,
    vertical,
}

public class Chain
{
    ChainType type;
   public List<Jewel> jewels = new List<Jewel>();

    public Chain(ChainType type)
    {
        this.type = type;
    }
    public void Add(Jewel jewel) =>jewels.Add(jewel);
    public Jewel First() => jewels[0];
    public Jewel Last() => jewels[jewels.Count-1];
    public int Length() => jewels.Count;
}
