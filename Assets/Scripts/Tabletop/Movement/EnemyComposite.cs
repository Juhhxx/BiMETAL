using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyComposite : TabletopMovement
{
    protected PlayerTabletopMovement _player;
    protected List<EnemyTabletopMovement> _enemies;
    protected override void Start()
    {
        base.Start();
        _player = FindFirstObjectByType<PlayerTabletopMovement>();
        _enemies = FindObjectsByType<EnemyTabletopMovement>(
            FindObjectsInactive.Include, FindObjectsSortMode.None).ToList();
    }

    protected override IEnumerator Move()
    {
        if (_player == null || _enemies == null || _enemies.Count == 0)
            yield break;


        List<EnemyTabletopMovement> leftEnemies = new(_enemies);
        List<EnemyTabletopMovement> currentEnemies = new(_enemies);
        PieceInteractive playerPiece = null;


        foreach (EnemyTabletopMovement enemy in _enemies)
        {
            enemy.TogglePath();
            enemy.FindPath();
        }

        leftEnemies.Sort();
        for (int i = 0; i < leftEnemies.Count; i++)
            leftEnemies[i].Priority = i;

        foreach (EnemyTabletopMovement enemy in leftEnemies)
            enemy.Pathfinder.Reverse();


        while (leftEnemies.Count > 0)
        {
            currentEnemies = new List<EnemyTabletopMovement>(leftEnemies);

            foreach (EnemyTabletopMovement enemy in leftEnemies)
            {
                if (enemy.Path.Count == 0)
                {
                    enemy.Stop();
                    leftEnemies.Remove(enemy);
                    continue;
                }

                HexagonCell nextCell = enemy.Path.Peek();

                // Is this cell planned by any higher-priority enemy?
                bool wait = leftEnemies
                    .Where(other => other.Priority < enemy.Priority)
                    .Any(other => other.Path.Contains(nextCell));

                if (!wait && nextCell.Piece == null)
                    enemy.MoveEnemy();

                if (nextCell.Piece is PieceInteractive pieceInteractive)
                {
                    if ( ! pieceInteractive.IsEnemy )
                        playerPiece = pieceInteractive;

                    else if (pieceInteractive.EnemyMovement.Path.Count <= 0)
                        enemy.Pathfinder.Stop();
                }

                if (enemy.Path.Count == 0)
                {
                    enemy.TogglePath();
                    leftEnemies.Remove(enemy);
                }
            }

            yield return new WaitForSeconds(0.1f);
        }

        if (playerPiece != null)
            playerPiece.Interact();
    }
}
