using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Environment : MonoBehaviour
{
    #region Variables

    [Header("Obstacle Generator")] [SerializeField]
    private List<GameObject> obstaclePrefabList;

    [SerializeField] [Range(1, 20)] private int obstacleDensity;
    [SerializeField] private GameObject groundObject;
    Vector3 _groundSize = Vector3.zero;

    // Obstacle List
    private List<GameObject> _instantiatedObstacleList;
    public bool environmentInstantiated;

    [Header("Goal Object")] [SerializeField]
    private GameObject goalObject;
    #endregion

    #region Main Methods

    void Start()
    {
        //Check if Necessary Fields are Complete
        if (obstaclePrefabList.Count == 0)
            Debug.LogError("Obstacles not Serialized");
        if (groundObject == null)
            Debug.LogError("Ground Game Object not Serialized");
        if (goalObject == null)
            Debug.LogError("Goal Game Object not Serialized");

        Vector3 groundScale = groundObject.transform.localScale;
        _groundSize = new Vector3(9*groundScale.x / 2.0f, groundScale.y, 9*groundScale.z / 2.0f);

        InstantiateObstacles();
        PlaceGoalObject();
        environmentInstantiated = true;
    }
    #endregion

    #region Custom Methods

    private void InstantiateObstacles()
    {
        _instantiatedObstacleList = new List<GameObject>();
        Vector3 obstacleSize = new Vector3(300,0,200);
        int maxObstacleCount =
            (int)(((2 * _groundSize.x * 2 * _groundSize.z) / (obstacleSize.x * obstacleSize.z))*(obstacleDensity/100.0f));
        for (int i = 0; i < maxObstacleCount; i++)
        {
            var position = GetRandomLocation();
            var randomObstacleIndex = (new System.Random()).Next(4);
            GameObject obstacle = Instantiate(original: obstaclePrefabList[randomObstacleIndex], this.transform, true);
            obstacle.transform.position = position;
            _instantiatedObstacleList.Add(obstacle);
        }
    }

    private void PlaceGoalObject()
    {
        bool objectPlaced = false;
        Vector3 goalBoxSize = goalObject.GetComponent<BoxCollider>().bounds.size;
            goalBoxSize = new Vector3 (goalBoxSize.x/3.0f,goalBoxSize.y/3.0f,goalBoxSize.z/3.0f);
            goalBoxSize -= new Vector3(1.0f, 1.0f, 1.0f);
        while (!objectPlaced)
        {
            Vector3 location = GetRandomLocation();
            location.y = goalObject.transform.position.y;
            if (!Physics.CheckBox(location, goalBoxSize))
            {
                goalObject.transform.position = location;
                objectPlaced = true;
            }
        }
    }


    private Vector3 GetRandomLocation()
    {
        var rnd = new System.Random();
        int x = rnd.Next((int)-_groundSize.x, (int)_groundSize.x);
        int z = rnd.Next((int)-_groundSize.z, (int)_groundSize.z);
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
    #endregion
}