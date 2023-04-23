using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Environment : MonoBehaviour
{
    #region Variables

    [Header("Obstacle Generator")] [SerializeField]
    private List<GameObject> obstaclePrefabList;

    [SerializeField] [Range(1, 99)] private int obstacleDensity;
    [SerializeField] private GameObject groundObject;
    public Vector3 groundSize = Vector3.zero;
    [SerializeField] private GameObject goalObject;

    private Vector3 _highestDensityPosition;

    // Obstacle List
    private List<GameObject> _instantiatedObstacleList;
    public bool environmentInstantiated;
    #endregion

    #region Main Methods

    void Start()
    {
        //Check if Necessary Fields are Complete
        if (obstaclePrefabList.Count == 0)
            Debug.LogError("Obstacles not Serialized");
        if (groundObject == null)
            Debug.LogError("Ground Game Object not Serialized");
        Vector3 groundScale = groundObject.transform.localScale;
        groundSize = new Vector3(9*groundScale.x / 2.0f, groundScale.y, 9*groundScale.z / 2.0f);

        InstantiateObstacles();
        GenerateHeatMap();
        environmentInstantiated = true;
    }
    #endregion

    #region Custom Methods

    private void InstantiateObstacles()
    {
        _instantiatedObstacleList = new List<GameObject>();
        Vector3 obstacleSize = new Vector3(300,0,200);
        int maxObstacleCount =
            (int)(((2 * groundSize.x * 2 * groundSize.z) / (obstacleSize.x * obstacleSize.z))*(obstacleDensity/100.0f));
        for (int i = 0; i < maxObstacleCount; i++)
        {
            var position = GetRandomLocation();
            var randomObstacleIndex = (new System.Random()).Next(4);
            GameObject obstacle = Instantiate(original: obstaclePrefabList[randomObstacleIndex], this.transform, true);
            obstacle.transform.position = position;
            _instantiatedObstacleList.Add(obstacle);
        }
    }

    private Vector3 GetRandomLocation()
    {
        var rnd = new System.Random();
        int x = rnd.Next((int)-groundSize.x, (int)groundSize.x);
        int z = rnd.Next((int)-groundSize.z, (int)groundSize.z);
        return new Vector3(x, 1, z);
    }

    public List<BoxCollider> GetCollidersInField()
    {
        List<BoxCollider> allCollidersInField = new();
        foreach (var obstacle in _instantiatedObstacleList)
        {
            var collidersInObstacleList = obstacle.GetComponentsInChildren<BoxCollider>().ToList();
            foreach (var colliderInObstacle in collidersInObstacleList)
                allCollidersInField.Add((colliderInObstacle));
        }

        return allCollidersInField;
    }
    
    private void GenerateHeatMap()
    {
        int highestDensityCount = 0;

        int rayLength = (int)Mathf.Pow(50.0f,2.0f);
        for (int x = -(int)groundSize.x; x < (int)groundSize.x; x += 100)
        {
            for (int z = -(int)groundSize.z; z < (int)groundSize.z; z += 100)
            {
                int densityCount = 0;
                Vector3 origin = new(x, 200, z);
                foreach (var obstacle in _instantiatedObstacleList)
                {
                    var distance = (origin - obstacle.transform.position).magnitude;
                    if (distance < rayLength)
                        densityCount++;
                }

                if (densityCount > highestDensityCount)
                {
                    highestDensityCount = densityCount;
                    _highestDensityPosition = origin;
                }
            }
        }
        Debug.Log("Highest Density Position: "+ _highestDensityPosition);
        goalObject.transform.position = new Vector3(_highestDensityPosition.x, 50, _highestDensityPosition.z);
    }

    public Vector3 GetGoalSearchPosition()
    {
        return this._highestDensityPosition;
    }
    #endregion
}