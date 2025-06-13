using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeSpawner : MonoBehaviour
{
    public GameObject[] shapePrefabs;
    public int ShapeCount;
    private Coroutine shapeDropCoroutine;

    private float maxXPos = 4f; // boundaries whithin Shape must spawn
    private float minXPos = -4f;
    private float maxZPos = 4f;
    private float minZPos = 0.5f;

    private float xPos;
    private float zPos;


    public void StartShapeDrop()
    {
        shapeDropCoroutine = StartCoroutine(ShapeDrop());
    }

    public void StopShapeDrop()
    {
        StopCoroutine(shapeDropCoroutine);
    }

    public void DestroyAllShapes()
    {
        GameObject[] shapes = GameObject.FindGameObjectsWithTag("Shape");
        foreach (GameObject shape in shapes)
            GameObject.Destroy(shape);
    }

    IEnumerator ShapeDrop()
    {
        Instantiate(shapePrefabs[0], new Vector3(xPos, 70, zPos), Quaternion.identity); // instantiate first Shape (closer)
        while (true)
        {
            xPos = Random.Range(minXPos, maxXPos);
            zPos = Random.Range(minZPos, maxZPos);
            int randomIndex = Random.Range(0, shapePrefabs.Length);
            GameObject shape = Instantiate(shapePrefabs[randomIndex], new Vector3(xPos, 90, zPos), Quaternion.identity);
            Rigidbody rb = shape.GetComponent<Rigidbody>();
            if (rb != null)
            {
                // Randomize spin axis and direction
                Vector3 spinAxis = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f),
                    Random.Range(-1f, 1f)
                ).normalized; // Normalized to ensure consistent speed
                rb.angularVelocity = spinAxis * 180f * Mathf.Deg2Rad; // Convert to radians/sec
            }
            yield return new WaitForSeconds(5);
        }
    }

}
