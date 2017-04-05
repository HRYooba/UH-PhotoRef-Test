                                                                    @mainpage
Unicessing
======================

Ver.0.16

Processing like Drawing API

@image html unicessing.jpg



About
--------

Unicessing is an asset for writing a code like 'Processing' in Unity.

It is designed to use DrawMesh().
It is used in the Unity coordinate system.

To use it, just inherit UGraphics class in your script instead of MonoBehaviour,
and then you have new callbacks such as Setup() and Draw() for your Processing-like coding.
You can still mix them perfectly with other Unity scripts.

Let's enjoy the dynamic graphic drawing (generative art) with a short code,
referring to samples.
You can draw in VR HMDs such as Oculus Rift and HTC Vive.

NOTE: Only a subset of Processing APIs are available today through this package,
also it is not yet heavily optimized.


                                                                    @page Updates
Updates
--------

### Ver.0.16 / 2016.12.22

Add reference.
* Added simple comments to UGraphics. Publish online reference.
* Japanease http://dev.eyln.com/unicessing/doc/jp/
* English http://dev.eyln.com/unicessing/doc/en/

Change functions.
* stroke(), fill(), background() Change arguments from int to float.
* background(CameraClearFlags flags) to backgroundFlags(CameraClearFlags flags).
* beginShape() Sets the default value to LINE_STRIP.
* color() Change from UConstans static to normal function.
* rotateDegree() to rotateDegrees().
* getWorldPos(localPos) to localToWorldPos(localPos).
* getLocalPos(worldPos) to worldToLocalPos(worldPos).
* getWorldDir() to localToWorldDir(), getLocalDir() to worldToLocalDir().
* By abolishing clearDepthStep(), this function is unified to noDepthStep().

Add functions.
* backgroundSkybox()
* colorMode(), red(), blue(), green(), alpha(), hue(), saturation(), brightness(), lerpColor()
* UShape addChild(), removeChild()
* rotate(xAngle, yAngle, zAngle), rotateDegrees(eulerAngles)


### Ver.0.15 / 2016.12.12

Bug fix.
* beginShape(QUAD) Fix to prevent polygons from being missing.
* Fixed a bug that draws ellipse() twice as large.
* Fix mouseX, Y, Z values in P2D, P3D mode.
* Changed to default to noLights() for P2D.

Add functions.
* Added lights() and noLights() to UShape.

### Ver.0.14 / 2016.12.8

Bug fix.
* scaleFix calculation of translate() and text() when using scale().
* Changed Shader to Cull Off.

Add functions.
* size(width, height, P2D or P3D or U3D) ... Beta version.
* triangle()

Add samples.
* P5_Snows, ZenTexts


### Ver.0.13 / 2016.11.9

First release.


### Ver.0.12 / 2016.11.4

Add functions.
* rectMode(), ellipseMode(), imageMode()
* stroke(), noStroke(), noFill()


### Ver.0.11 / 2016.10.29

Bug fix.
* loadImage(), randomSeed()


### Ver.0.10 / 2016.10.29

Alpha test.



                                                                    @page RequiredAssets
Required Assets
---------------

Unicessing Samples uses another asset provided by Unity.
Sorry to trouble you, but please import the following assets in advance.


[1]. Cinematic Image Effects

https://www.assetstore.unity3d.com/#!/content/51515

Please obtain the above asset from the Asset Store.
I am using only Bloom and TonemappingColorGrading as camera ImageEffect.


[2]. Third Person Character (Ethan)

I use it as a character in the Runnner sample.
Choose Unity's menu Assets -> Import Package -> Character
Please check "Third Person Character" and import it.



                                                                    @page Samples
Samples
-------

Demo scenes: Assets/Unicessing/Scenes/*

Sample scripts: Unicessing/Scripts/Samples/*

Basic template: Unicessing/Scripts/Samples/UnicessingTemplate.cs

Note: In order to load each scene with Menu sample,
you need to add each sample scenes to Build Settings.

### Additional samples

I transplanted some code of Processing and published it below.

https://goo.gl/cC8gbj



                                                                    @page Descriptions
Descriptions
------------

### Script reference

Here is a simple online reference.

* English http://dev.eyln.com/unicessing/doc/en/
* Japanease http://dev.eyln.com/unicessing/doc/jp/

Unicessing modeled on Processing API

* https://processing.org/reference/

NOTE: Only a subset of Processing APIs are available today through this package.
There are no camera() and strokeWeight() functions for Unicessing. 
Adjust each object with Unity's hierarchy.
Also, width and height do not exist either.


### Summary and comparison with Processing

The coordinate system matches the Unity coordinate system.
Unlike Processing, the upward direction is Y +.
The reference position of rect () is lower left.

NOTE: Beta version function : If you specify size(640, 480, P3D, 0.01f) etc.,
you can change to Processing coordinate system.
If it coexists with the Unity coordinates it will be confusing and not recommended.
However, it may be easier to port Processing code. 0.01 f is the scale value totally.

Draw lines and points:
line(), curve(), point()

Set the color of lines and points:
stroke(), noStroke()

Draw shapes:
rect(), ellipse(), box(), sphere()

Set the color of shapes:
fill(), noFill()

Process color:
color(), colorMode(), red(), blue(), green(), alpha(), hue(), saturation(), brightness(), lerpColor()

Change the reference position:
rectMode(), ellipseMode(), imageMode()

Default...rectMode(CORNER), ellipse(CENTER).
rectMode(CORNER_P5) is Left Top.

Change shading:
lights(), noLigths()

Change blend mode:
blendMode()

Draw texts:
text(), textAlign(), textSize()

Change font:
loadFont(), textFont()

Draw images:
loadImage(), image()

Change textures:
texture(), noTexture()

Change matrix:
translate(), scale(), rotate(),
rotateX(), rotateY(), rotateZ(),
rotateDegrees(), rotateRadians()

Stack:
pushMatrix(), popMatrix(),
pushStyle(), popStyle(),
push(), pop()

Mouse informations:
mouseX, mouseY, mouse3D,
mousePressed, mouseReleased, mouseButton

NOTE:The position of the mouse is determined to be a virtual plane
with the normal of forward of mousePlane specified in UGraphics.

Key informations:
isKey(), isKeyDown(), isKeyUp()

Draw custom shapes:
beginShape(), vertex(), curveVertex(), endShape(),
createShape() and draw(shape)

Mathematical functions:
Please see Unicessing/Scripts/Core/UMath.cs


### Extras

Extra functions:
loadScene(),
loadPrefab(), draw(prefab), mesh(),
createObject() and draw(obj),
...etc.

You can also judge whether it is VR mode by isVR.
In VR mode, the value of mouseX, mouseY,
mouse3D is the center position in the direction you see in HMD.

Multiple Unicessings can run simultaneously.
However, using USubGraphics, one UGraphics can be shared by more than one Composite.
Examples of using these are in UnicessingMenu.cs and UnicessingRunnner.cs.

There are other functions as well.
Please see Unicessing/Scripts/Core/UGraphics.cs and Core/*.cs



Attention
-----------

Standard Assets is provided by Unity Technologies.

Unicessing has undefined functions and specifications
that are different from those in Processing.
I may change the specification of Unicessing without notice in the future.



Copyright
----------

### Unicessing

Copyright (C) 2016 NISHIDA Ryota / ship of EYLN

http://dev.eyln.com/
