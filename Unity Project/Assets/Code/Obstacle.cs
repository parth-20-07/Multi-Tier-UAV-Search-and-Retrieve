using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Transform searchDrone;
    
    public Material obstacle;
    public Material target;
    
    bool isTree = true;
    
    void Start()
    {
    	this.searchDrone = GameObject.FindWithTag("search").transform;
    	
    	int x_size = Random.Range(1, 3);
    	int y_size = Random.Range(10, 50);
    	int z_size = Random.Range(1, 3);
    	
    	this.transform.localScale = new Vector3(x_size, y_size, z_size);
    	//this.transform.position.y = y_size/2;
    }
    
    void Update()
    {
        if ((transform.position - searchDrone.transform.position).magnitude <= 10)
        {
        	if (isTree)
        	{
        		this.GetComponent<Renderer>().material = obstacle;
        	}
        	else if (!isTree)
        	{
        		this.GetComponent<Renderer>().material = target;
        	}
        }
    }
}
