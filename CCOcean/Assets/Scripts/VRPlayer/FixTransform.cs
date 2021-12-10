using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixTransform : MonoBehaviour
{
    [SerializeField]
    private Transform fixTransform = null;

    private void Update()
    {
        transform.position = fixTransform.position;
    }
}
