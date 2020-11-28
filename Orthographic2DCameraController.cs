using System;
using UnityEngine;
using UnityEngine.UI;

namespace VileRik.Orthographic2DCameraController
{
    [RequireComponent(typeof(Camera))]
    public class Orthographic2DCameraController : MonoBehaviour
    {
        public enum AllowZoomSetting
        {
            Always,
            WhenFollowingObject,
            Never
        }

        private Camera targetCamera;
        private readonly ZoomOffsetCalculator zoomOffsetCalculator = new ZoomOffsetCalculator();

        [Header("Follow object")]
        [Tooltip("The (transform of the) game object the camera should follow.")]
        public Transform objectToFollow;
        [Tooltip("After setting the object to follow, set this to true")]
        public bool isFollowingObject;
        [Tooltip("Offset in X-axis to use when the camera is following the specified object.")]
        public float followXOffset;
        [Tooltip("Offset in Y-axis to use when the camera is following the specified object.")]
        public float followYOffset;

        [Header("Player zoom(orthographic size) limits")]
        public AllowZoomSetting allowZoom;
        [Tooltip("The minimum orthographic size value allowed by the player zoom controls.")]
        public float minimumZoom;
        [Tooltip("The maximum orthographic size value allowed by the player zoom controls.")]
        public float maximumZoom;
        [Tooltip("Amount the zoom should snap in/out with every turn of the mouse wheel.")]
        public float mouseWheelZoomSnap;
        [Tooltip("Amount to zoom in/out when pinching the screen.")]
        public float pinchZoomSensitivity;

        [Header("Backdrop")]
        [Tooltip("Image to use for backdrop.")]
        public Image backdropImage;
        [Tooltip("The pixels per unit for the backdrop is calculated as [base multiplier + (orthographic size x camera multiplier)], so this value is the base, or the displacement of the range of effect of the camera multiplier on the backdrop size.")]
        public float backdropPPUBaseMultiplier;
        [Tooltip("The pixels per unit for the backdrop is calculated as [base multiplier + (orthographic size x camera multiplier)], so the higher value of this, the greater effect zooming will have on the backdrop size.")]
        public float backdropPPUCameraMultiplier;

        [Header("Sweeping")]
        [Tooltip("Indicates if the camera is currently sweeping or not (read-only, for debug and testing purposes).")]
        [SerializeField] [ReadOnly] private bool isSweeping = false;
        private float sweepTimePassed;
        private float sweepTargetDuration;
        private OrthographicCameraState sweepTargetState = null;
        private OrthographicCameraState sweepStartState = null;
        private Action onSweepFinish;

        private void Start()
        {
            targetCamera = GetComponent<Camera>();
        }

        private void Update()
        {
            UpdateBackdrop();

            if (isSweeping)
            {
                UpdateSweep();
            }
            else
            {
                UpdateZoom();
                UpdateCameraPosition();
            }
        }

        public void UpdateBackdrop()
        {
            if (backdropImage is Image)
            {
                backdropImage.SetAllDirty();
                backdropImage.pixelsPerUnitMultiplier = backdropPPUBaseMultiplier + (targetCamera.orthographicSize * backdropPPUCameraMultiplier);
            }
        }

        public void Sweep(OrthographicCameraState targetState, float duration, Action onFinish = null)
        {
            sweepTimePassed = 0;
            sweepTargetState = targetState;
            sweepStartState = GetCurrentState();
            sweepTargetDuration = duration;
            onSweepFinish = onFinish;
            isSweeping = true;
        }

        public OrthographicCameraState GetCurrentState()
        {
            return new OrthographicCameraState
            {
                position = transform.position,
                size = targetCamera.orthographicSize,
                rotation = transform.rotation
            };
        }

        public Vector3 GetFollowedObjectPositionForSize(float size)
        {
            var x = objectToFollow is null ? 0 : objectToFollow.position.x + (size * 2 * followXOffset);
            var y = objectToFollow is null ? 0 : objectToFollow.position.y + (size * 2 * followYOffset);
            var z = targetCamera.transform.position.z;

            return new Vector3(x, y, z);
        }

        private void UpdateSweep()
        {
            sweepTimePassed += Time.deltaTime / sweepTargetDuration;

            Camera.main.orthographicSize = Mathf.Lerp(sweepStartState.size, sweepTargetState.size, Mathf.SmoothStep(0.0f, 1.0f, sweepTimePassed));

            var newPosition = Vector3.Lerp(sweepStartState.position, sweepTargetState.position, Mathf.SmoothStep(0.0f, 1.0f, sweepTimePassed));
            newPosition.z = transform.position.z;

            transform.position = newPosition;
            transform.rotation = Quaternion.Lerp(sweepStartState.rotation, sweepTargetState.rotation, Mathf.SmoothStep(0.0f, 1.0f, sweepTimePassed));

            if (sweepTargetState.Equals(GetCurrentState()))
            {
                isSweeping = false;

                if (onSweepFinish is Action)
                {
                    onSweepFinish();
                }
            }
        }

        private void UpdateZoom()
        {
            if (allowZoom.Equals(AllowZoomSetting.Always) || (allowZoom.Equals(AllowZoomSetting.WhenFollowingObject) && isFollowingObject && objectToFollow is Transform))
            {
                var zoomOffset = zoomOffsetCalculator.GetZoomOffset(mouseWheelZoomSnap, pinchZoomSensitivity);
                var newZoom = Mathf.Clamp(targetCamera.orthographicSize + zoomOffset, minimumZoom, maximumZoom);
                targetCamera.orthographicSize = newZoom;
            }
        }

        private void UpdateCameraPosition()
        {
            if (isFollowingObject && objectToFollow is Transform)
            {
                targetCamera.transform.position = GetFollowedObjectPositionForSize(targetCamera.orthographicSize);
            }
        }
    }
}