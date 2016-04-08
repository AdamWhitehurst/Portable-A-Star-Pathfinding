using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Grid : MonoBehaviour
{
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	Node[,] grid;
	float nodeDiameter;
	public int 
		gridSizeX, 
		gridSizeY;
	public bool displayGridGizmos;

	void Awake ()
	{
		nodeDiameter = nodeRadius * 2;
		gridSizeX = Mathf.RoundToInt (gridWorldSize.x / nodeDiameter);
		gridSizeY = Mathf.RoundToInt (gridWorldSize.y / nodeDiameter);
		CreateGrid ();
	}

	public int MaxSize
	{
		get {
			return gridSizeX * gridSizeY;
		}
	}
	 
	void CreateGrid ()
	{
		grid = new Node[gridSizeX, gridSizeY];
		Vector2 worldBottomLeft = (Vector2) transform.position - Vector2.right * gridWorldSize.x / 2 - Vector2.up * gridWorldSize.y / 2;

		for (int x = 0; x < gridSizeX; x ++) {
			for (int y = 0; y < gridSizeY; y ++) {
				Vector2 worldPoint = worldBottomLeft + Vector2.right * (x * nodeDiameter + nodeRadius ) + Vector2.up * (y * nodeDiameter + nodeRadius );
				//bool walkable = !(Physics.CheckSphere (worldPoint, nodeRadius, unwalkableMask));
				bool walkable = !(Physics2D.CircleCast (worldPoint, nodeRadius, Vector2.zero, 1f, unwalkableMask) );
				grid [x, y] = new Node (walkable, worldPoint, x, y);
			}
		}
	}
	
	public Node NodeFromWorldPoint(Vector2 worldPosition) {
		float percentX = Mathf.InverseLerp (0,gridWorldSize.x, worldPosition.x + nodeRadius);
		float percentY = Mathf.InverseLerp (0,gridWorldSize.y, worldPosition.y + nodeRadius);
		//Debug.Log (percentX + ", " + percentY);

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		
		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		//Debug.Log (x + ", " + y);
		return grid[x,y];
	}

	void OnDrawGizmos ()
	{
		Gizmos.DrawWireCube (transform.position, new Vector3 (gridWorldSize.x, gridWorldSize.y, 1.0f));
		
		if (grid != null && displayGridGizmos) {
			foreach (Node n in grid) {
				Gizmos.color = (n.walkable) ? (new Color (0,255,0)) : (new Color (255,0,0));
				Gizmos.DrawCube (n.worldPosition, Vector3.one * (nodeDiameter - .1f));
			}
		}
	}

	public  List<Node> GetNeighbors (Node node)
	{
		List<Node> neighbors = new List<Node>();

		for (int x = -1 ; x <= 1 ; x++) 
		{
			for (int y = -1 ; y <=1 ; y++)
			{
				if (x == 0 && y == 0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbors.Add (grid[checkX,checkY]);
				}
			}
		}
		return neighbors;
	}
}

public class Node : IHeapItem <Node>
{
	
	public bool walkable;
	public Vector2 worldPosition;

	public int
		gridX,
		gridY,
		gCost,
		hCost;
	int heapIndex;

	public Node parent;

	public Node (bool _walkable, Vector3 _worldPos, int _gridX, int _gridY)
	{
		walkable = _walkable;
		worldPosition = _worldPos;
		gridX = _gridX;
		gridY = _gridY;
	}

	public int fCost
	{
		get 
		{
			return gCost + hCost;
		}
	}

	public int HeapIndex
	{
		get {
			return heapIndex;
		}
		set {
			heapIndex = value;
		}
	}

	public int CompareTo (Node nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);
		if ( compare == 0 )
		{
			compare = hCost.CompareTo(nodeToCompare.hCost);
		}

		return -compare;
	}
}