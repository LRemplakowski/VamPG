PEGASUS
=======

Welcome to Pegasus, the cutscene and flythrough generation system for Unity 3D.

OVERVIEW:

Pegasus consists of a Manager, and POI (Points of Interest) that it flies through. The Manager allows you to control overall flythrough properties, and the POI allows you to control the behaviour of the camera as it flies through individual locations in your scene.

There are two types of fly throughs:
	- SINGLE SHOT fly throughs will just play through a single time and then stop;
	- LOOPED fly throughs are infinite fly throughs that will look back to the start after they have finished.

QUICKSTART:

Option One:

To add a Pegasus Capture object to your scene select GameObject -> Pegasus -> Add Pegasus Capture.

Press Play and compose your shots - when you have a nice shot press P to capture it. The stop your game and press Create Pegasus on your Pegasus Capture. Done :)


Option Two:

To add a Pegasus Manager to your scene select GameObject -> Pegasus -> Add Pegasus Manager. 

To add Points of Interest (POI) to your scene hit the CTRL button and while holding it down click the LEFT Mouse Button. This will create a new point of interest at every location that you click on. You need to add two or more points of interest to get a fly through.

By default Pegasus will pick up your main camera as its target. If you would like Pegasus to control something else then drop it into the Target Object slot in the Pegasus Manager.

To Play your fly through just press the Play button. 


DEMO:

For a quick demo of Pegasus in action open up the demo scene and press Play. Check out the configuration of the manager, and then the configuration of each of the POI to see how different things work.

CREDITS:

The default Mecanim Animations have been sourced from the Unity 3D - Standard Assets - Characters animations, and provided here for your convenience to save you from having to load the Standard Asset Characters into your project. These authors of these animations retain full intellectual property rights and ownership in their products and Procedural Worlds does not assert any rights over these works.


DETAILED DOCUMENTATION:

Detailed documentation can be found in the documentation directory, or online at http://www.procedural-worlds.com/pegasus/documentation/.