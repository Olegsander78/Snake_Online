using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public void Init(float offsetY)
    {
        Transform camera = Camera.main.transform;
        camera.parent = transform;
        camera.localPosition = Vector3.up * offsetY;
    }

    private void OnDestroy()
    {
        if (Camera.main == null)
            return;

        Transform camera = Camera.main.transform;
        camera.parent = null;
    }
}
