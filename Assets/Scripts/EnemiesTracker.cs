using System.Collections.Generic;
using UnityEngine;

public class EnemiesTracker : MonoBehaviour
{
    public static EnemiesTracker Instance { get; private set; }

    [SerializeField] private List<Transform> _enemies = new List<Transform>();
    public IReadOnlyList<Transform> Enemies => _enemies;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public void Register(Transform enemy)
    {
        if (!_enemies.Contains(enemy))
            _enemies.Add(enemy);
    }

    public void Unregister(Transform enemy) => _enemies.Remove(enemy);
}
