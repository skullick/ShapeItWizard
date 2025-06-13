using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IGameManager : MonoBehaviour
{
    public static IGameManager Instance { get; private set; }
    public ShapeSpawner shapeSpawner;
    public GameObject mainCamera;
    public UIManager uiManager;
    public HandTracking handTracking;

    private void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame()
    {
        IScoreManager.Instance.ResetScore();
        ILivesManager.Instance.ResetLife();
        mainCamera.transform.Rotate(Vector3.left, 90);
        uiManager.ShowGameUI();
        shapeSpawner.StartShapeDrop();
        if (handTracking != null)
        {
            handTracking.ClearDrawing();
            Debug.Log("Drawing cleared on StartGame");
        }
        else
        {
            Debug.LogWarning("HandTracking not assigned, unable to clear drawing");
        }
    }

    public void GameOver()
    {
        IScoreManager.Instance.UpdateHighScore();
        mainCamera.transform.Rotate(Vector3.left, -90);
        uiManager.ShowGameOverUI();
        shapeSpawner.StopShapeDrop();
        shapeSpawner.DestroyAllShapes();
        if (handTracking != null)
        {
            handTracking.ClearDrawing();
            Debug.Log("Drawing cleared on GameOver");
        }
        
    }

    public void MainMenu()
    {
        uiManager.ShowMainMenuUI();
    }

    public void PauseMenu()
    {
        uiManager.ShowPauseUI();
    }

}
