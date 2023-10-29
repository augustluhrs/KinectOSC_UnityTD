using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CalibrationProfileManager))]
public class CalibrationInspector : Editor {

    public override void OnInspectorGUI() {
        // base.OnInspectorGUI();
        CalibrationProfileManager manager = (CalibrationProfileManager)target;

        /*
        GUILayout.Label("Select Dancer Profile:");
        manager.selectedProfileIndex = EditorGUILayout.Popup(manager.selectedProfileIndex, GetProfiles(manager.profiles));

        if (GUILayout.Button("MAKE NEW CALIBRATION PROFILE"))
        {
            CalibrationProfile newProfile = ScriptableObject.CreateInstance<CalibrationProfile>();
            newProfile.dancerName = "Test";
            CalibrationProfile[] newProfiles = new CalibrationProfile[manager.profiles.Length + 1];
            manager.profiles.CopyTo(newProfiles, 0);
            newProfiles[newProfiles.Length - 1] = newProfile;
            manager.profiles = newProfiles;
        }
        */

        manager.c_0_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 0_BR_1030", manager.c_0_pos_kinect);

        if (GUILayout.Button("SET 0_BR_1030"))
        {
            manager.Calibrate(0);
        }

        manager.c_1_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 1_BR_130", manager.c_1_pos_kinect);

        if (GUILayout.Button("SET 1_BR_130"))
        {
            manager.Calibrate(1);
        }

        manager.c_2_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 2_BR_430", manager.c_2_pos_kinect);

        if (GUILayout.Button("SET 2_BR_430"))
        {
            manager.Calibrate(2);
        }

        manager.c_3_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 3_BR_730", manager.c_3_pos_kinect);

        if (GUILayout.Button("SET 3_BR_730"))
        {
            manager.Calibrate(3);
        }

        manager.c_4_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 4_CEILING_MAXREACH", manager.c_4_pos_kinect);

        if (GUILayout.Button("SET 4_CEILING_MAXREACH RIGHT HAND"))
        {
            manager.CalibrateHands(4);
        }

        manager.c_5_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at CENTER FLOOR", manager.c_5_pos_kinect);

        if (GUILayout.Button("SET FLOOR HANDS"))
        {
            manager.CalibrateHands(5);
        }

        /*
        if (GUILayout.Button("CLEAR PROFILES"))
        {
            CalibrationProfile[] newProfiles = new CalibrationProfile[0];
            manager.profiles = newProfiles;
        }
        */

        DrawDefaultInspector();
    }

    /*
    private string[] GetProfiles(CalibrationProfile[] profiles)
    {
        string[] profileNames = new string[profiles.Length];
        for (int i = 0;  i < profiles.Length; i++)
        {
            profileNames[i] = profiles[i].dancerName;
        } 
        return profileNames;
    }
    */
}
