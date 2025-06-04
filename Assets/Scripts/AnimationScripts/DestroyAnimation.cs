using UnityEngine;

public class DestroyAnimation : MonoBehaviour
{
    [SerializeField] private EnemyMovement _enemyMovement;

    public void DespawnEnemy()
    {
        _enemyMovement.DespawnEnemy();
    }
}
