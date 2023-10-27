using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using OscJack;
/*
* takes all incoming OSC messages from TD,
* sorts by address, assigns position and rotation to manager gameobjects in hierarchy
* optional detection features with trigger/range/threshold settings
*
* TODO -- calibration with center, four corners, and height
*
* using OSC Jack -- https://github.com/keijiro/OscJack
*/

public class BodyDataManager_OSCfromTD : MonoBehaviour
{
    //calibration
    // y of the avatar manager should be set by height calibration TODO
    public float floorScale = 7f; //however we want to scale the incoming position data
    public float heightScale = 1f;

    //need to store a reference to all children of our avatar
    //can be assigned to meshes or avatar joints (todo, check anim stuff)
    //always using local Position because assumes nested hierarchy, both for joints and avatar to avatarManager
    // **ACTUALLY JK, Kinect sends "world position" relative to sensor so still local position relative to manager, but no longer nested
    public GameObject[] joints;
    string[] jointNames; //have to do this because can't check gameObject.name in the message threads
    Vector3[] jointPositions; //same as above, localposition vectors
    // need rotations
    //Quaternion[] jointRotations;
    
    int jointIndex = 0; //dumb but idk, for now

    //test variables
    public float cubeScale = 0.1f;

    OscServer _server;

    void Awake(){
        //need to set up the joints before the server tries to access them
        //assumes pelvis and nested joints are the only gameobjects in the avatar manager
        joints = new GameObject[32];
        jointNames = new string[32];
        jointPositions = new Vector3[32];
        IterateTransformHierarchy(transform);
        AddDemoCubes(); //comment out if using avatar
    }

    void IterateTransformHierarchy(Transform parentTransform){
        //thanks chat-GPT
        foreach (Transform childTransform in parentTransform)
        {
            // add the game object to the joints array so that we can update position (or create mesh component)
            Debug.Log("Child name: " + childTransform.name);
            Debug.Log("Child go name: " + childTransform.gameObject.name);

            //gotta be a better way, guess I should just use a list, but idk
            joints[jointIndex] = childTransform.gameObject;
            jointNames[jointIndex] = childTransform.gameObject.name;
            jointPositions[jointIndex] = new Vector3(0, 0, 0); //cant remember the .zero way
            jointIndex++;
             
            Debug.Log(jointIndex);
            
            // If the child has more children, recursively iterate through them
            if (childTransform.childCount > 0)
            {
                IterateTransformHierarchy(childTransform);
            }
        }
    }

    void AddDemoCubes(){
        foreach (GameObject joint in joints){
            //add a demo cube for visualizing the joints before we connect these to an avatar
            GameObject cubeObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cubeObject.transform.parent = joint.transform;
            cubeObject.transform.localPosition = new Vector3(0, 0, 0);
            cubeObject.transform.localScale = new Vector3(cubeScale, cubeScale, cubeScale);
        }
    }

    void Start()
    {
        _server = new OscServer(9000); // Port number

        _server.MessageDispatcher.AddCallback(
            "", // OSC address --empty is all incoming messages
            (string address, OscDataHandle data) => {
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

                //if need to scale, should do so here instead of per feature?
                /* //getting scaling issues
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
                int index = 0; //annoying, for checking name
                foreach (GameObject joint in joints){
                    if (jointNames[index] == label){
                        Vector3 jPos = jointPositions[index];
                        if (param == "tx"){
                            jPos = new Vector3 (val, jPos.y, jPos.z);
                        } else if (param == "ty") {
                            jPos = new Vector3 (jPos.x, val, jPos.z);
                        } else if (param == "tz") {
                            jPos = new Vector3 (jPos.x, jPos.y, val);
                        }
                        jointPositions[index] = jPos;
                    }
                    index++;
                }


            }
        );
    }

     // Update is called once per frame
    void Update()
    {
        //update gameObject transforms
        int index = 0;
        foreach (GameObject joint in joints){
            joint.transform.localPosition = jointPositions[index];
            index++;
        }

        //feature tests:
    }

    void OnDestroy()
    {
        _server?.Dispose();
        _server = null;
    }
}
