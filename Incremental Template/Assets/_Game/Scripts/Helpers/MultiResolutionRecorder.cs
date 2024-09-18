using System.Collections;
using System.Collections.Generic;
using UnityEditor.Recorder;
using UnityEditor.Recorder.Input;
using UnityEngine;

public class MultiResolutionRecorder : MonoBehaviour
{
    public KeyCode recordKey = KeyCode.P;

    public float waitTimeBetweenRecordings = 0.1f; // Bekleme süresi (yarım saniye)
    private RecorderController recorderController1;
    private RecorderController recorderController2;
    private RecorderController recorderController3;

    void Start()
    {
        recorderController1 = SetupRecorder(1242, 2208, "Record_5.5inch");
        recorderController2 = SetupRecorder(2048, 2732, "Record_12.9inch");
        recorderController3 = SetupRecorder(1290, 2796, "Record_6.7inch");
    }

    void Update()
    {
        if (Input.GetKeyDown(recordKey))
        {
            StartCoroutine(StartRecordingWithDelay());
        }

        if (Input.GetKeyUp(recordKey))
        {
            StopRecording();
        }
    }

    RecorderController SetupRecorder(int width, int height, string outputName)
    {
        var settings = ScriptableObject.CreateInstance<ImageRecorderSettings>();
        settings.name = outputName;
        settings.Enabled = true;
        settings.OutputFormat = ImageRecorderSettings.ImageRecorderOutputFormat.PNG;
        settings.CaptureAlpha = false;
        string timeStamp = System.DateTime.Now.ToString("HHmmss");
        settings.OutputFile = "Recordings/" + outputName + "_" + timeStamp;
        settings.RecordMode = RecordMode.SingleFrame;
        settings.imageInputSettings = new GameViewInputSettings
        {
            OutputWidth = width,
            OutputHeight = height
        };

        var controllerSettings = ScriptableObject.CreateInstance<RecorderControllerSettings>();
        controllerSettings.AddRecorderSettings(settings);
        var controller = new RecorderController(controllerSettings);

        return controller;
    }

    IEnumerator StartRecordingWithDelay()
    {
        recorderController1.PrepareRecording();
        recorderController1.StartRecording();
        yield return new WaitForSeconds(waitTimeBetweenRecordings);
        recorderController1.StopRecording();

        recorderController2.PrepareRecording();
        recorderController2.StartRecording();
        yield return new WaitForSeconds(waitTimeBetweenRecordings);
        recorderController2.StopRecording();

        recorderController3.PrepareRecording();
        recorderController3.StartRecording();
        yield return new WaitForSeconds(waitTimeBetweenRecordings);
        recorderController3.StopRecording();
    }
    
    void StopRecording()
    {
      
    }
}
