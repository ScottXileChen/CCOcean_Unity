using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fix : MonoBehaviour
{
    public Transform mTransform;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = mTransform.position;
    }
}
