using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [HideInInspector]
    public Node node;
    [HideInInspector]
    public int ballCount;

    public void Initialize(Node node)
    {
        this.node = node;
    }

    public void Check()
    {
        if (ballCount > 0)
            if (this.node.GetBottomNeighbor().type == "E")
            {
                GameObject goBall = Instantiate(C.gameManager.prefabBall);
                goBall.name = "Ball";
                goBall.transform.parent = C.gameManager.trGrid;
                Ball ball = goBall.GetComponent<Ball>();
                ball.node = this.node.GetBottomNeighbor();
                ball.node.type = "B";
                ball.isClickable = true;
                ball.Initialize(C.gameManager.GetRandomBallColor());
                C.gameManager.balls.Add(ball);
                goBall.transform.position = new Vector3(this.node.GetBottomNeighbor().transform.position.x, 0, this.node.GetBottomNeighbor().transform.position.z);
                ballCount--;
                if (ballCount == 0)
                {
                    node.type = "E";
                    C.gameManager.CheckGrid();
                    Destroy(this.gameObject);
                }
            }
    }
}
