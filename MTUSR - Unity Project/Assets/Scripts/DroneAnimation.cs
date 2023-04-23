using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneAnimation : MonoBehaviour
{
    #region Variables

    [SerializeField] private List<GameObject> propellerList;
    private bool _activatePropellers;

    #endregion

    #region Main Methods

    private void Update()
    {
        if (_activatePropellers)
        {
            foreach (var propeller in propellerList)
            {
                var currentRotation = propeller.transform.rotation.eulerAngles;
                currentRotation.y += 10;
                propeller.transform.rotation = Quaternion.Euler(currentRotation);
            }
        }
    }

    #endregion

    #region Custom Methods

    public void ActivateDrone()
    {
        StartCoroutine(ActivatePropeller());
    }

    public void DeactivateDrone()
    {
        StopAllCoroutines();
    }
    
    private IEnumerator ActivatePropeller()
    {
        while (true)
        {
            foreach (var propeller in propellerList)
            {
                propeller.transform.Rotate(Vector3.up, 5.0f);
            }
            yield return null;
        }
    }
    #endregion
}