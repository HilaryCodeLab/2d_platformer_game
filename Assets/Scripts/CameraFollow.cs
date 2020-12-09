using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;

    public float maxX = 10f;
    public float minX = -10f;

    public float maxY = 10f;
    public float minY = -10f;

    Vector3 finalVector;

    void Start()
    {
        if (target.position.x < minX)
        {
            finalVector.x = minX;
        }
        if (target.position.x > maxX)
        {
            finalVector.x = maxX;
        }
        if (target.position.y < minY)
        {
            finalVector.y = minY;
        }
        if (target.position.y > maxY)
        {
            finalVector.y = maxY;
        }
        finalVector.z = transform.position.z;
        transform.position = finalVector;
    }

    void LateUpdate()//called once per frame after all update functions
    {
        if(target.position.x > minX && target.position.x < maxX)
        {
            finalVector.x = target.position.x;
        }
        if(target.position.y > minY && target.position.y < maxY)
        {
            finalVector.y = target.position.y;
        }
        transform.position = finalVector;
    }
}
