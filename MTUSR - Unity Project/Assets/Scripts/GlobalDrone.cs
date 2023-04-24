using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class GlobalDrone : MonoBehaviour
{
    #region Variables

    // The Global Drone Planner uses A-Star for path planning
    [SerializeField] private Vector3 goalPosition;
    [SerializeField] private GameObject droneObject;
    [SerializeField] private DroneAnimation droneAnimator;
    [SerializeField] private Material selectionMaterial,nodeMaterial;
    [SerializeField] private string mappingLayerString;
    private int _mappingLayer;
    
    private bool _endGoalFound, _pathFound, _pathRetraced;
    public bool heatMapDeveloped;
    [SerializeField] private float maxDroneSpeed;
    private Vector3 _droneHalfCollider;
    private const float MoveDistance = 100.0f;
    Vector3 _highestDensityPosition;
    [SerializeField] private Environment environment;

    private readonly List<Vector3> _neighborPositionsToScan = new List<Vector3>()
    {
        new Vector3(MoveDistance, MoveDistance, MoveDistance),
        new Vector3(MoveDistance, MoveDistance, 0),
        new Vector3(MoveDistance, MoveDistance, -MoveDistance),

        new Vector3(0, MoveDistance, MoveDistance),
        new Vector3(0, MoveDistance, 0),
        new Vector3(0, MoveDistance, -MoveDistance),

        new Vector3(-MoveDistance, MoveDistance, MoveDistance),
        new Vector3(-MoveDistance, MoveDistance, 0),
        new Vector3(-MoveDistance, MoveDistance, -MoveDistance),

        new Vector3(MoveDistance, 0, MoveDistance),
        new Vector3(MoveDistance, 0, 0),
        new Vector3(MoveDistance, 0, -MoveDistance),

        new Vector3(0, 0, MoveDistance),
        new Vector3(0, 0, -MoveDistance),

        new Vector3(-MoveDistance, 0, MoveDistance),
        new Vector3(-MoveDistance, 0, 0),
        new Vector3(-MoveDistance, 0, -MoveDistance),

        new Vector3(MoveDistance, -MoveDistance, MoveDistance),
        new Vector3(MoveDistance, -MoveDistance, 0),
        new Vector3(MoveDistance, -MoveDistance, -MoveDistance),

        new Vector3(0, -MoveDistance, MoveDistance),
        new Vector3(0, -MoveDistance, 0),
        new Vector3(0, -MoveDistance, -MoveDistance),

        new Vector3(-MoveDistance, -MoveDistance, MoveDistance),
        new Vector3(-MoveDistance, -MoveDistance, 0),
        new Vector3(-MoveDistance, -MoveDistance, -MoveDistance),
    };

    class AStarNode
    {
        private readonly GameObject _nodeGameObject;
        private AStarNode _parentNode;

        public int GCost { get; private set; }
        public int HCost { get; set; }
        public int FCost { get; private set; }

        private LineRenderer _edge;

        public AStarNode(Vector3 position, AStarNode parent, Transform parentTransform, int mappingLayer, Material nodeMaterial)
        {
            _nodeGameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _nodeGameObject.transform.position = position;
            _nodeGameObject.transform.parent = parentTransform;
            _nodeGameObject.layer = mappingLayer;
            _nodeGameObject.GetComponent<MeshRenderer>().material = nodeMaterial;
            
            Destroy(_nodeGameObject.GetComponent<SphereCollider>());

            _parentNode = parent;


            if (parent != null)
            {
                _edge = _nodeGameObject.AddComponent<LineRenderer>();
                _edge.material = nodeMaterial;
                _edge.gameObject.layer = mappingLayer;
                _edge.widthMultiplier = 0.5f;
                _edge.positionCount = 2;
                _edge.SetPosition(0, _nodeGameObject.transform.position);
                _edge.SetPosition(1, _parentNode.GetTransform().position);
            }
        }

        public void SetCost(int gCost, int hCost)
        {
            this.GCost = gCost;
            this.HCost = hCost;
            this.FCost = gCost + hCost;
        }

        public Transform GetTransform()
        {
            return this._nodeGameObject.transform;
        }

        public AStarNode GetParent()
        {
            return this._parentNode;
        }

        public void UpdateParent(AStarNode parent, int gCost, int hCost)
        {
            _parentNode = parent;
            this.GCost = gCost;
            this.HCost = hCost;
            this.FCost = gCost + hCost;
            _edge.SetPosition(0, _nodeGameObject.transform.position);
            _edge.SetPosition(1, _parentNode.GetTransform().position);
        }

        public void SelectNodeAsPath(Material selectionMaterial)
        {
            _nodeGameObject.transform.localScale = new Vector3(10.0f, 10.0f, 10.0f);
            _nodeGameObject.GetComponent<MeshRenderer>().material = selectionMaterial;
            _nodeGameObject.layer = 0;

            var edge = _nodeGameObject.GetComponent<LineRenderer>();
            if (edge != null)
            {
                _edge.gameObject.layer = 0;
                _edge.material = selectionMaterial;
                edge.widthMultiplier = 4.0f;
                edge.startColor = selectionMaterial.color;
                edge.endColor = selectionMaterial.color;
            }
        }
    }

    private List<AStarNode> _openList, _closedList, _nodeList, _travelPath;

    #endregion

    #region Main Methods

    void Start()
    {
        Debug.Log("Finding Path to Center of Field");
        _mappingLayer = LayerMask.NameToLayer(mappingLayerString);

        _openList = new();
        _closedList = new();
        _nodeList = new();

        var position = droneObject.transform.position;
        AStarNode startNode = new AStarNode(position, null, this.transform,_mappingLayer,nodeMaterial);
        int gCost, hCost;
        (gCost, hCost) = FindCost(position, position, goalPosition);
        startNode.SetCost(gCost, hCost);
        _openList.Add(startNode);
        _nodeList.Add(startNode);

        _droneHalfCollider =
            (droneObject.transform.localScale.x * droneObject.GameObject().GetComponent<BoxCollider>().size) / 2.0f;
        Destroy(droneObject.GameObject().GetComponent<BoxCollider>());
    }

    void Update()
    {
        if (!_endGoalFound && environment.environmentInstantiated)
        {
            if (_openList.Count != 0)
            {
                AStarNode openNode = SmallestFCostFromOpenList();
                _openList.Remove(openNode);
                _closedList.Add(openNode);
                List<Vector3> neighborNodes = FindNeighborNodes(openNode.GetTransform().position);
                foreach (var nodeInClosedList in _closedList)
                {
                    if (neighborNodes.Contains(nodeInClosedList.GetTransform().position))
                        neighborNodes.Remove(nodeInClosedList.GetTransform().position);
                }

                foreach (var nodeInOpenList in _openList)
                {
                    if (neighborNodes.Contains(nodeInOpenList.GetTransform().position))
                    {
                        //Discovered Node Already in Open List
                        neighborNodes.Remove(nodeInOpenList.GetTransform().position);
                        int tempGCost, tempHCost;
                        (tempGCost, tempHCost) = FindCost(openNode.GetTransform().position,
                            nodeInOpenList.GetTransform().position, goalPosition);
                        tempGCost += openNode.GCost;
                        int tempFCost = tempGCost + tempHCost;
                        if (tempFCost < nodeInOpenList.FCost)
                            nodeInOpenList.UpdateParent(openNode, tempGCost, tempHCost);
                    }
                }

                foreach (var neighborPosition in neighborNodes)
                {
                    AStarNode node = new AStarNode(neighborPosition, openNode, this.transform,_mappingLayer,nodeMaterial);
                    int gCost, hCost;
                    (gCost, hCost) = FindCost(openNode.GetTransform().position, neighborPosition, goalPosition);
                    gCost += openNode.GCost;
                    node.SetCost(gCost, hCost);
                    _openList.Add(node);
                    _nodeList.Add(node);
                    if ((neighborPosition - goalPosition).sqrMagnitude < 10000.0f)
                    {
                        Debug.Log("Path to Center of Field Found");
                        _endGoalFound = true;
                        break;
                    }
                }
            }
            else
                Debug.LogError("OpenList is empty");
        }

        if (_endGoalFound && !_pathFound)
        {
            _travelPath = new List<AStarNode>();

            var currentNode = _nodeList.Last();
            while (currentNode != null)
            {
                currentNode.SelectNodeAsPath(selectionMaterial);
                _travelPath.Add(currentNode);
                currentNode = currentNode.GetParent();
            }

            _travelPath.Reverse();
            _pathFound = true;
            StartCoroutine(TravelOnPath());
        }
        else if (_pathFound && _pathRetraced && !heatMapDeveloped)
        {
            _highestDensityPosition = environment.GetGoalSearchPosition();
            heatMapDeveloped = true;
        }
    }

    #endregion

    #region Custom Methods

    private static (int gCost, int hCost) FindCost(Vector3 parentNodePosition, Vector3 currentPosition,
        Vector3 endNodePosition)
    {
        int gCost = (int)((parentNodePosition - currentPosition) / 10.0f).sqrMagnitude;
        int hCost = (int)((endNodePosition - currentPosition) / 10.0f).sqrMagnitude;
        return (gCost, hCost);
    }

    private AStarNode SmallestFCostFromOpenList()
    {
        int fCost = int.MaxValue;
        int smallestFCostNodeIndex = 0;
        for (int i = 0; i < _openList.Count; i++)
        {
            int nodeFCost = _openList[i].FCost;
            if (nodeFCost < fCost)
            {
                fCost = nodeFCost;
                smallestFCostNodeIndex = i;
            }
        }

        return _openList[smallestFCostNodeIndex];
    }

    private List<Vector3> FindNeighborNodes(Vector3 position)
    {
        List<Vector3> neighbors = new List<Vector3>();
        foreach (var neighborPosition in _neighborPositionsToScan)
        {
            Vector3 possiblePosition = position + neighborPosition;
            bool obstacleInPath = false;
            for (float t = 0; t < 1; t += 0.1f)
            {
                Vector3 interpolatedPosition = Vector3.Lerp(position, possiblePosition, t);
                if (Physics.CheckBox(interpolatedPosition, _droneHalfCollider))
                {
                    obstacleInPath = true;
                    break;
                }
            }

            if (!obstacleInPath)
                neighbors.Add(possiblePosition);
        }

        return neighbors;
    }

    private IEnumerator TravelOnPath()
    {
        droneAnimator.ActivateDrone();
        int i = 0;
        var startNode = _travelPath[0];
        var startPosition = startNode.GetTransform().position;

        var nextNode = _travelPath[1];
        var nextPosition = nextNode.GetTransform().position;
        float time = 0.0f;

        float travelDistance = (startPosition - nextPosition).sqrMagnitude;
        float timeStep = travelDistance / maxDroneSpeed;

        while (true)
        {
            Vector3 position = Vector3.Lerp(startPosition, nextPosition, time);
            droneObject.transform.position = position;
            time += (1 / timeStep);
            if (time >= 1.0f)
            {
                i++;

                if (i == _travelPath.Count)
                {
                    _pathRetraced = true;
                    StopAllCoroutines();
                    break;
                }

                startNode = nextNode;
                startPosition = startNode.GetTransform().position;

                nextNode = _travelPath[i];
                nextPosition = nextNode.GetTransform().position;

                travelDistance = (startPosition - nextPosition).sqrMagnitude;
                timeStep = travelDistance / maxDroneSpeed;
                time = 0.0f;
            }

            yield return null;
        }
    }



    public Vector3 GetPossibleLocationForGoalObject()
    {
        return this._highestDensityPosition;
    }

    #endregion
}