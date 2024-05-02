using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction
{
    Left, Right, Top, Bottom
}

[System.Serializable]
public class WallEnemy
{
    public GameObject enemy;
    public List<Direction> spawnableWalls;
}

public class EnemyWaveSpawner : MonoBehaviour
{
    public Transform TopWall;
    public Transform BotWall;

    [Space(2f)]
    public Transform LeftWall;
    public Transform RightWall;

    public LayerMask AvoidLayer;

    [Space(5f)]
    public List<GameObject> Enemies = new();
    public List<WallEnemy> WallEnemies = new();

    private (float low, float high) _verticalRange;
    private (float low, float high) _horizontalRange;

    private void Start()
    {
        _verticalRange.low = LeftWall.position.x;
        _verticalRange.high = RightWall.position.y;

        _horizontalRange.low = BotWall.position.y;
        _horizontalRange.high = TopWall.position.y;
    }

    private void SpawnWallEnemy()
    {
        Quaternion _rotation = Quaternion.identity;
        Vector2 _position = Vector2.zero;
        WallEnemy walLEnemy = WallEnemies[Random.Range(0, WallEnemies.Count)];

        GameObject _enemy = walLEnemy.enemy;
        Direction chosenDirection = walLEnemy.spawnableWalls[Random.Range(0, walLEnemy.spawnableWalls.Count)];
        SpriteRenderer _enemyBounds = _enemy.GetComponent<SpriteRenderer>();

        switch (chosenDirection)
        {
            case Direction.Left: // Left
                _rotation = Quaternion.Euler(0, 0, -90);
                break;

            case Direction.Right: // Right
                _rotation = Quaternion.Euler(0, 0, 90);
                break;

            case Direction.Top: // Top
                _rotation = Quaternion.Euler(0, 0, 180);
                break;

            case Direction.Bottom: // Bot
                _rotation = Quaternion.identity;
                break;
        }

        _position = FindEmptyWallLocation( _enemyBounds, chosenDirection);

        if (_position != Vector2.zero)
        {
            Instantiate(_enemy, _position, _rotation);
        }
        
    }

    private Vector2 FindEmptyWallLocation(SpriteRenderer enemyBounds, Direction dir)
    {
        Vector2 _position = Vector2.zero;
        float paddingX = enemyBounds.bounds.size.x / 2;
        float paddingY = enemyBounds.bounds.size.y / 2;

        for (int i = 0; i < 100; i ++)
        {
            switch (dir)
            {
                case Direction.Left:
                    _position = new Vector2(LeftWall.position.x + paddingY, Random.Range(_horizontalRange.low + paddingX, _horizontalRange.high - paddingX));
                    break;

                case Direction.Right:
                    _position = new Vector2(RightWall.position.x - paddingY, Random.Range(_horizontalRange.low + paddingX, _horizontalRange.high - paddingX));
                    break;

                case Direction.Top:
                    _position = new Vector2(Random.Range(_verticalRange.low, _verticalRange.high) + paddingX, TopWall.position.y - paddingY);
                    break;

                case Direction.Bottom:
                    _position = new Vector2(Random.Range(_verticalRange.low, _verticalRange.high) + paddingX, BotWall.position.y + paddingY);
                    break;
            }

            // check position available
            if (!Physics2D.OverlapBox(_position, new Vector2(enemyBounds.bounds.size.x, enemyBounds.bounds.size.y), 0, AvoidLayer))
            {
                return _position;
            }
            
        }
        print("no pos found");
        return Vector2.zero;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Y))
        {
            SpawnWallEnemy();
        }
    }
}
