using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour 
{

	public bool displayGridGizmos;
	public LayerMask unwalkableMask;
	public Vector3 gridWorldSize;
	public float nodeRadius;
	Node[, ,] grid;

	float nodeDiameter;
	int gridSizeX, gridSizeY, gridSizeZ;

	void Awake() 
	{
		StartCoroutine(Delay(1));
	}
	
	IEnumerator Delay(int seconds)
	{
		yield return new WaitForSeconds(seconds);
		
		nodeDiameter = nodeRadius*2;
		gridSizeX = Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY = Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);
		gridSizeZ = Mathf.RoundToInt(gridWorldSize.z/nodeDiameter);
		CreateGrid();
		
		yield return new WaitForSeconds(seconds);
	}

	public int MaxSize 
	{
		get 
		{
			return gridSizeX * gridSizeY * gridSizeZ;
		}
	}

	void CreateGrid() 
	{
		grid = new Node[gridSizeX,gridSizeY,gridSizeZ];
		Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y/2;

		for (int x = 0; x < gridSizeX; x ++) 
		{
			for (int y = 0; y < gridSizeY; y ++) 
			{
				for (int z = 0; z < gridSizeZ; z ++)
				{
					Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius) + Vector3.up * (z * nodeDiameter + nodeRadius);
					bool walkable = !(Physics.CheckSphere(worldPoint,nodeRadius,unwalkableMask));
					grid[x,y,z] = new Node(walkable,worldPoint, x,y,z);
				}
			}
		}
	}

	public List<Node> GetNeighbours(Node node) 
	{
		List<Node> neighbours = new List<Node>();

		for (int x = -1; x <= 1; x++) 
		{
			for (int y = -1; y <= 1; y++) 
			{
				for (int z = -1; z <= 1; z++)
				{
					if (x == 0 && y == 0)
						continue;

					int checkX = node.gridX + x;
					int checkY = node.gridY + y;
					int checkZ = node.gridZ + z;

					if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY && checkZ >= 0 && checkZ < gridSizeZ) 
					{
						neighbours.Add(grid[checkX,checkY,checkZ]);
					}
				}
			}
		}

		return neighbours;
	}
	

	public Node NodeFromWorldPoint(Vector3 worldPosition) 
	{
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		float percentZ = (worldPosition.y + gridWorldSize.z/2) / gridWorldSize.z;
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		percentZ=  Mathf.Clamp01(percentZ);

		int x = Mathf.RoundToInt((gridSizeX-1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY-1) * percentY);
		int z = Mathf.RoundToInt((gridSizeZ-1) * percentZ);
		return grid[x,y,z];
	}
	
	void OnDrawGizmos() 
	{
		Vector3 wireLocation = transform.position;
		wireLocation.y = wireLocation.y + gridWorldSize.z/2;
		
		Gizmos.DrawWireCube(wireLocation,new Vector3(gridWorldSize.x,gridWorldSize.z,gridWorldSize.y));
		if (grid != null && displayGridGizmos) 
		{
			foreach (Node n in grid) 
			{
				Gizmos.color = (n.walkable)?Color.white:Color.red;
				Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter-.1f));
			}
		}
	}
}
