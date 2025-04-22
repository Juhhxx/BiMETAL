using System;
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
        _player = FindFirstObjectByType<PlayerTabletopMovement>();
        _enemies = FindObjectsByType<EnemyTabletopMovement>(
            FindObjectsInactive.Exclude, FindObjectsSortMode.None).ToList();
        
        if ( _enemies.Count <= 0 ) gameObject.SetActive(false);
    }

    public void StartMoving()
    {
        StartCoroutine(Move());
    }

    public Action EnemyTurn;
    protected override IEnumerator Move()
    {
        if (_player == null || _enemies == null || _enemies.Count == 0)
        {
            DoneMoving();
            yield break;
        }

        List<EnemyTabletopMovement> leftEnemies = new(_enemies);
        List<EnemyTabletopMovement> currentEnemies;
        PieceInteractive playerPiece = null;


        foreach (EnemyTabletopMovement enemy in _enemies)
        {
            enemy.TogglePath();
            enemy.FindPath();

            if (enemy.Interactive is PieceInteractive interactive)
                interactive.Stop();
        }

        yield return new WaitUntil(() => _enemies.All(t => t.Pathfinder.Done));
        Debug.Log("Enemies done path finding.");

        leftEnemies.Sort();
        for (int i = 0; i < leftEnemies.Count; i++)
            leftEnemies[i].Priority = i;

        foreach (EnemyTabletopMovement enemy in leftEnemies)
        {
            enemy.last = enemy.Path.Peek();
            enemy.Pathfinder.Reverse();
            // remove start
            enemy.Path.Pop();
        }


        while (leftEnemies.Count > 0)
        {
            Debug.Log("Moving enemies, count now: " + leftEnemies.Count);
            currentEnemies = new(leftEnemies);

            foreach (EnemyTabletopMovement enemy in currentEnemies)
            {
                if ( enemy.Path.Count <= 0 || enemy.QueueCount >= enemy.Path.Count || enemy.Moving )
                {
                    if ( enemy.Path.Count <= 0 )
                    {
                        Debug.Log("last, Removing " + enemy + " path count: " + enemy.Path.Count + " queue count: " + enemy.QueueCount);
                        leftEnemies.Remove(enemy);
                    }
                    Debug.Log("last, Skipping " + enemy + " path count: " + enemy.Path.Count + " queue count: " + enemy.QueueCount);
                    continue;
                }

                HexagonCell nextCell = enemy.Path.Peek();

                if ( nextCell == enemy.last )
                    Debug.Log("found last at path count: " + enemy.Path.Count + " queue count: " + enemy.QueueCount);

                // Is this cell planned by any higher-priority enemy?
                bool wait = leftEnemies
                    .Where(other => other.Priority < enemy.Priority)
                    .Any(other => other.Path.Contains(nextCell));

                if ( ! wait )
                    enemy.MoveEnemy();
                
                else if ( nextCell.Piece is PieceInteractive piece )
                {
                    if ( ! piece.IsEnemy ) // is enemy is based on if enemy movement is null or not
                    {
                        playerPiece = piece;
                        enemy.Stop();
                        leftEnemies.Remove(enemy);
                    }
                    else if ( piece.EnemyMovement.Path.Count <= 0 )
                    {
                        enemy.Stop();
                        leftEnemies.Remove(enemy);
                    }
                }
            }

            Debug.Log("Still running enemy composite.");

            yield return null;
        }
        Debug.Log("Waiting for enemies to stop.");
        yield return new WaitUntil(() => _enemies.Where(t =>  t != null ).All(t =>!t.Moving));
        Debug.Log("Enemies stopped.");
        
        if (playerPiece != null)
            playerPiece.Interact();

        foreach (EnemyTabletopMovement enemy in _enemies)
        {
            enemy.TogglePath();

            if (enemy.Interactive is PieceInteractive interactive)
                interactive.Modify();
        }

        DoneMoving();
    }

    private void DoneMoving()
    {
        Debug.Log("Enemy un-turned.");
        EnemyTurn?.Invoke();
    }
}
