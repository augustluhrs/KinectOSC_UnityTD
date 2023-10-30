using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
/*
* takes all incoming OSC messages from TD,
* sorts by address, assigns position and rotation to manager gameobjects in hierarchy
* optional detection features with trigger/range/threshold settings
*
*
* using OSC Jack -- https://github.com/keijiro/OscJack
*/

public class BodyDataManager : MonoBehaviour
{
    // y of the avatar manager should be set by height calibration TODO
    // public float floorScale = 7f; //however we want to scale the incoming position data
    // public float heightScale = 1f;
    
    [Header("DEBUG/TESTING")]
    //test variables
    public bool showDemoCubes = true;
    public GameObject demoCube;
    public float cubeScale = 0.1f;

    [Header("JOINT REFERENCES")]
    public GameObject[] jointTrackingSlots; //to assign gameobjects whose transforms we want to match these joints
    //need to store a reference to all joint children of our avatar
    //can be assigned to meshes or avatar joints (todo, check anim stuff)
    //always using local Position because assumes nested hierarchy, both for joints and avatar to avatarManager
    // **ACTUALLY JK, Kinect sends "world position" relative to sensor so still local position relative to manager, but no longer nested
    GameObject[] joints;
    string[] jointNames; //have to do this because can't check gameObject.name in the message threads
    Vector3[] jointPositions; //same as above, localposition vectors
    Vector3[] mappedJointPositions; //after calibration, testing diff between jointPositions

    // need rotations
    //Quaternion[] jointRotations;
    int jointIndex = 0; //dumb but idk, for now

    [Header("OSC DATA AND CALIBRATION")]
    OscServer _server;
    public Vector3 incomingPelvisPos = new Vector3(0, 0, 0); //hmm
    public Vector3 incomingRightHandPos = new Vector3(0, 0, 0); //hmm
    public bool isCalibrating = true;
    CalibrationProfileManager calib;

    void Awake(){
        // calibrationProfileManager = GetComponent<CalibrationProfileManager>();
        calib = GetComponent<CalibrationProfileManager>(); //shortening just for map function legibility

        //need to set up the joints before the server tries to access them
        //checks for joint label in the children of the avatar manager
        joints = new GameObject[32];
        jointNames = new string[32];
        jointPositions = new Vector3[32];
        mappedJointPositions = new Vector3[32];
        IterateTransformHierarchy(transform);

        //uncheck in inspector if don't want cubes to show
        if (showDemoCubes){
            AddDemoCubes(); 
        }
    }

    void IterateTransformHierarchy(Transform parentTransform){
        //thanks chat-GPT
        foreach (Transform childTransform in parentTransform)
        {
            // add the game object to the joints array so that we can update position (or create mesh component)
            // Debug.Log("Child name: " + childTransform.name);

            //gotta be a better way, guess I should just use a list, but w/e
            if (childTransform.gameObject.tag == "joint"){
                joints[jointIndex] = childTransform.gameObject;
                jointNames[jointIndex] = childTransform.gameObject.name;
                jointPositions[jointIndex] = new Vector3(0, 0, 0); //cant remember the .zero way
                jointIndex++;
                
                // If the child has more children, recursively iterate through them
                if (childTransform.childCount > 0)
                {
                    IterateTransformHierarchy(childTransform);
                }
            }
        }
    }

    void AddDemoCubes(){
        foreach (GameObject joint in joints){
            //add a demo cube for visualizing the joints before we connect these to an avatar
            //now using prefab on the Avatar Layer
            GameObject cubeObject = Instantiate(demoCube, joint.transform.position, joint.transform.rotation);
            cubeObject.transform.parent = joint.transform;
            cubeObject.transform.localScale = new Vector3(cubeScale, cubeScale, cubeScale);
        }
    }

