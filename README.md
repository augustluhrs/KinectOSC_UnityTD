# KinectOSC_UnityTD

TouchDesigner sending Kinect body tracking data to Unity over OSC (port 9000).

## Note

Right now, just takes the pelvis position and updates the root of an avatar gameobject. Need to figure out the underlying OSCJack API so that I can just parse the incoming messages instead of having to make a separate event handler for each x,y,z of every joint.

Dummy data also just sends flat x, y, z on all joints (i.e. all joints have same x, y, z, no actual skeleton positions like in Kinect data).

## TouchDesigner

Toggle that sends dummy data if no Kinect is present.

## Unity

- Version 2022.3.3f1
- Keijiro's [OSC Jack](https://github.com/keijiro/OscJack) package
