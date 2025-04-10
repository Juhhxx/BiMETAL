using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] [Range(1,5)] private float _mouseSensitivitySetter;
    private static float _mouseSensitivity;
    private void Awake()
    {
        _mouseSensitivity = _mouseSensitivitySetter;
        // DontDestroyOnLoad(gameObject);
    }
    public static bool CamRotDown()
    {
        return Input.GetMouseButton(1);
    }
    public static Vector3 CamRot()
    {
        return Input.mousePositionDelta;
    }
    public static bool CamMovDown()
    {
        return Input.GetMouseButton(2);
    }
    public static Vector3 CamMov()
    {
        return Input.mousePositionDelta;
    }
    public static float CamZoom()
    {
        return Input.mouseScrollDelta.y;
    }

    public static bool Select()
    {
        return Input.GetMouseButtonUp(0);
    }
    public static float MouseX()
    {
        return Input.GetAxis("Mouse X") * _mouseSensitivity;
    }
    public static float MouseY()
    {
        return Input.GetAxis("Mouse Y") * _mouseSensitivity;
    }
    public static float Forward()
    {
        return Input.GetAxis("Forward");
    }
    public static float Strafe()
    {
        return Input.GetAxis("Strafe");
    }
    public static bool Jump()
    {
        return Input.GetButtonDown("Jump");
    }
    public static bool Attack()
    {
        return Input.GetButtonDown("Attack");
    }
    public static bool Dash()
    {
        return Input.GetButtonDown("Dash");
    }
    public static bool Camera()
    {
        return Input.GetButton("Camera");
    }
    public static float Zoom()
    {
        return Input.GetAxis("Zoom");
    }

    public static bool HoverCell(LayerMask cellLayer, out HexagonCell newCell)
    {
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

        if ( Physics.Raycast(ray, out RaycastHit hit, cellLayer)
            && hit.transform.parent.TryGetComponent(out newCell) )
                return true;
        newCell = null;
        return false;
    }
}
