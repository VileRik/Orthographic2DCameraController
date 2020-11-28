using UnityEngine;

namespace VileRik.Orthographic2DCameraController
{
    public class ZoomOffsetCalculator
    {
        private bool wasTouchZoomingLastChecked;
        private Vector2[] lastTouchZoomPositions;

        public float GetZoomOffset(float mouseWheelSnap, float pinchSensitivity)
        {
            if (Input.touchSupported && pinchSensitivity != 0 && IsTouchZooming())
            {
                return GetZoomOffsetFromTouch(pinchSensitivity);
            }
            else wasTouchZoomingLastChecked = false;

            if (Input.mousePresent && mouseWheelSnap != 0 && IsMouseScrolling())
            {
                return GetZoomOffsetFromMouseWheel(mouseWheelSnap);
            }

            return 0;
        }

        private float GetZoomOffsetFromMouseWheel(float snap) => Input.mouseScrollDelta.y > 0 ? -snap : snap;
        private bool IsTouchZooming() => Input.touchCount == 2;
        private bool IsMouseScrolling() => Input.mouseScrollDelta.y != 0;

        private float GetZoomOffsetFromTouch(float sensitivity)
        {
            var zoomOffset = 0f;
            var newPositions = new Vector2[]
            {
                Input.GetTouch(0).position,
                Input.GetTouch(1).position
            };

            if (wasTouchZoomingLastChecked)
            {
                var newDistance = Vector2.Distance(newPositions[0], newPositions[1]);
                var oldDistance = Vector2.Distance(lastTouchZoomPositions[0], lastTouchZoomPositions[1]);
                var distanceDiff = oldDistance - newDistance;
                zoomOffset = distanceDiff * sensitivity;
            }

            lastTouchZoomPositions = newPositions;
            wasTouchZoomingLastChecked = true;

            return zoomOffset;
        }
    }
}
