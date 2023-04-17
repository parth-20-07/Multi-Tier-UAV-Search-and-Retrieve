using UnityEngine;
using System.Collections;
using System;

public class Unit : MonoBehaviour 
{

	public float turningRadius;
	public Transform target;
	
	public float speed;
	public float maxTurnAngle;
	
	Vector3[] path;
	int targetIndex;
	
	Vector3 previousTargetPosition;

	void Start() 
	{
		int seconds = 5;
		
		this.target = GameObject.FindWithTag("Finish").transform;
		
		StartCoroutine(Delay(seconds));
		
		previousTargetPosition = target.position;
	}
	
	void FixedUpdate()
	{
		Vector3 currentWaypoint = path[0];
	
		if (previousTargetPosition != target.position)
		{
			OnUpdatePath();
			previousTargetPosition = target.position;
		}
		
		if (Math.Abs(transform.position.x - target.position.x) < turningRadius && Math.Abs(transform.position.z - target.position.z) < turningRadius)
		{
			
		}
		
	}
	
	void OnUpdatePath()
	{
		PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
	}
	
	public void OnPathFound(Vector3[] newPath, bool pathSuccessful) 
	{
		if (pathSuccessful) 
		{
			path = newPath;
			targetIndex = 0;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}
	
	IEnumerator Delay(int seconds)
	{
		yield return new WaitForSeconds(seconds);
		
		PathRequestManager.RequestPath(transform.position,target.position, OnPathFound);
	}

	IEnumerator FollowPath() 
	{
		Vector3 currentWaypoint = path[0];
		while (true) 
		{
			if (transform.position == currentWaypoint) 
			{
				targetIndex ++;
				if (targetIndex >= path.Length) 
				{
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}
			
			transform.position = Vector3.MoveTowards(transform.position,currentWaypoint,speed * Time.deltaTime);
			yield return null;
		}
		
	}

	public void OnDrawGizmos() 
	{
		if (path != null) 
		{
			for (int i = targetIndex; i < path.Length; i ++) 
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if (i == targetIndex) 
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else 
				{
					Gizmos.DrawLine(path[i-1],path[i]);
				}
			}
		}
	}
}
