using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniPool<T>
{
    private List<GameObject> list = new List<GameObject>();
    private Dictionary<GameObject,T> dict = new Dictionary<GameObject,T>();
    GameObject prefab;
    Transform parent;

    public void OnInit(GameObject prefab, Transform parent = null)
    {
        this.prefab = prefab;
        this.parent = parent;
    }

    public T Spawn(Vector3 pos, Quaternion rot)
    {
        GameObject go = null;

        for (int i = 0; i < list.Count; i++)
        {
            if (!list[i].activeInHierarchy)
            {
                go = list[i];
                break;
            }
        }

        if (go == null)
        {
            go = GameObject.Instantiate(prefab, parent);
            list.Add(go);
            dict.Add(go, go.GetComponent<T>());
        }

        go.transform.SetPositionAndRotation(pos, rot);
        go.SetActive(true);

        return dict[go];
    }

    public void Collect()
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].activeInHierarchy)
            {
                list[i].SetActive(false);
            }
        }
    }

    public void Release()
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameObject.Destroy(list[i]);
        }

        list.Clear();
    }

}
