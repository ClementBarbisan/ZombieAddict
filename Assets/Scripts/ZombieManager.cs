using System;
using System.Collections.Generic;
using UnityEngine;

public class ZombieManager : MonoBehaviour
{
    [SerializeField] private Vector2 _sizeSpawnZone = new Vector2(5.3f, 3f);
    [SerializeField] private List<GameObject> _listZombie = new List<GameObject>();
    [SerializeField] private List<string> _listNameZombies = new List<string>();
    [SerializeField] private List<int> _maxZombie = new List<int>();
    private int[] _nbZombie;
    private List<List<EnemyController>> _zombies = new List<List<EnemyController>>();

    private void Start()
    {
        _nbZombie = new int[_listZombie.Count];
        _zombies = new List<List<EnemyController>>();
        for (int i = 0; i < _listZombie.Count; i++)
        {
            _zombies.Add(new List<EnemyController>());
        }
    }

    public void SpawnZombie(Vector2 pos, string name)
    {
        GameObject obj = Instantiate(_listZombie[_listNameZombies.IndexOf(name)],
            new Vector3(pos.x * _sizeSpawnZone.x - _sizeSpawnZone.x / 2, 0, (1f - pos.y) * _sizeSpawnZone.y - _sizeSpawnZone.y / 2), Quaternion.identity);
        EnemyController enemy = obj.GetComponent<EnemyController>();
        enemy.OnDeath.AddListener((EnemyController x) => DeleteZombie(x));
        _nbZombie[_listNameZombies.IndexOf(name)]++;
        _zombies[_listNameZombies.IndexOf(name)].Add(enemy);
        WebsocketManager.Zombies infos = new WebsocketManager.Zombies();
        infos.type = name;
        infos.maxZombie = _maxZombie[_listNameZombies.IndexOf(name)];
        infos.nbZombie = _nbZombie[_listNameZombies.IndexOf(name)];
        WebsocketManager.Instance.SendZombieMessage(infos);
    }

    private void DeleteZombie(EnemyController enemy)
    {
        int indexList = -1; 
        int indexObj = -1;
        EnemyController delete = null;
        for (int i = 0; i <_zombies.Count; i++)
        {
            indexList = i;
            for (int j = 0; j < _zombies[i].Count; j++)
            {
                if (_zombies[i][j] == enemy)
                {
                    indexObj = j;
                    delete = _zombies[i][j];
                    break;
                }
            }
            if (delete)
            {
                break;
            }
        }
        if (indexObj > -1)
        {
            _nbZombie[indexList]--;
            _zombies[indexList].Remove(delete);
        }
        WebsocketManager.Zombies infos = new WebsocketManager.Zombies();
        infos.type = name;
        infos.maxZombie = _maxZombie[_listNameZombies.IndexOf(name)];
        infos.nbZombie = _nbZombie[_listNameZombies.IndexOf(name)];
        WebsocketManager.Instance.SendZombieMessage(infos);
    }
}
