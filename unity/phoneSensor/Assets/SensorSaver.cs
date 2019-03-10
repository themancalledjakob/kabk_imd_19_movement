using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

public class SensorSaver : MonoBehaviour {

    // this is the data we want to save
    [Serializable]
    public class SensorData
    {
        public string timeStamp;
        public Vector3 rotationRate;
        public Vector3 userAcceleration;
        public Vector3 deviceAcceleration;
        public Vector3 gravity;
        public float magneticHeading;
        public float trueHeading;
        public float latitude;
        public float longitude;
        public float altitude;
    }

    public Camera cam;
    public Transform example;

    System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
    long lastSave;
    public long saveIntervalMillis = 1000;

    string savePath;

    List<SensorData> sensorDataList;

    // Use this for initialization
    void Start () {
        sensorDataList = new List<SensorData>();
        cam.backgroundColor = Color.white;

        stopwatch.Start ();
        lastSave = stopwatch.ElapsedMilliseconds;

        Input.compass.enabled = true;
        Input.gyro.enabled = true;
        Input.location.Start();
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        // take default data path for saving your data
        savePath = Application.persistentDataPath;

        // you could also try to create a special directory,
        // but this might not work on your phone
        // System.IO.Directory.CreateDirectory(savePath);

    }

    // Update is called once per frame
    void Update () {

        cam.backgroundColor = Color.black;

        // transform the example cube, just to show that it works
        example.localScale = Input.gyro.userAcceleration + new Vector3(1, 1, 1);

        long now = stopwatch.ElapsedMilliseconds;
        if (now > lastSave + saveIntervalMillis) {    
            lastSave = now;

            SensorData sensorData = new SensorData();


            sensorData.timeStamp = DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:fff");
            sensorData.rotationRate = Input.gyro.rotationRate;
            sensorData.userAcceleration = Input.gyro.userAcceleration;
            sensorData.gravity = Input.gyro.gravity;
            sensorData.deviceAcceleration = Input.acceleration;
            sensorData.magneticHeading = Input.compass.magneticHeading;
            sensorData.trueHeading = Input.compass.trueHeading;
            sensorData.longitude = Input.location.lastData.longitude;
            sensorData.latitude = Input.location.lastData.latitude;
            sensorData.altitude = Input.location.lastData.altitude;


            sensorDataList.Add(sensorData);
            SaveData(sensorData);

            Debug.Log(sensorData.timeStamp);

            cam.backgroundColor = Color.white;
        }
    }


    void OnGUI()
    {
        // Make a background box
        GUI.Box(new Rect(10, 10, 440, 60), "Where is the data?");

        GUI.Label(new Rect(30, 40, 400, 20), savePath);

    }

    void SaveData(SensorData sensorData)
    {
        string fileName = "sensorData_" + DateTime.Now.ToString("yyyy-MM-dd-HH:mm:ss:fff") + ".json";
        string fullDestination = Path.Combine(savePath, fileName);

        File.WriteAllText(fullDestination, JsonUtility.ToJson(sensorData));
    }
}

