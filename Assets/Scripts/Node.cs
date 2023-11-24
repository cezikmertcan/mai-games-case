using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node : MonoBehaviour
{
    [HideInInspector]
    public int x;
    [HideInInspector]
    public int y;
    [HideInInspector]
    public string type = string.Empty;


    public List<Node> GetNeighbors()
    {
        List<Node> neighbors = new List<Node>();
        if (x + 1 <= C.gameManager.gridX)
            neighbors.Add(C.gameManager.GetNodeByPosition(x + 1, y));
        if (x - 1 >= 1)
            neighbors.Add(C.gameManager.GetNodeByPosition(x - 1, y));
        if (y + 1 <= C.gameManager.gridY)
            neighbors.Add(C.gameManager.GetNodeByPosition(x, y + 1));
        if (y - 1 >= 1)
            neighbors.Add(C.gameManager.GetNodeByPosition(x, y - 1));

        return neighbors;
    }

    public void Initialize(int x = -1, int y = -1, string type = "")
    {
        this.x = x;
        this.y = y;
        this.type = type;


        int portalBallCount;
        bool isNumeric = int.TryParse(type, out portalBallCount);
        if (isNumeric)
        {
            GameObject goPortal = Instantiate(C.gameManager.prefabPortal);
            goPortal.name = "Portal " + portalBallCount;
            goPortal.transform.parent = C.gameManager.trGrid;
            Portal portal = goPortal.GetComponent<Portal>();
            portal.node = this;
            portal.ballCount = portalBallCount;
            C.gameManager.portals.Add(portal);
            goPortal.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            this.type = "P";
        }
        else if (type == "O")
        {
            GameObject goBlock = Instantiate(C.gameManager.prefabBlock);
            goBlock.name = "Block";
            goBlock.transform.parent = C.gameManager.trGrid;
            goBlock.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
        else if (type == "B")
        {
            GameObject goBall = Instantiate(C.gameManager.prefabBall);
            goBall.name = "Ball";
            goBall.transform.parent = C.gameManager.trGrid;
            Ball ball = goBall.GetComponent<Ball>();
            ball.node = this;
            C.gameManager.balls.Add(ball);
            goBall.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    public Node GetBottomNeighbor()
    {
        return C.gameManager.GetNodeByPosition(x, y + 1);
    }

    public List<Node> BFS(List<Node> _previousNodes = null)
    {
        List<Node> queue = new List<Node>();
        if (this.type == "F")
        {
            //Debug.Log(string.Format("Completed from {0} to {1}", previousNode, this));
            queue.Add(this);
            return queue;
        };
        List<Node> neighbors;
        List<Node> previousNodes;
        if (_previousNodes != null)
        {
            previousNodes = new List<Node>(_previousNodes) { this };
            neighbors = GetNeighbors();
            //neighbors = neighbors.Where(n => n.type == "E" || n.type == "F").Where(n => !(n.x == previousNode.x && n.y == previousNode.y)).ToList();
            neighbors = neighbors.Where(n => n.type == "E" || n.type == "F").ToList();
            neighbors = neighbors.Where(n => !previousNodes.Any(pn => pn.ID() == n.ID())).ToList();
            //Debug.Log(string.Format("{0} to {1}", previousNode, this));
        }
        else
        {
            previousNodes = new List<Node> { this };
            neighbors = GetNeighbors();
            neighbors = neighbors.Where(n => n.type == "E" || n.type == "F").ToList();

        }
        if (neighbors.Count < 1)
            return new List<Node>();

        List<List<Node>> allNeighborBFSs = new List<List<Node>>();
        foreach (Node n in neighbors)
        {
            var nextBFS = n.BFS(previousNodes);
            if (nextBFS.Count > 0)
                allNeighborBFSs.Add(nextBFS);
        }
        if (allNeighborBFSs.Count == 0)
            return new List<Node>();
        var shortestLength = allNeighborBFSs.Min(p => p.Count);
        var shortest = allNeighborBFSs.First(p => p.Count == shortestLength);
        queue.Add(this);
        queue.AddRange(shortest);
        return queue;


    }

    public bool isValid()
    {
        return type != "O" && type != "C";
    }
    public override string ToString()
    {
        return x + "x" + y;/* + " | " + Type;*/
    }

    public string ID()
    {
        return x + "x" + y;
    }
}
