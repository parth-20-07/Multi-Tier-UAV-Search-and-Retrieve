using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Environment : MonoBehaviour
{
    #region Variables

    [FormerlySerializedAs("obstacleList")] [Header("Obstacle Generator")] [SerializeField]
    private List<GameObject> obstaclePrefabList;

    [SerializeField] [Range(1, 20)] private int obstacleDensity;
    [SerializeField] private GameObject groundObject;
    Vector3 _groundSize = Vector3.zero;

    // Obstacle List
    private List<GameObject> _instantiatedObstacleList;

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
        _groundSize = new Vector3(9*groundScale.x / 2.0f, groundScale.y, 9*groundScale.z / 2.0f);

        InstantiateObstacles();
    }
    #endregion

    #region Custom Methods

    private void InstantiateObstacles()
    {
        _instantiatedObstacleList = new List<GameObject>();
        int maxObstacleCount =
            (int)((2 * _groundSize.x * 2 * _groundSize.z*obstacleDensity) / (3 * 1 * 1000 * 1000 * 1.0f *100 ));
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
        int x = rnd.Next((int)-_groundSize.x, (int)_groundSize.x);
        int z = rnd.Next((int)-_groundSize.z, (int)_groundSize.z);
        return new Vector3(x, 1, z);
    }

    public List<GameObject> GetObstaclesInField()
    {
        return this._instantiatedObstacleList;
    }
    #endregion
}