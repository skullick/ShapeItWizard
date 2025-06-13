using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotation : MonoBehaviour
{

    public float turningRate = 30f; // Maximum turn rate in degrees per second.
    private Quaternion targetRotation; // Rotation to blend towards.

    private void Awake()
    {
        targetRotation = Quaternion.Euler(Vector3.up);
    }


    public void SetBlendedEulerAngles(Vector3 angles) // Call this to turn object smoothly.
    {
        targetRotation = Quaternion.Euler(angles);
    }

    public void CamLookUp()
    {
        targetRotation = Quaternion.Euler(Vector3.up);
    }

    public void CamLookDown()
    {
        targetRotation = Quaternion.Euler(Vector3.back);
    }

    private void Update()
    {
        // Turn towards our target rotation.
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, turningRate * Time.deltaTime);
    }

    


}


