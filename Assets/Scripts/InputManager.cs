using UnityEngine;

public class InputManager : MonoBehaviour
{
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

    public static bool HoverCell(LayerMask cellLayer, out HexagonCell newCell)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if ( Physics.Raycast(ray, out RaycastHit hit, cellLayer)
            && hit.transform.parent.TryGetComponent(out newCell) )
                return true;
        newCell = null;
        return false;
    }
}
