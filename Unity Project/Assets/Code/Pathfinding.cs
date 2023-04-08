using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class Pathfinding : MonoBehaviour 
{
	
	PathRequestManager requestManager;
	Grid grid;
	
	void Awake() 
	{
		requestManager = GetComponent<PathRequestManager>();
		grid = GetComponent<Grid>();
	}
	
	public void StartFindPath(Vector3 startPosition, Vector3 targetPosition) 
	{
		StartCoroutine(FindPath(startPosition,targetPosition));
	}
	
	IEnumerator FindPath(Vector3 startPosition, Vector3 targetPosition) 
	{

		Vector3[] waypoints = new Vector3[0];
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(startPosition);
		Node targetNode = grid.NodeFromWorldPoint(targetPosition);
		
		
		if (startNode.walkable && targetNode.walkable) 
		{
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) 
			{
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) 
				{
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbours(currentNode)) 
				{
					if (!neighbour.walkable || closedSet.Contains(neighbour)) 
					{
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) 
					{
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;
						
						if (!openSet.Contains(neighbour))
							openSet.Add(neighbour);
					}
				}
			}
		}
		yield return null;
		if (pathSuccess) 
		{
			waypoints = RetracePath(startNode,targetNode);
		}
		requestManager.FinishedProcessingPath(waypoints,pathSuccess);
		
	}
	
	Vector3[] RetracePath(Node startNode, Node endNode) 
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;
		
		while (currentNode != startNode) 
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		Vector3[] waypoints = SimplifyPath(path);
		Array.Reverse(waypoints);
		return waypoints;
		
	}
	
	Vector3[] SimplifyPath(List<Node> path) 
	{
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;
		
		for (int i = 1; i < path.Count; i ++) 
		{
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX,path[i-1].gridY - path[i].gridY);
			if (directionNew != directionOld) 
			{
				waypoints.Add(path[i].worldPosition);
			}
			directionOld = directionNew;
		}
		return waypoints.ToArray();
	}
	
	int GetDistance(Node nodeA, Node nodeB) 
	{
		int xDistance = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int yDistance = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		int zDistance = Mathf.Abs(nodeA.gridZ - nodeB.gridZ);
		
		/*
		if (xDistance > yDistance)// && xDistance > zDistance)
			return 14*yDistance + 10* (xDistance-yDistance);
		return 14*xDistance + 10 * (yDistance-xDistance);
			
		}
		else if (yDistance > xDistance && yDistance > zDistance)
		{
			
		}
		else if (zDistance > xDistance && zDistance > yDistance)
		{
			
		}*/
		
		Vector3 distanceVector = new Vector3(xDistance, yDistance, zDistance);
		
		return (int)Math.Round(distanceVector.magnitude);
		
	}
	
	
}

