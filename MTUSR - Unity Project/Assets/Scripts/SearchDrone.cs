using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SearchDrone : MonoBehaviour
{
    #region Variables

    [SerializeField] private GameObject droneObject;
    [SerializeField] private DroneAnimation droneAnimator;
    [SerializeField] private GlobalDrone globalDroneCommunication;

    private Vector3 _goalPosition;
    [SerializeField] private float maxDroneSpeed;

    private bool _goalPositionFetched, _pathDeveloped, _motionStarted, _pathRetraced;

    private readonly float _gapHeight = 50.0f;
    private readonly float _gapLength = 100.0f;

    class AddNodeToPath
    {
        private readonly GameObject _node;
        private readonly AddNodeToPath _parentNode;

        public AddNodeToPath(Vector3 position, AddNodeToPath parent, Transform parentTransform)
        {
            _node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            _node.transform.position = position;
            _node.transform.parent = parentTransform;

            _parentNode = parent;

            if (_parentNode != null)
            {
                LineRenderer edge = _node.AddComponent<LineRenderer>();
                edge.widthMultiplier = 0.5f;
                edge.positionCount = 2;
                edge.SetPosition(0, _node.transform.position);
                edge.SetPosition(1, _parentNode.GetTransform().position);
            }
        }

        public Transform GetTransform()
        {
            return _node.transform;
        }

        public AddNodeToPath GetParent()
        {
            return _parentNode;
        }
    }

    private List<AddNodeToPath> _path;

    #endregion

    #region Main Methods

    private void Start()
    {
        _path = new List<AddNodeToPath>();
        AddNodeToPath startNode = new AddNodeToPath(droneObject.transform.position, null, this.transform);
        _path.Add(startNode);
    }

    void Update()
    {
        if (!_goalPositionFetched && globalDroneCommunication.heatMapDeveloped)
        {
            _goalPosition = globalDroneCommunication.GetPossibleLocationForGoalObject();
            _goalPositionFetched = true;
        }
        else if (_goalPositionFetched && !_pathDeveloped)
        {
            float time = 0.0f;
            Vector3 startPosition = _path.Last().GetTransform().position;
            float travelDistance = (startPosition - _goalPosition).sqrMagnitude;
            float timeStep = travelDistance / maxDroneSpeed;

            while (time < 1.0f)
            {
                Vector3 position = Vector3.Lerp(startPosition, _goalPosition, time);
                if (!RerouteForObstacleInFront(position))
                {
                    time += (1 / timeStep);
                    AddNodeToPath node = new AddNodeToPath(position, _path.Last(), this.transform);
                    _path.Add(node);
                }
                else
                    break;
            }

            if (time >= 1.0f)
                _pathDeveloped = true;
        }
        else if (_pathDeveloped && !_motionStarted)
        {
            Debug.Log("Motion Initiated");
            StartCoroutine(TravelOnPath());
            _motionStarted = true;
        }
        else if (_motionStarted && _pathRetraced)
            Debug.Log("Path Completed");
    }

    #endregion

    #region Custom Methods

    private bool RerouteForObstacleInFront(Vector3 origin)
    {
        int rotationAngle = 10;
        float detectionDistance = _gapLength/2.0f;

        var forward = (_goalPosition - droneObject.transform.position).normalized;
        var up = droneObject.transform.up;
        var right = Quaternion.AngleAxis(90,up).eulerAngles;

        Vector3 centerRayDirection = forward;
        Vector3 leftRayDirection = Quaternion.AngleAxis(-rotationAngle, up) * centerRayDirection;
        Vector3 rightRayDirection = Quaternion.AngleAxis(rotationAngle, up) * centerRayDirection;

        Vector3 topCenterRayDirection = Quaternion.AngleAxis(-rotationAngle, right) * forward;
        Vector3 topLeftRayDirection = Quaternion.AngleAxis(-rotationAngle, up) * topCenterRayDirection;
        Vector3 topRightRayDirection = Quaternion.AngleAxis(rotationAngle, up) * topCenterRayDirection;

        Vector3 bottomCenterRayDirection = Quaternion.AngleAxis(rotationAngle, right) * forward;
        Vector3 bottomLeftRayDirection = Quaternion.AngleAxis(-rotationAngle, up) * bottomCenterRayDirection;
        Vector3 bottomRightRayDirection = Quaternion.AngleAxis(rotationAngle, up) * bottomCenterRayDirection;

        Debug.DrawRay(origin, topLeftRayDirection * (detectionDistance + 50), Color.magenta);
        Debug.DrawRay(origin, topCenterRayDirection * detectionDistance, Color.cyan);
        Debug.DrawRay(origin, topRightRayDirection * (detectionDistance + 50), Color.red);

        Debug.DrawRay(origin, leftRayDirection * (detectionDistance + 50), Color.black);
        Debug.DrawRay(origin, centerRayDirection * detectionDistance, Color.blue);
        Debug.DrawRay(origin, rightRayDirection * (detectionDistance + 50), Color.green);

        Debug.DrawRay(origin, bottomLeftRayDirection * (detectionDistance + 50), Color.yellow);
        Debug.DrawRay(origin, bottomCenterRayDirection * detectionDistance, Color.white);
        Debug.DrawRay(origin, bottomRightRayDirection * (detectionDistance + 50), Color.gray);


        if (Physics.Raycast(origin, centerRayDirection, out _, detectionDistance))
        {
            Vector3 startPoint = origin;
            Vector3 rotationVector = Vector3.zero;

            var directionVector = (_goalPosition - origin).normalized;
            Vector3 endPoint = (directionVector * _gapLength) + origin;

            Debug.LogError("Obstacle Hit");

            if (!Physics.Raycast(origin, leftRayDirection, out _, detectionDistance))
                rotationVector = Quaternion.AngleAxis(-90, up) * directionVector * _gapHeight;
            else if (!Physics.Raycast(origin, rightRayDirection, out _, detectionDistance))
                rotationVector = Quaternion.AngleAxis(90, up) * directionVector * _gapHeight;
            else if (!Physics.Raycast(origin, topCenterRayDirection, out _, detectionDistance))
                rotationVector = Quaternion.AngleAxis(-90, right) * directionVector * _gapHeight;
            else if (!Physics.Raycast(origin, topLeftRayDirection, out _, detectionDistance))
            {
                rotationVector = Quaternion.AngleAxis(-90, right) * directionVector * _gapHeight;
                rotationVector = Quaternion.AngleAxis(-45, up) * rotationVector * _gapHeight;
            }
            else if (!Physics.Raycast(origin, topRightRayDirection, out _, detectionDistance))
            {
                rotationVector = Quaternion.AngleAxis(-90, right) * directionVector * _gapHeight;
                rotationVector = Quaternion.AngleAxis(45, up) * rotationVector * _gapHeight;
            }
            else if (!Physics.Raycast(origin, bottomCenterRayDirection, out _, detectionDistance))
                rotationVector = Quaternion.AngleAxis(90, right) * directionVector * _gapHeight;
            else if (!Physics.Raycast(origin, bottomLeftRayDirection, out _, detectionDistance))
            {
                rotationVector = Quaternion.AngleAxis(90, right) * directionVector * _gapHeight;
                rotationVector = Quaternion.AngleAxis(-45, up) * rotationVector * _gapHeight;
            }
            else if (!Physics.Raycast(origin, bottomRightRayDirection, out _, detectionDistance))
            {
                rotationVector = Quaternion.AngleAxis(90, right) * directionVector * _gapHeight;
                rotationVector = Quaternion.AngleAxis(45, up) * rotationVector * _gapHeight;
            }

            Vector3 midPoint = ((directionVector * (_gapLength / 2)) + origin) + rotationVector;
            float travelDistance = (startPoint - endPoint).sqrMagnitude;
            float timeStep = travelDistance / (2 * maxDroneSpeed);

            float timeCounter = 0.0f;
            while (timeCounter < timeStep)
            {
                Vector3 position = Vector3.Lerp(startPoint, midPoint, timeCounter);
                AddNodeToPath node = new AddNodeToPath(position, _path.Last(), this.transform);
                _path.Add(node);
                timeCounter += 0.1f * timeStep;
            }

            timeCounter = 0.0f;
            while (timeCounter < timeStep)
            {
                Vector3 position = Vector3.Lerp(midPoint, endPoint, timeCounter);
                AddNodeToPath node = new AddNodeToPath(position, _path.Last(), this.transform);
                _path.Add(node);
                timeCounter += 0.1f * timeStep;
            }

            return true;
        }
        else
            return false;
    }

    private IEnumerator TravelOnPath()
    {
        droneAnimator.ActivateDrone();
        int i = 0;
        var startNode = _path[0];
        var startPosition = startNode.GetTransform().position;

        var nextNode = _path[1];
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

                if (i == _path.Count)
                {
                    _pathRetraced = true;
                    StopAllCoroutines();
                    break;
                }

                startNode = nextNode;
                startPosition = startNode.GetTransform().position;

                nextNode = _path[i];
                nextPosition = nextNode.GetTransform().position;

                travelDistance = (startPosition - nextPosition).sqrMagnitude;
                timeStep = travelDistance / maxDroneSpeed;
                time = 0.0f;
            }

            yield return null;
        }
    }

    #endregion
}