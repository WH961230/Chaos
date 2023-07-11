using System;
using UnityEngine;
using UnityEngine.Events;

public class VirtualCameraTargetController : MonoBehaviour {
    public Transform FarTarget;
    public Vector3 offPos;
    
    private GameObject localPlayerGameObj;
    public GameObject LocalPlayerGameObj {
        get {
            if (localPlayerGameObj == null) {
                localPlayerGameObj = GameObject.FindWithTag("Player");
            }
            
            return localPlayerGameObj;
        }
    }
    
    //相机第一人称视角实现
    private float xRotate = 0.0f;
    private float yRotate = 0.0f;
    public float xRotateMinLimit = 100f;
    public float xRotateMaxLimit = 100f;

    private bool isResetDirection;

    public static VirtualCameraTargetController instance;
    public void Awake() {
        instance = this;
    }

    void Update() {
        xRotate -= Input.GetAxis("Mouse Y");
        xRotate = Mathf.Clamp(xRotate, xRotateMinLimit, xRotateMaxLimit);
        yRotate += Input.GetAxis("Mouse X");
    }

    void FixedUpdate() {
        if (LocalPlayerGameObj != null) {
            if (!isResetDirection) {
                transform.rotation = LocalPlayerGameObj.transform.rotation;
                isResetDirection = true;
            }
            transform.position = LocalPlayerGameObj.transform.position + offPos;
            transform.rotation = Quaternion.Euler(xRotate, yRotate, 0);
        }
    }
}