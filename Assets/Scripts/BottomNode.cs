using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BottomNode : MonoBehaviour
{
    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;

    public Ball ball;
    public void Initialize(int x = -1, int y = -1, string type = "")
    {
        this.x = x;
        this.y = y;
       
    }
}
