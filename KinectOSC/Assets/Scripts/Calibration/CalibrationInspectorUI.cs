using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/*
*   This script creates the inspector buttons and calibration UI
*   uses the functions created in the CalibrationProfileManager
*   this isn't a component, so can't access GameObject stuff, have to do that in the manager
*/

[CustomEditor(typeof(CalibrationProfileManager))]
public class CalibrationInspectorUI : Editor {

    public override void OnInspectorGUI() {
        // base.OnInspectorGUI();
        CalibrationProfileManager manager = (CalibrationProfileManager)target;

        //default color for UI
        Color defaultColor = GUI.backgroundColor;
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

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();    
        EditorGUILayout.LabelField("GET CALIBRATION POINT POSITIONS:");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("note: \"clear\" buttons will not show reset values until next \"set\" button press");
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.cyan;
        manager.c_0_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 0_BR_1030", manager.c_0_pos_kinect);
        
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SET 0_BR_1030"))
        {
            manager.Calibrate(0);
        }
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("CLEAR 0 READINGS"))
        {
            manager.ClearReadings(0);
        }
        GUILayout.EndHorizontal();
        
        GUI.backgroundColor = Color.cyan;
        manager.c_1_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 1_BR_130", manager.c_1_pos_kinect);
        
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SET 1_BR_130"))
        {
            manager.Calibrate(1);
        }
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("CLEAR 1 READINGS"))
        {
            manager.ClearReadings(1);
        }
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.cyan;
        manager.c_2_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 2_BR_430", manager.c_2_pos_kinect);
        
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SET 2_BR_430"))
        {
            manager.Calibrate(2);
        }
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("CLEAR 2 READINGS"))
        {
            manager.ClearReadings(2);
        }
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.cyan;    
        manager.c_3_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 3_BR_730", manager.c_3_pos_kinect);
        
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SET 3_BR_730"))
        {
            manager.Calibrate(3);
        }
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("CLEAR 3 READINGS"))
        {
            manager.ClearReadings(3);
        }
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.cyan;
        manager.c_4_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at 4_CEILING_MAXREACH", manager.c_4_pos_kinect);
        
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SET 4_REACH_RIGHT"))
        {
            manager.CalibrateHands(4);
        }
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("CLEAR 4 READINGS"))
        {
            manager.ClearReadings(4);
        }
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.cyan;
        manager.c_5_pos_kinect = EditorGUILayout.Vector3Field("Kinect position at CENTER FLOOR", manager.c_5_pos_kinect);
        
        GUILayout.BeginHorizontal();
        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("SET FLOOR HANDS"))
        {
            manager.CalibrateHands(5);
        }
        GUI.backgroundColor = Color.yellow;
        if (GUILayout.Button("CLEAR 5 READINGS"))
        {
            manager.ClearReadings(5);
        }
        GUILayout.EndHorizontal();

        EditorGUILayout.LabelField("\n");
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();    
        EditorGUILayout.LabelField("FINISH CALIBRATION:");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUI.backgroundColor = Color.magenta;
        if (GUILayout.Button("CALIBRATE"))
        {
            manager.FinishCalibration();
        }

        /*
        if (GUILayout.Button("CLEAR PROFILES"))
        {
            CalibrationProfile[] newProfiles = new CalibrationProfile[0];
            manager.profiles = newProfiles;
        }
        */
        GUI.backgroundColor = defaultColor;

        EditorGUILayout.LabelField("\n");
        EditorGUILayout.LabelField("\n");
        EditorGUILayout.LabelField("\n");
        EditorGUILayout.LabelField("\n");

        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();    
        EditorGUILayout.LabelField("BELOW JUST FOR CHECKING VALUES:");
        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

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
