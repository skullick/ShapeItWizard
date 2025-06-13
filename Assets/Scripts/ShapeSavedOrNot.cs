using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShapeSavedOrNot : MonoBehaviour
{
    /// <summary>
    /// Shape saved if stays in this trigger box for 1 second.
    /// Shape broken if exits from box.
    /// </summary>
    /// 
    private float timer = 0f;
    private float timerEnd = 1f;

    public AudioSource playShapeSuccess;
    public AudioSource playShapeCrack1;

    private HandTracking handTracking; // Reference to HandTracking

    private void Start()
    {
        handTracking = FindObjectOfType<HandTracking>(); // Get HandTracking instance
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Shape"))
        {
            string shapeNameWithVariant = other.gameObject.name; // e.g., "Triangle_normal"
            string shapeName = shapeNameWithVariant.Split('_')[0]; // Extract shape name (e.g., "CirclePrefab" → "Circle")
            Debug.Log("Shape recognized: " + handTracking.recognizedShape + ", Expected: " + shapeName);
            if (handTracking != null && !string.IsNullOrEmpty(handTracking.recognizedShape) && handTracking.recognizedShape == shapeName)
            {
                Destroy(other.gameObject);
                playShapeSuccess.Play();
                IScoreManager.Instance.IncreaseScore();
                handTracking.recognizedShape = ""; // Reset recognized shape
            }
            handTracking.recognizedShape = "";
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Shape"))
        {
            Destroy(other.gameObject);
            timer = 0f; // permits only one Shape at a time
            playShapeCrack1.Play();
            ILivesManager.Instance.RemoveLife();
        }
    }

}


