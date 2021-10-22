using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turntable : MonoBehaviour
{
    /// <summary>
    ///     How many seconds between each iteration of the turntable.
    /// </summary>
    private const float _delay = 0.25F;

    /// <summary>
    ///     How many degrees for each turntable increment.
    /// </summary>
    private const int _increment = 5;

    /// <summary>
    ///     Distance to offset from the object.
    ///     This is a multiplier times the object's size (ie _offset lengths).
    /// </summary>
    private const float _offset = 3.0F;

    /// <summary>
    ///     How high above the object to orbit.
    ///     This is a multiplier times the object's size (ie _height lengths). 
    /// </summary>
    private const float _height = 0.5F;

    /// <summary>
    ///     The directory on the desktop to save screenshots to.
    /// </summary>
    private const string _dir = "Turntable";

    /// <summary>
    ///     The object to orbit around.
    /// </summary>
    public GameObject target;

    /// <summary>
    ///     The camera to move.
    /// </summary>
    public Camera cam;

    /// <summary>
    ///     Whether or not the turntable effect is currently in action.
    /// </summary>
    private bool _orbit;

    /// <summary>
    ///     The time from the previous orbit iteration.
    /// </summary>
    private float _last;

    /// <summary>
    ///     The current angle at which to orbit the object.
    /// </summary>
    private int _degrees;

    /// <summary>
    ///     Called before the first frame update.
    /// </summary>
    void Start()
    {
        _orbit = false;

        Go();
    }

    /// <summary>
    ///     Called once per frame.
    /// </summary>
    void Update()
    {
        if (_orbit)
        {
            // Wait for 1 second intervals.
            if (Time.time - _last < _delay)
                return;

            // Reset clock for next wait.
            _last = Time.time;

            // Iterate position like the object is on a turntable.
            // Use a scale times its size as the render distance.
            var size = target.GetComponent<Renderer>().bounds.size.magnitude;
            var range = size * _offset;
            var height = size * _height;
            AimCamAtTarget(_degrees, range, height);

            // Take the screenshot.
            CaptureScreenshot();

            // Increment next angle.
            _degrees += _increment;

            // Stop when done.
            if (_degrees >= 360)
                _orbit = false;
        }
    }

    /// <summary>
    ///     Start a new pass at the orbit.
    /// </summary>
    public void Go()
    {
        _orbit = true;
        _degrees = 0;
        _last = Time.time;
    }

    /// <summary>
    ///     Aim the given camera at the given target.
    /// </summary>
    /// <param name="angle">Angle in degrees.</param>
    /// <param name="range">Range to target in terms of object's size.</param>
    /// <param name="height">Height above target in terms of object's size.</param>
    private void AimCamAtTarget(double angle, double range, double height)
    {
        // Position the camera.
        var rad = angle * Mathf.Deg2Rad;
        var aimpoint = target.transform.position;
        var x = aimpoint.x + range * Mathf.Cos((float) rad);
        var z = aimpoint.z + range * Mathf.Sin((float) rad);
        var y = aimpoint.y + height;
        var pos = new Vector3((float) x, (float) y, (float) z);
        cam.transform.position = pos;

        // Aim the camera.
        cam.transform.LookAt(target.transform);
    }

    /// <summary>
    ///     Takes a screenshot of the current render.
    ///     Screenshots are stored in a folder on the user's desktop.
    ///     Each will be tagged with the object name and the angle of orbit in degrees.
    /// </summary>
    private void CaptureScreenshot()
    {
        // Get the filename.
        var filename = target.name;
        filename += "-" + _degrees + ".png";

        // Get the directory.
        var directory = System.IO.Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop), _dir);

        // Make sure the path exists.
        if (!System.IO.Directory.Exists(directory))
            System.IO.Directory.CreateDirectory(directory);

        // Get the path.
        var path = System.IO.Path.Combine(directory, filename);
        path = path.ToString();

        // Take the screenshot.
        ScreenCapture.CaptureScreenshot(path);
    }
}
