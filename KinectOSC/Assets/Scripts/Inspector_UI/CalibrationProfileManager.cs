using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CalibrationProfileManager : MonoBehaviour
{
    //I don't fully understand why this is separate from the CalibrationInspector file
    [Header("CALIBRATION")]
    public GameObject[] calibrationPoints = new GameObject[6]; //ugh floor is just parent tho
    // public List<CalibrationProfile> profiles = new List<CalibrationProfile>();
    // public CalibrationProfile selectedProfile;
    BodyDataManager bodyDataManager;

    //calibration profile dropdown and selection
    /*
    public CalibrationProfile[] profiles;
    public int selectedProfileIndex = 0;
    */

    // calibration positions -- stores current best kinect pos or mapped unity pos
    // kinect is averaged incoming pos from kinect
    // unity is the position from kinect mapped to stage bounds
    public Vector3[] cPositions_kinect;
    public Vector3[] cPositions_unity;
    float distanceLimit; //just to prevent spikes in data
    // lists for smooth calibration
    List<Vector3>[] kinectReadings;
    // List<Vector3> c_0_kinectReadings = new List<Vector3>();
    // List<Vector3> c_1_kinectReadings = new List<Vector3>();
    // List<Vector3> c_2_kinectReadings = new List<Vector3>();
    // List<Vector3> c_3_kinectReadings = new List<Vector3>();
    // List<Vector3> c_4_kinectReadings = new List<Vector3>();

    //annoying, have to have redundant code
    public Vector3 c_0_pos_kinect;
    public Vector3 c_1_pos_kinect;
    public Vector3 c_2_pos_kinect;
    public Vector3 c_3_pos_kinect;
    public Vector3 c_4_pos_kinect; //right hand reaching up
    public Vector3 c_5_pos_kinect; //hands on floor
    // public Vector3 c_0_pos_unity;
    // public Vector3 c_1_pos_unity;
    // public Vector3 c_2_pos_unity;
    // public Vector3 c_3_pos_unity;
    // public Vector3 c_4_pos_unity;

    //calibrated range for mapping in BodyDataManager
    //default values used until calibration profile loaded or more specific values come in
    //in map, x values need to flip
    public float kinect_x_min = -1.25f;
    public float kinect_x_max = 1.25f;
    public float kinect_y_min = -0.75f;
    public float kinect_y_max = 1.15f;
    public float kinect_z_min = 1.25f;
    public float kinect_z_max = 3.55f;
    public float stage_x_min = -5f;
    public float stage_x_max = 5f;
    public float stage_y_min = 0f;
    public float stage_y_max = 3f;
    public float stage_z_min = -5f;
    public float stage_z_max = 5f;

    private void Awake(){
        /*
        LoadProfiles();
        */

        //get the calibration points from our parent, 
        //assumes avatarManager is on same child level as the calibrationPoints
        //**and that the calibrationPoints are the second child in the hierarchy (under floor)**
        Transform cPoints = transform.parent.GetChild(1);
        for (int i = 0; i < 5; i++)
        {
            calibrationPoints[i] = cPoints.GetChild(i).gameObject;
            //skips the floor b/c same as default vec3
        }
    }

    void Start(){
        bodyDataManager = GetComponent<BodyDataManager>();

        cPositions_kinect = new Vector3[6];
        cPositions_unity = new Vector3[6];
        kinectReadings = new List<Vector3>[6];

        //assumes calibration points are placed in a square with each at the four corners and maxreach in the middle
        stage_x_min = calibrationPoints[0].transform.localPosition.x;
        stage_x_max = calibrationPoints[1].transform.localPosition.x;
        stage_z_max = calibrationPoints[0].transform.localPosition.z;
        stage_z_max = calibrationPoints[2].transform.localPosition.z;
        stage_y_max = calibrationPoints[4].transform.localPosition.y;

        distanceLimit = Vector3.Distance(calibrationPoints[0].transform.position, calibrationPoints[2].transform.position);
        for (int i = 0; i < 6; i++)
        {
            kinectReadings[i] = new List<Vector3>();
        }
    }

    /*
    *   PROFILE SAVE/LOAD
    */
    /*
    public void SaveProfiles(){
        PlayerPrefs.SetString("Profiles", JsonUtility.ToJson(profiles));
        PlayerPrefs.Save();
    }

    public void LoadProfiles(){
        string profilesJson = PlayerPrefs.GetString("Profiles");
        if (!string.IsNullOrEmpty(profilesJson))
        {
            profiles = JsonUtility.FromJson<CalibrationProfile[]>(profilesJson);
        }
        else 
        {
            profiles = new CalibrationProfile[1];
            profiles[0] = new CalibrationProfile();
        }
    }
    */

    /*
    *   POSITION CALIBRATION
    */
    // public void Calibrate(int point, Vector3 kinectPos){
    public void Calibrate(int point){ //can't pass b/c CalibrationInspector not component...
        Vector3 kinectPos = bodyDataManager.incomingPelvisPos;
        //check to see if first reading, will be used to prevent spikes in data that would ruin average
        if (cPositions_kinect[point] != Vector3.zero)
        {
            //make sure not a weird noise spike, then average current reading
            if (Vector3.Distance(cPositions_kinect[point], kinectPos) < distanceLimit) 
            {
                kinectReadings[point].Add(kinectPos);

                //take all readings and average
                Vector3 avgPos = new Vector3(0, 0, 0);
                foreach (Vector3 reading in kinectReadings[point])
                {
                    avgPos += reading;
                }
                avgPos /= kinectReadings[point].Count;

                //update with smoothed position
                cPositions_kinect[point] = new Vector3(avgPos.x, avgPos.y, avgPos.z); //worried about reference, but idk
            }
        }
        else 
        {
            //first reading
            cPositions_kinect[point] = new Vector3(kinectPos.x, kinectPos.y, kinectPos.z);
        }

        //so dumb, getting an index out of bounds error in my inspector script because the array hasn't been initialized, so duplicating for now
        for (int i = 0; i < 5; i++)
        {
            if (i == 0) {
                c_0_pos_kinect = cPositions_kinect[0];
            } else if (i == 1) {
                c_1_pos_kinect = cPositions_kinect[1];
            } else if (i == 2) {
                c_2_pos_kinect = cPositions_kinect[2];
            } else if (i == 3) {
                c_3_pos_kinect = cPositions_kinect[3];
            } 
            // else if (i == 4) {
            //     c_4_pos_kinect = cPositions_kinect[4];
            // }
        }
    }

    public void CalibrateHands(int point){ 
        Vector3 kinectHandPos = bodyDataManager.incomingRightHandPos;
        //check to see if first reading, will be used to prevent spikes in data that would ruin average
        if (cPositions_kinect[point] != Vector3.zero)
        {
            //make sure not a weird noise spike, then average current reading
            if (Vector3.Distance(cPositions_kinect[point], kinectHandPos) < distanceLimit) 
            {
                kinectReadings[point].Add(kinectHandPos);

                //take all readings and average
                Vector3 avgPos = new Vector3(0, 0, 0);
                foreach (Vector3 reading in kinectReadings[point])
                {
                    avgPos += reading;
                }
                avgPos /= kinectReadings[point].Count;

                //update with smoothed position
                cPositions_kinect[point] = new Vector3(avgPos.x, avgPos.y, avgPos.z); //worried about reference, but idk
            }
        }
        else 
        {
            //first reading
            cPositions_kinect[point] = new Vector3(kinectHandPos.x, kinectHandPos.y, kinectHandPos.z);
        }

        //so dumb, getting an index out of bounds error in my inspector script because the array hasn't been initialized, so duplicating for now
        for (int i = 0; i < 6; i++)
        {
            if (i == 4) {
                c_4_pos_kinect = cPositions_kinect[4];
            }
            else if (i == 5) {
                c_5_pos_kinect = cPositions_kinect[5];
            }
        }
    }
}