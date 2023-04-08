using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
{

	public GameObject treePrefab;
	public GameObject startPrefab;
	public GameObject goalPrefab;
	
	void Awake()
	{
		int[,] gridworld = new int[100, 100];
		
		int count = 0;
		
		while (count < 150)
		{
			int row_look = Random.Range(0, 99);
			int column_look = Random.Range(0, 99);
			
			if (gridworld[row_look, column_look] != 0)
			{
				continue;
			}
			
			gridworld[row_look, column_look] = 1;
			
			Vector3 spawnPosition = new Vector3(row_look - 50, 0, column_look - 50);
			Instantiate(treePrefab, spawnPosition, Quaternion.identity);
			count++;
		}
		
		//Vector3 startSpawnPosition = new Vector3(-45, 0, 45);
		//Instantiate(startPrefab, startSpawnPosition, Quaternion.identity);
			
		Vector3 goalSpawnPosition = new Vector3(45, 10, -45);
		Instantiate(goalPrefab, goalSpawnPosition, Quaternion.identity);
	}

	void Update()
	{
        	if(Input.GetKeyDown(KeyCode.Space))
        	{
        		Vector3 randomSpawnPosition = new Vector3(Random.Range(-49, 50), 0, Random.Range(-49, 50));
        		Instantiate(treePrefab, randomSpawnPosition, Quaternion.identity);
        	}
	}
}
