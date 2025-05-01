using UnityEngine;

public class TestPersistance : MonoBehaviour
{
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            TabletopController ctrl = FindFirstObjectByType<TabletopController>();

            ctrl.EndBattle(true);
        }
        if (Input.GetMouseButtonDown(1))
        {
            TabletopController ctrl = FindFirstObjectByType<TabletopController>();
            
            ctrl.EndBattle(false);
        }
    }
}
