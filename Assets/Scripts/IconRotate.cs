using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IconRotate : MonoBehaviour
{
    private Transform iconTransform;

    [SerializeField]
    private float rotationSpeed = 18f;
    [SerializeField]
    private float maxAngleY = 30f;
    void Start()
    {
        iconTransform = transform;
    }

    // Update is called once per frame
    void Update()
    {
        float angle = Mathf.PingPong(Time.time * rotationSpeed, maxAngleY*2) - maxAngleY;
        iconTransform.localEulerAngles = new Vector3(0, angle, 0);
    }
}
