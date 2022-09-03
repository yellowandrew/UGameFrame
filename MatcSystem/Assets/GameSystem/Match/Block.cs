
using UnityEngine;
using System;
public class Block : MonoBehaviour
{
    public int type;
    public Vector2Int posInBoard;
    public bool isAnimation = false;
    public bool dieAfterAnim = false;
    public virtual void moveToX(float x) { }
    public virtual void moveToY(float y) { }
    public virtual void move(Vector2 vec) { }
    public virtual void touchDown() { }
    public virtual void touchUp() { }

    public virtual void setSprite(Sprite s1,Sprite s2) { }
}


