using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static float MouseSensitivity = 1f;
    public static bool Paused = false;
    public static bool CamRotDown()
    {
        return Input.GetMouseButton(1);
    }
    public static Vector2 CamRot()
    {
        return Input.mousePositionDelta;
    }
    public static bool CamMovDown()
    {
        return Input.GetMouseButton(2);
    }
    public static Vector2 CamMov()
    {
        return Input.mousePositionDelta;
    }
    public static float CamZoom()
    {
        return Input.mouseScrollDelta.y;
    }

    public static bool Select()
    {
        if ( Paused ) return false;
        
        Debug.Log("Pause off and select");

        return Input.GetMouseButtonDown(0);
    }
    public static float MouseX()
    {
        if ( Paused ) return 0f;
        
        return Input.GetAxis("Mouse X") * MouseSensitivity;
    }
    public static float MouseY()
    {
        if ( Paused ) return 0f;
        
        return Input.GetAxis("Mouse Y") * MouseSensitivity;
    }
    public static float Forward()
    {
        if ( Paused ) return 0f;
        
        return Input.GetAxis("Forward");
    }
    public static float Strafe()
    {
        if ( Paused ) return 0;
        
        return Input.GetAxis("Strafe");
    }
    public static bool Jump()
    {
        if ( Paused ) return false;
        
        return Input.GetButtonDown("Jump");
    }
    public static bool Attack()
    {
        if ( Paused ) return false;
        
        return Input.GetButtonDown("Attack");
    }
    public static bool Dash()
    {
        if ( Paused ) return false;
        
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
        newCell = null;
        if ( Paused ) return false;
        
        Ray ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);

        if ( Physics.Raycast(ray, out RaycastHit hit, cellLayer)
            && hit.transform.parent.TryGetComponent(out newCell) )
                return true;
        return false;
    }

    public static bool Pause()
    {
        return Input.GetKeyDown(KeyCode.Escape);
    }
}
