# Orthographic2DCameraController is ..

.. a component for Unity that provides commonly needed camera functionality for 2D games.

# What functionality does it provide?

* Set a game object for the camera to follow
** Enable/disable without losing the reference to the object
** Use zoom-compatible displacement if you don't want exact centering
* Set player zoom (touch control pinch / mouse wheel)
** Set conditions for allowing zoom (always, never, when following object)
** Set maximum/minimum allowed amount of zoom
** Set zoom sensitivity for touch and mouse separately
* Set backdrop
** Set an image component as a static backdrop
** Set values that manipulate both the base size of the backdrop as well as how much the backdrop size should change in relation to zoom.
* "Sweeping"
** Set a state (position, rotation and/or zoom level AKA orthographic size) to transition to and a duration for how quickly to do it
** Optionally send in an _Action_ to execute once the "sweep" is complete

# What versions does it work on?

This has been thoroughly tested on **2019.4.0f1** - it may or may not work on earlier or later versions.

# How do I use it?

Just add _Orthographic2DCameraController_ as a component to the camera (a camera with _Projection_ set to _Orthographic_) in your 2D game and fill in the values.

## How do I use the sweep function?

The signature of the function is ..

```cs
public void Sweep(OrthographicCameraState targetState, float duration, Action onFinish = null)
```

.. so let's say we have a `GameObject` in the variable `boss`, we have the instance of the `Orthographic2DCameraController` attached to our main camera in the variable `cameraController`, and we have function that activates an animation where the boss character starts taunting the player character, `activateBossTauntAnimation()`.

So the camera is currently following the player character, and then the boss character appears. What we want to do then is have the camera stop following the player character, sweep over to the boss character to an orthographic size of 10 while swinging the camera around 180 degrees in 1.5 seconds, then trigger the taunt animation function.

What we would do then is the following:

```cs
var cameraState = new OrthographicCameraState
{
	position = boss.transform.position,
	size = 10f,
	rotation = Quaternion.Euler(0, 0, 180f)
};

cameraController.isFollowingObject = false;
cameraController.Sweep(cameraState, 1.5f, () => { activateBossTauntAnimation(); });
```

Notable for this type of use-case is that before all this you could also do `cameraController.GetCurrentState()` which would save the current camera state (position, size AKA zoom, rotation), and when `activateBossTauntAnimation()` (or any code executed thereafter) decides the boss taunting is finished, 
you could feed your previously stored result of `cameraController.GetCurrentState()` into the sweep function again, sweeping back to the character, and if you also send in `() => { cameraController.isFollowingObject = true; }` as the `onFinish` argument you would then resume following the player character once the camera is done with its sweeping.