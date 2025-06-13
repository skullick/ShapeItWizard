using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class HandTracking : MonoBehaviour
{
    public UDPReceive udpReceive;
    public GameObject[] handPoints;
    public LineDrawer lineCode;
    public Text shapeText; // UI Text for displaying shape
    private List<Vector3> points = new List<Vector3>(); // Changed to Vector3 for 3D
    public string recognizedShape = "";
    [Space]
    public Button startButton;
    public Text startButtonText;
    [Space]
    public float t = 0.1f;
    public float waitStartTime = 2.75f;
    [Space]
    public float drawAreaSize = 3f; // Size of the 3D drawing area (e.g., 10x10 units)

    private bool isHandDetectorTransmitting = false;

    private GameObject cursor;

    [System.Serializable]
    private class HandData
    {
        public string command;
        public float x;
        public float y;
        public string shape;
    }

    private void Start()
    {
        StartCoroutine(WaitForPythonLoading());
    }

    private void Update()
    {
        if (!isHandDetectorTransmitting) return;
    }

    private IEnumerator WaitForPythonLoading()
    {
        string data;
        Debug.Log("Waiting for Python app to be ready...");
        do
        {
            data = udpReceive.data;
            yield return null;
        }
        while (string.IsNullOrEmpty(data));


        yield return new WaitForSecondsRealtime(waitStartTime);

        startButton.interactable = true;
        startButtonText.text = "Play!";
        Debug.Log("Python app is ready!");

        isHandDetectorTransmitting = true;
        StartCoroutine(GetHandData());
    }

    private IEnumerator GetHandData()
    {
        while (true)
        {
            string data = udpReceive.data;
            if (!string.IsNullOrEmpty(data))
            {
                HandData jsonData = JsonUtility.FromJson<HandData>(data);
                if (jsonData == null || string.IsNullOrEmpty(jsonData.command))
                {
                    Debug.LogWarning("Invalid JSON data: " + data);
                    yield return null;
                    continue;
                }

                string command = jsonData.command;
                if (command == "draw")
                {
                    float x = jsonData.x; // Normalized x (0 to 1)
                    float y = jsonData.y; // Normalized y (0 to 1)
                    // Map to 3D space: x to z, y to x, y=0 (adjustable plane)
                    Vector3 point = new Vector3(
                        -(x - 0.5f) * drawAreaSize, // x: -5 to 5
                        0f,                        // y: fixed plane height
                        -(y - 0.5f) * drawAreaSize  // z: -5 to 5
                    );
                    points.Add(point);
                    if (cursor == null)
                    {
                        cursor = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        cursor.GetComponent<Renderer>().material.color = Color.red;
                        cursor.transform.localScale = Vector3.one * 0.2f; // Initial size
                    }
                    cursor.transform.position = point;
                    if (lineCode != null)
                    {
                        lineCode.UpdatePoints(points.ToArray());
                    }
                    else
                    {
                        Debug.LogWarning("lineCode is not assigned!");
                    }
    
                }
                else if (command == "erase")
                {
                    recognizedShape = jsonData.shape ?? "";
                    if (lineCode != null)
                    {
                        lineCode.UpdatePoints(points.ToArray(), true); // Re-render with green color
                        yield return new WaitForSeconds(0.05f); // Delay to show green color
                    }
                    else
                    {
                        Debug.LogWarning("lineCode not assigned, unable to change color");
                    }
                    ClearDrawing();
                    Debug.Log($"Erase command received, shape: {recognizedShape}");
                }
                
            }

            yield return null;
        }
    }
    public void ClearDrawing()
    {
        points.Clear();
        if (lineCode != null)
        {
            lineCode.UpdatePoints(points.ToArray());
            Debug.Log("Drawing cleared");
        }
        else
        {
            Debug.LogWarning("lineCode not assigned, unable to clear drawing");
        }

        if (shapeText != null)
        {
            shapeText.text = "";
            Debug.Log("Shape text cleared");
        }
        else
        {
            Debug.LogWarning("shapeText not assigned");
        }
    }
}