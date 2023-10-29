# KinectOSC_UnityTD

TouchDesigner sending Kinect body tracking data to Unity over OSC (port 9000).

## Setup

To import to existing project, bring in the `Stage` and `AvatarManager` prefabs and the `BodyDataManager_OSCfromTD` script. AvatarManager should be on the "Avatar" layer (21), but double check.

*Assumes the camera position in the scene is where the audience is and where the dancer is facing. Should hopefully calibrate correctly regardless of where Kinect is placed.*

## Calibration

Calibration process uses buttons in the AvatarManager inspector to capture Kinect positions at 6 points on the stage. Incoming kinect values then can be mapped between the kinect min/max positions and the 6 CalibrationPoints. Uses PlayerPrefs for persistent data so we can store dancer profiles between sessions and not have to calibrate every time we go into play mode.

*Clicking the calibration buttons multiple times in each spot stores a range of positions that are then averaged to smooth out the noisy data.*

1) If the floor size has been adjusted, place the **CalibrationPoints** GameObjects `0-3` in the four corners of the floor, still parented to the stage. Place the ceiling GameObject `4` in the center of the stage, at the highest point you want the avatar's hands.
   1) The clock labels assume the camera/audience viewpoint, not the Kinect viewpoint (assuming the Kinect isn't in front of the dancer) (e.g. 10:30 is to the left, upstage). The direction labels assume the dancer's orientation (e.g. BackRight is behind the dancer to their right).
   2) The Floor, CalibrationPoints, and AvatarManager GameObjects all need to be on the same child level, and all at [0, 0, 0].
2) Make sure the **TouchDesigner project** is open, running, and set to **Live Kinect mode**.
3) Select the **AvatarManager** in the hierarchy and go to the custom inspector interface in the BodyDataManager script component.
   1) [TODO]Make sure **`Calibrate New Dancer`** is selected in the `Profile` dropdown.
4) **Play** the scene.
5) ~~Have the dancer step in the **center** of the stage facing the audience and then press the `CENTER` button.~~
   1) ~~Check the `centerPos` Vec3 value that appears in the inspector. It should be somewhere around `[0, 0, 2.5]*`. If it's not, click the `RESET CENTER` button.~~
      1) ~~The x should be 0, but the y and z will vary depending on hip height vs Kinect height and distance to Kinect.~~
   2) ~~Have them shift their weight slightly, stand up straight, small adjustments -- keep clicking the `CENTER` as they do this. The `centerPos` Vec3 will update slightly until enough positions are stored to display a more stable position.~~
6) Have them move to the back right **corner** (at 10:30 on the clock) and stand facing the kinect.
   1) Have them shift their weight slightly, stand up straight, rotate, small adjustments -- keep clicking the `SET 0_BR_1030` button as they do this. The `0_BR_1030` Vec3 will update slightly until enough positions are stored to display a more stable position.
7) Follow the same steps moving **clockwise** around the stage for the next three points.
   1) 4 corner calibration points and example Vec3 positions if Kinect is in front, at hip height, and roughly 2m from center.
      1) `0_BR_1030`: `[1.1,  .1,  3.9]`
      2) `1_BL_130`:  `[-1.2, .2,  3.7]`
      3) `2_FL_430`:  `[-1.2,  0,  1.3]`
      4) `3_FR_730`:  `[1.2,  .1,  1.2]`
8) Have them go back to the center and stand up straight and **reach as high as they can with their right hand**. Click 'MAXREACH' and repeat the checking/smoothing process.
   1) IT MUST BE THEIR RIGHT HAND (for now)
   2) The value will be something like `[0, 1.2, 2.5]`, assuming Kinect around hip height.
   3) ~~Both hands is fine too, but they'll reach higher with one.~~
   4) They could also jump if they want that range, but you have to time the button presses, tricky.
9) Have them place **both hands on the ground**. Click 'FLOOR` and repeat the checking/smoothing process.
   1) The value will be something like `[0, -0.75, 2.5]`, assuming Kinect around hip height.
10) [TODO]Lastly, ask them to face forward and **stretch their hands out** to the sides as far as they can. Click the 'WINGSPAN' button and check/smooth.
11) After all calibration steps, press the 'CALIBRATE' button. The avatar position should immediately update.
12) To confirm the calibration has worked, have the dancer move about the stage and **compare their virtual position**.
13) [TODO]If satisfied with the calibration, type the Dancer's name along with the Kinect orientation in the **`ProfileName` input field**.
    1) For example, for Duncan's calibration when the Kinect is in front of him, at hip height, around 2m from center, that profile would be called `DUNCAN_frontHip2m`.
14) Click `SAVE CALIBRATION PROFILE` to **finish** the calibration and save the profile.

## TouchDesigner

Press `k` key to switch between Live Kinect and Dummy Data.

When in dummy data mode, press `1` key to switch between:

- Sending example calibration positions using the reference pose
  - press '6' to cycle between the calibration points (currently don't have hands in the reference pose)
- Sending all joints with the same x, y, z positions, animating with noise
- Sending all joints with the same x, y, z positions, animating along box boundaries
- Sending a snapshot pose with correct Kinect data (person standing with left arm raised)

## Unity

- Version 2022.3.3f1
- Keijiro's [OSC Jack](https://github.com/keijiro/OscJack) package
