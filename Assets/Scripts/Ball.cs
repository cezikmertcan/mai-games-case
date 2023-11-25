using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class Ball : MonoBehaviour
{
    [HideInInspector]
    public Node node;
    [HideInInspector]
    public string type = "None";
    [HideInInspector]
    public BottomNode bottomNode;
    public Material matRed;
    public Material matGreen;
    public Material matBlue;
    public Material matOrange;

    public bool isClickable = false;

    Coroutine goToBottomRowCoroutine;


    public void Initialize(string type)
    {
        this.type = type;

        if (type == "Red")
        {
            GetComponent<Renderer>().material = matRed;
        }
        else if (type == "Green")
        {
            GetComponent<Renderer>().material = matGreen;
        }
        else if (type == "Blue")
        {
            GetComponent<Renderer>().material = matBlue;
        }
        else if (type == "Orange")
        {
            GetComponent<Renderer>().material = matOrange;
        }

    }

    public void Check()
    {
        if (node.BFS().Count == 0)
        {
            transform.position = new Vector3(transform.position.x, -0.5f, transform.position.z);
            isClickable = false;
        }
        else
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            isClickable = true;
        }
    }

    public void onClick()
    {
        if (!isClickable || !C.gameManager.ISPLAYING) return;
        isClickable = false;
        C.gameManager.balls.Remove(this);
        var nodesToGo = node.BFS();
        StartCoroutine(GoToLastRow(nodesToGo));
        node.type = "E";
        C.gameManager.CheckGrid();
        if (bottomNode) bottomNode.ball = this;

    }

    IEnumerator GoToLastRow(List<Node> nodes)
    {
        foreach (var node in nodes)
        {
            Vector3 targetPosition = new Vector3(node.transform.position.x, transform.position.y, node.transform.position.z);
            Vector3 direction = (targetPosition - transform.position).normalized / 10;
            while (Vector3.Distance(transform.position, targetPosition) > (direction.magnitude * 1.1))
            {
                if (!C.gameManager.ISPLAYING) StopAllCoroutines();
                transform.Translate(direction);
                yield return new WaitForSeconds(1 / 60f);
            }
            transform.position = targetPosition;
        }
        C.gameManager.addToBottomNode(this);
    }

    public void CallGoToBottomRow()
    {
        if (goToBottomRowCoroutine != null) StopCoroutine(goToBottomRowCoroutine);
        goToBottomRowCoroutine = StartCoroutine(GoToBottomRow());
    }

    IEnumerator GoToBottomRow()
    {
        Vector3 targetPosition = new Vector3(bottomNode.transform.position.x, transform.position.y, bottomNode.transform.position.z);
        Vector3 direction = (targetPosition - transform.position).normalized / 10;
        while (Vector3.Distance(transform.position, targetPosition) > (direction.magnitude * 1.1))
        {
            transform.Translate(direction);
            yield return new WaitForSeconds(1 / 60f);
        }
        transform.position = targetPosition;
    }
}
