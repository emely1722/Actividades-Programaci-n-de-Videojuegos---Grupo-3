using UnityEngine;

public class ParallaxScript : MonoBehaviour
{
    public Transform cameraTransform;
    public Vector2 parallaxMultiplier = new Vector2(0.2f, 0.1f);

    private Vector3 lastCameraPosition;

    void Start()
    {
        if (cameraTransform == null)
            cameraTransform = Camera.main.transform;

        lastCameraPosition = cameraTransform.position;
    }

    void LateUpdate()
    {
        Vector3 cameraMovement = cameraTransform.position - lastCameraPosition;

        transform.position += new Vector3(
            cameraMovement.x * parallaxMultiplier.x,
            cameraMovement.y * parallaxMultiplier.y,
            0f
        );

        lastCameraPosition = cameraTransform.position;
    }
}