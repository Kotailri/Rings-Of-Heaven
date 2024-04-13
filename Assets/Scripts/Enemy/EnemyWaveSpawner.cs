using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWaveSpawner : MonoBehaviour
{
    private enum Direction
    {
        Left, Right, Top, Bottom
    }

    public Transform TopWall;
    public Transform BotWall;

    [Space(2f)]
    public Transform LeftWall;
    public Transform RightWall;

    public LayerMask AvoidLayer;

    [Space(5f)]
    public List<GameObject> Enemies = new();
    public List<GameObject> WallEnemies = new();

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
        GameObject _enemy = WallEnemies[Random.Range(0, WallEnemies.Count)];
        SpriteRenderer _enemyBounds = _enemy.GetComponent<SpriteRenderer>();

        switch (Random.Range(0,4))
        {
            case 0: // Left
                _rotation = Quaternion.Euler(0, 0, -90);
                _position = FindEmptyWallLocation(_enemyBounds, Direction.Left);
                break;

            case 1: // Right
                _rotation = Quaternion.Euler(0, 0, 90);
                _position = FindEmptyWallLocation(_enemyBounds, Direction.Right);
                break;

            case 2: // Top
                _rotation = Quaternion.Euler(0, 0, 180);
                _position = FindEmptyWallLocation(_enemyBounds, Direction.Top);
                break;

            case 3: // Bot
                _rotation = Quaternion.identity;
                _position = FindEmptyWallLocation(_enemyBounds, Direction.Bottom);
                break;
        }

        if (_position != Vector2.zero)
            Instantiate(_enemy, _position, _rotation);

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
            if (!Physics2D.OverlapBox(_position, new Vector2(enemyBounds.bounds.size.x * 2, 
                                                             enemyBounds.bounds.size.y * 2), 0, AvoidLayer))
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
