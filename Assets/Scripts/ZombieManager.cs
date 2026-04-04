using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private Vector2 _sizeSpawnZone = new Vector2(5.3f, 3f);
    [SerializeField] private List<GameObject> _listZombie = new List<GameObject>();
    [SerializeField] private List<string> _listNameZombies = new List<string>();
    
    public void SpawnZombie(Vector2 pos, string name)
    {
        GameObject obj = Instantiate(_listZombie[_listNameZombies.IndexOf(name)],
            new Vector3(pos.x * _sizeSpawnZone.x - _sizeSpawnZone.x / 2, 0, (1f - pos.y) * _sizeSpawnZone.y - _sizeSpawnZone.y / 2), Quaternion.identity);
    }
}
