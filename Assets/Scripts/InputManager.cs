using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static float MouseSensitivity = 1f;
    public static bool Paused = false;

    // to be used later for ui stacking
    private static int _paused = 0;
    public static bool CamRotDown()
    {
        if ( Paused ) return false;
        return Input.GetMouseButton(1);
    }
    public static Vector2 CamRot()
    {
        if ( Paused ) return Vector2.zero;
        return Input.mousePositionDelta;
    }
    public static bool CamMovDown()
    {
        if ( Paused ) return false;
        return Input.GetMouseButton(2);
    }
    public static Vector2 CamMov()
    {
        if ( Paused ) return Vector2.zero;
        return Input.mousePositionDelta;
    }
    public static float CamZoom()
    {
        if ( Paused ) return 0f;
        return Input.mouseScrollDelta.y;
    }

    public static bool Select()
    {
        if ( Paused ) return false;

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
        if ( Paused ) return false;

        return Input.GetButton("Camera");
    }
    public static float Zoom()
    {
        if ( Paused ) return 0f;

        return Input.GetAxis("Zoom");
    }

    public static bool HoverCell(Camera cam, LayerMask cellLayer, out HexagonCell newCell)
    {
        newCell = null;

        if ( Paused ) return false;

        if ( cam == null )
            return false;
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, cellLayer)
            && hit.transform.parent.TryGetComponent(out newCell) )
                return true;
        return false;
    }

    public static bool Pause()
    {
        if ( Paused ) return false;

        return Input.GetKeyDown(KeyCode.Escape);
    }

    public static bool Skip()
    {
        if ( Paused ) return false;

        return Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.F) || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
    }
}
