using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [HideInInspector]
    public Node node;
    [HideInInspector]
    public int ballCount;
    public TMP_Text BallCountText;


    public void Initialize(Node node)
    {
        this.node = node;
    }

    public void Check()
    {
        BallCountText.text = ballCount.ToString();
        if (ballCount > 0)
        {
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
                BallCountText.text = ballCount.ToString();
                if (ballCount == 0)
                {
                    node.type = "E";
                    C.gameManager.CheckGrid();
                    Destroy(this.gameObject);
                }
            }
        }
    }
}