    void Start()
    {
        _server = new OscServer(9000); // Port number

        _server.MessageDispatcher.AddCallback(
            "", // OSC address --empty is all incoming messages
            (string address, OscDataHandle data) => {
                //TODO should see if there's a more optimized way of doing this that decreases latency
                
                //see incoming data and and addresses
                // Debug.Log(string.Format("({0}, {1})",
                //     address,
                //     data.GetElementAsFloat(0)));

                //parse the OSC message by body joint and parameter 
                //ex. if msg is "/p1/pelvis:tx:0.12", joint label = pelvis, param = tx, val = 0.12
                string[] splitAddy = address.Split('/');
                string label = "";
                string param = "";
                float val = 0f;
                bool isBody = false;
               
                if (splitAddy[1] == "p1"){ //make sure its a body tracking message
                    if (splitAddy[2] != "id") { //don't need for now
                        isBody = true; //just don't want to have the rest of this stuff nested in here
                        string[] parts = splitAddy[2].Split(':');
                        
                        label = parts[0];
                        param = parts[1];
                        val = data.GetElementAsFloat(0);
                        // foreach (string part in parts){
                        //     Debug.Log(string.Format("({0}, {1})",
                        //     "part",
                        //     part));
                        // } 
                    }
                }

                if (!isBody) {return;}
                //if past this point, we have a joint label, param, and val

                //
                /*
                if (param == "tx" || param == "tz"){
                    val *= floorScale;
                } else if (param == "ty"){
                    val *= heightScale;
                } else {
                    Debug.Log("wtf is this");
                    Debug.Log(param);
                }
                */

                //have to store all these as separate references because can't check gameObjects in message threads
                //update the position of the gameObjects accordingly
                for (int i = 0; i < 32; i++){
                    if (jointNames[i] == label){
                        Vector3 jPos = jointPositions[i];
                        if (param == "tx"){
                            jPos = new Vector3 (val, jPos.y, jPos.z);
                        } else if (param == "ty") {
                            jPos = new Vector3 (jPos.x, val, jPos.z);
                        } else if (param == "tz") {
                            jPos = new Vector3 (jPos.x, jPos.y, val);
                        }
                        jointPositions[i] = jPos;
                    }
                }

                //update pre-mapping for calibration
                if (isCalibrating)
                {
                    incomingPelvisPos = jointPositions[0];
                    incomingRightHandPos = jointPositions[15]; //TODO double check
                } else
                {
                    // //map the positions to the calibrated scale
                    // for (int i = 0; i < 32; i++)
                    // {
                    //     Vector3 jointPos = jointPositions[i];
                    //     Debug.Log(jointPos);
                    //     float mapped_x, mapped_y, mapped_z;
                    //     mapped_x = Map(jointPos.x, calib.kinect_x_min, calib.kinect_x_max, calib.stage_x_min, calib.stage_x_max);
                    //     mapped_y = Map(jointPos.y, calib.kinect_y_min, calib.kinect_y_max, calib.stage_y_min, calib.stage_y_max);
                    //     mapped_z = Map(jointPos.z, calib.kinect_z_min, calib.kinect_z_max, calib.stage_z_min, calib.stage_z_max);
                    //     mappedJointPositions[i] = new Vector3(mapped_x, mapped_y, mapped_z);
                    //     Debug.Log(mappedJointPositions[i]);
                    // }
                }    
            }
        );
    }

     // Update is called once per frame
    void Update()
    {
        //update gameObject transforms -- TODO check for optimization
        if (isCalibrating)
        {
            //display incoming kinect positions without scaling
            for(int i = 0; i < 32; i++){
                joints[i].transform.localPosition = jointPositions[i];
            }
        } else 
        {
            //map the positions to the calibrated scale
            for (int i = 0; i < 32; i++)
            {
                Debug.Log(i);
                Vector3 jointPos = jointPositions[i];
                Debug.Log(jointPos);
                float mapped_x, mapped_y, mapped_z;
                mapped_x = Map(jointPos.x, calib.kinect_x_min, calib.kinect_x_max, calib.stage_x_min, calib.stage_x_max);
                mapped_y = Map(jointPos.y, calib.kinect_y_min, calib.kinect_y_max, calib.stage_y_min, calib.stage_y_max);
                mapped_z = Map(jointPos.z, calib.kinect_z_min, calib.kinect_z_max, calib.stage_z_min, calib.stage_z_max);
                mappedJointPositions[i] = new Vector3(mapped_x, mapped_y, mapped_z);
                Debug.Log(mappedJointPositions[i]);
            }
            for(int i = 0; i < 32; i++){
                joints[i].transform.localPosition = mappedJointPositions[i];
            }
        }
        

        //feature tests:
    }

    // Map function to remap a value from one range to another
    float Map(float value, float fromMin, float fromMax, float toMin, float toMax)
    {
        // Ensure the value is within the source range
        // value = Mathf.Clamp(value, fromMin, fromMax); //need this?

        // Calculate the mapped value
        return (value - fromMin) / (fromMax - fromMin) * (toMax - toMin) + toMin;
    }

    void OnDestroy()
    {
        _server?.Dispose();
        _server = null;
    }
}
