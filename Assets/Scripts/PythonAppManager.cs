using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
public class PythonAppManager : MonoBehaviour
{
    private const string PythonAppName = "/PythonApp/dist/ShapeItWizard/ShapeItWizard";

    private static Process PythonProcess;



    [RuntimeInitializeOnLoadMethod]
    private static void RunOnStart()
    {

        ProcessStartInfo PythonInfo = new ProcessStartInfo();
        PythonInfo.FileName = Application.dataPath + PythonAppName;

        PythonProcess = Process.Start(PythonInfo);

        Application.quitting += () =>
            { if (!PythonProcess.HasExited) PythonProcess.Kill(); };
        UnityEngine.Debug.Log(Application.dataPath);
        System.Diagnostics.Debug.WriteLine(Application.dataPath);
    }
}
