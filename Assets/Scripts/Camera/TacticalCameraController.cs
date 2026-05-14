using UnityEngine;

namespace Skybound.CameraSystem
{
    public class TacticalCameraController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 25f;
        [SerializeField] private float zoomSpeed = 150f;
        [SerializeField] private float minHeight = 8f;
        [SerializeField] private float maxHeight = 30f;

        private void Update()
        {
            HandleMovement();
            HandleZoom();
        }

        private void HandleMovement()
        {
            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            Vector3 forward = transform.forward;
            Vector3 right = transform.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 move = (right * x + forward * z).normalized;
            transform.position += move * moveSpeed * Time.unscaledDeltaTime;
        }

        private void HandleZoom()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");

            if (Mathf.Abs(scroll) < 0.01f)
                return;

            Vector3 pos = transform.position;
            pos += transform.forward * scroll * zoomSpeed * Time.unscaledDeltaTime;

            pos.y = Mathf.Clamp(pos.y, minHeight, maxHeight);
            transform.position = pos;
        }
    }
}