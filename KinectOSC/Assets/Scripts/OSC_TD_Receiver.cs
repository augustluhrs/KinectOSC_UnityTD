using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class OSC_TD_Receiver : MonoBehaviour
{
    //right now just takes the pelvis tx,ty,tz and updates root's transform
    //could just have this on the AvatarManager, but prob better practice to keep separate in case we wanna change
    public GameObject root;

    public float floorScale = 5; //however we want to scale the incoming position data
    // public string incomingData;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log(incomingData.ToString()); //dunno how to log this
    }

    //not working
    // public void setDebugText(string text)
    // {
    //     incomingData = text;
    // }

    //dumb, need to access underlying messages or OSCJack API
    public void setTransformX(float _x)
    {
        root.transform.localPosition = new Vector3(_x * floorScale, root.transform.localPosition.y, root.transform.localPosition.z);
        // Debug.Log(root.transform.localPosition);
    }

    public void setTransformY(float _y)
    {
        root.transform.localPosition = new Vector3(root.transform.localPosition.x, _y * floorScale, root.transform.localPosition.z);
    }

    public void setTransformZ(float _z)
    {
        root.transform.localPosition = new Vector3(root.transform.localPosition.x, root.transform.localPosition.y, _z * floorScale);
    }
}
