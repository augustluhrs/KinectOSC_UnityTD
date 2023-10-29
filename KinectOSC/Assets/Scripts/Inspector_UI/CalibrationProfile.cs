using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ProfilePresets", menuName = "KinectOSC/ProfilePresets", order = 0)]
[System.Serializable]
public class CalibrationProfile : ScriptableObject {
    
    public string dancerName;
    public Vector3 pos0;
    public Vector3 pos1;
    public Vector3 pos2;
    public Vector3 pos3;
    public Vector3 pos4;
    public Vector3 pos5;
    public float wingspan;

    //eventually, could have dropdowns for settings, but overkill for now
    // i.e. kinect position, kinect distance, kinect height
    // then auto append to dancer name for profile name
}
