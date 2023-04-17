using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target : MonoBehaviour
{
    public Transform searchDrone;
    
    public Material obstacle;
    public Material target;
    
    bool isTree = false;
    
    void Start()
    {
    	this.searchDrone = GameObject.FindWithTag("search").transform;
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
