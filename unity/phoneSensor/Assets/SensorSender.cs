using UnityEngine;
using System.Collections;
using System;
using System.Net;
using System.Collections.Generic;
using UnityOSC;

public class SensorSender : MonoBehaviour {

	public Camera cam;

	public Transform example;

	System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
	long lastSend;
	public long sendInterval = 30;

	OSCClient osc;

	IPAddress ipaddress;

    public string receiverIP = "192.168.1.255";
    public int receiverPort = 8050;

    // Use this for initialization
    void Start () {
        osc = new OSCClient (IPAddress.Parse (receiverIP), receiverPort);
		cam.backgroundColor = Color.black;

		stopwatch.Start ();
		lastSend = stopwatch.ElapsedMilliseconds;

		Input.compass.enabled = true;
		Input.gyro.enabled = true;
		Input.location.Start();

		Screen.sleepTimeout = SleepTimeout.NeverSleep;
	}

	// Update is called once per frame
	void Update () {

        // transform the example cube, just to show that it works
        example.localScale = Input.gyro.userAcceleration + new Vector3(1, 1, 1);

        long now = stopwatch.ElapsedMilliseconds;
		if (now > lastSend + sendInterval) {
			
			lastSend = now;

			example.localRotation = Input.gyro.attitude;

			Send ("/gyro/attitude", Input.gyro.attitude);
			Send ("/gyro/rotationRate", Input.gyro.rotationRate);
			Send ("/gyro/acceleration", Input.gyro.userAcceleration);
			Send ("/gyro/gravity", Input.gyro.gravity);
			Send ("/compass/mag", Input.compass.magneticHeading);
			Send ("/compass/true", Input.compass.trueHeading);
			Send ("/compass/raw", Input.compass.rawVector);
			Send ("/compass/accuracy", Input.compass.headingAccuracy);
			Send ("/acceleration", Input.acceleration);
			Send ("/touchCount", Input.touchCount);
		}
	}

	void Send(string address, Vector3 v) {
		Send (address, new List<object> { v.x, v.y, v.z });
	}

	void Send(string address, Vector4 v) {
		Send (address, new List<object> { v.x, v.y, v.z, v.w });
	}

	void Send(string address, Quaternion q) {
		Send (address, new List<object> { q.x, q.y, q.z, q.w });
	}

	void Send(string address, float f) {
		Send (address, new List<object> { f });
	}

	void Send(string address, int i) {
		Send (address, new List<object> { i });
	}

	void Send(string address, List<object> args) {
		{
			OSCMessage msg = new OSCMessage (address);
			for (var i = 0; i < args.Count; i++) {
				object arg = args [i];
				msg.Append (arg);
			}
			osc.Send (msg);
		}
	}

	Vector3 smoothed(Vector3 v1, Vector3 v2, float factor) {
		return v1 * factor + v2 * (1.0F - factor);
	}
}

