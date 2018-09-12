using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SineRider : MonoBehaviour
{
    public float frequency = 1.0f;
    public float height = 1.0f;

    void LateUpdate()
    {
        transform.localPosition = new Vector3(0.0f, Mathf.Cos(Time.time * frequency) * height, 0.0f);
    }
}