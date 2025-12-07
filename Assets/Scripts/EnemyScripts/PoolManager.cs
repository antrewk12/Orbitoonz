using System.Collections.Generic;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    public GameObject bombPrefab;
    public int poolSize = 3;

    private List<GameObject> pool = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < poolSize; i++)
        {
            GameObject obj = Instantiate(bombPrefab);
            obj.SetActive(false);
            pool.Add(obj);

        }
    }
    public GameObject GetObjectFromPool(GameManager managerGame)
    {
        foreach (GameObject obj in pool)
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);

                BombScale bombScale = obj.GetComponent<BombScale>();
                if (bombScale != null)
                {
                    bombScale.ResetBomb();
                    
                }

                return obj;
            }
        }
        return null;
    }


    public void ReturnObjectToPool(GameObject obj)
    {
        BombScale bombScale = obj.GetComponent<BombScale>();

        if (bombScale != null)
        {
            bombScale.ResetBomb();
        }
        obj.SetActive(false);


    }
}

