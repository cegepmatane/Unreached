using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateComponentAfterX : MonoBehaviour
{
    public float timeUntilDeactivating = 0.2f;
    public Behaviour componentToDeactivate;

    void Start()
    {
        StartCoroutine(DeactivateAfterX());
    }

    IEnumerator DeactivateAfterX()
    {
        yield return new WaitForSeconds(timeUntilDeactivating);
        componentToDeactivate.enabled = false;
    }
}
