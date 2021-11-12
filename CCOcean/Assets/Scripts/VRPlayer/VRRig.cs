using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;
    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint(trackingPositionOffset);
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class VRRig : MonoBehaviour
{
    [SerializeField]
    private float turnSmoothness = 5f;
    [SerializeField]
    private VRMap head;
    [SerializeField]
    private VRMap leftHand;
    [SerializeField]
    private VRMap rightHand;
    [SerializeField]
    private Transform headConstraint;

    private Vector3 headBodyOffest;

    private void Start()
    {
        headBodyOffest = transform.position - headConstraint.position;
    }

    private void Update()
    {
        transform.position = headConstraint.position + headBodyOffest;
        transform.forward = Vector3.Lerp(transform.forward,
            Vector3.ProjectOnPlane(headConstraint.up, Vector3.up).normalized, Time.deltaTime * turnSmoothness);

        head.Map();
        leftHand.Map();
        rightHand.Map();
    }
}
