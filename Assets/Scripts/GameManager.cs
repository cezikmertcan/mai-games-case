using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Node prefab_Node;
    public BottomNode prefab_BottomNode;

    public GameObject prefabBlock;
    public GameObject prefabBall;
    public GameObject prefabPortal;

    public Transform trGrid;
    public Transform trBottomGrid;
    public int BottomGridSize = 7;

    public TextAsset levelText; 
    
    [HideInInspector]
    public int gridX = -1;
    [HideInInspector]
    public int gridY = -1;
    [HideInInspector]
    public List<Node> nodes = new List<Node>();
    [HideInInspector]
    public List<BottomNode> bottomNodes = new List<BottomNode>();
    [HideInInspector]
    public List<Ball> balls = new List<Ball>();
    [HideInInspector]
    public List<Portal> portals = new List<Portal>();
    [HideInInspector]
    public List<string> LevelBallColors = new List<string>();


    public bool ISPLAYING = true;

    private void Awake()
    {
        C.gameManager = this;
    }

    private void Start()
    {
        CreateGrid();
        CreateBottomGrid();
        ReadyTheGrid();
    }
    private void CreateGrid()
    {
        var rows = levelText.text.Split('\n').ToList();
        gridY = rows.Count;

        for (int i = 0; i < gridY; i++)
        {
            var columns = rows[i].Split("-").ToList();
            if (i == 0) gridX = columns.Count;
            for (int j = 0; j < gridX; j++)
            {
                GameObject goNode = Instantiate(prefab_Node.gameObject);
                goNode.transform.position = new Vector3(j * 1, -0.5f, i * -1);
                goNode.transform.parent = trGrid;
                Node node = goNode.GetComponent<Node>();
                node.Initialize(j + 1, i + 1, columns[j].Substring(0, 1));
                goNode.name = (j + 1).ToString() + "x" + (i + 1).ToString();

                nodes.Add(node);
            }
        }
        trGrid.transform.Translate(-1 * ((gridX - 1) / 2f), 0, 1 * (gridY - 4f));
    }
    private void CreateBottomGrid()
    {
        for (int i = 0; i < BottomGridSize; i++)
        {
            GameObject goBottomNode = Instantiate(prefab_BottomNode.gameObject);
            goBottomNode.transform.parent = trBottomGrid;
            goBottomNode.transform.localPosition = new Vector3(i * 1, -0.5f, 0);

            BottomNode node = goBottomNode.GetComponent<BottomNode>();
            node.Initialize(i + 1, 10);
            goBottomNode.name = ((i + 1).ToString() + "x10").ToString();

            bottomNodes.Add(node);
        }
        trBottomGrid.transform.Translate(-1 * ((BottomGridSize - 7) / 2), 0, 0);

    }

    private void ReadyTheGrid()
    {

        int TotalBallCount = balls.Count + portals.Sum(p => p.ballCount);//THIS SHOULD BE ALWAYS N * 12 and N>=1
        for (int i = 0; i < TotalBallCount / 4; i++)
        {
            LevelBallColors.Add("Red");
            LevelBallColors.Add("Green");
            LevelBallColors.Add("Blue");
            LevelBallColors.Add("Orange");
        }
        foreach (var ball in balls)
        {
            ball.Initialize(GetRandomBallColor());
        }
        CheckGrid();
    }

    public void CheckGrid()
    {
        foreach (var ball in balls)
        {
            ball.Check();
        }
        foreach(var portal in portals)
        {
            portal.Check();
        }
    }

    public string GetRandomBallColor()
    {
        var randomIndex = UnityEngine.Random.Range(0, LevelBallColors.Count);
        var ballType = LevelBallColors[randomIndex];
        LevelBallColors.RemoveAt(randomIndex);

        return ballType;
    }

    public Node GetNodeByPosition(int X, int Y)
    {
        return nodes.Find(gp => gp.x == X && gp.y == Y);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                Ball ball = hit.collider.GetComponent<Ball>();
                if (ball == null) return;
                ball.onClick();
            }
        }
    }

    public void addToBottomNode(Ball ball)
    {
        BottomNode node = bottomNodes.FirstOrDefault(n => n.ball == null);
        if (node == null) { ISPLAYING = false; Debug.Log("LOST"); return; }
        node.ball = ball;

        BottomNodesCalculations();
    }

    private void BottomNodesCalculations()
    {
        var allBottomBalls = bottomNodes.Select(node => node.ball).Where(b => b != null).ToList();

        var groupedBalls = allBottomBalls.GroupBy(ball => ball.type).OrderByDescending(group => group.Count());
        var sortedBalls = new List<Ball>();
        foreach (var ballGroup in groupedBalls)
        {
            if (ballGroup.Count() == 3)
            {
                foreach (var item in ballGroup)
                {
                    item.bottomNode = null;
                    allBottomBalls.Remove(item);
                    Destroy(item.gameObject);
                }
            }
            else
            {
                sortedBalls.AddRange(ballGroup);
            }
        }

        for (int i = 0; i < bottomNodes.Count; i++)
        {
            if (i <= sortedBalls.Count - 1) { bottomNodes[i].ball = sortedBalls[i]; sortedBalls[i].bottomNode = bottomNodes[i]; }
            else { bottomNodes[i].ball = null; }
        }

        foreach (var b in allBottomBalls)
        {
            b.CallGoToBottomRow();
        }
    }

    public void CheckBottomRow()
    {
        //throw new NotImplementedException();
    }
}
