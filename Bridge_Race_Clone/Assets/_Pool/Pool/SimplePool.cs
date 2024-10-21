///
/// Simple pooling for Unity.
///   Author: Martin "quill18" Glaude (quill18@quill18.com)
///   Latest Version: https://gist.github.com/quill18/5a7cfffae68892621267
///   License: CC0 (http://creativecommons.org/publicdomain/zero/1.0/)
///   UPDATES:
/// 	2015-04-16: Changed Pool to use a Stack generic. 


using UnityEngine;
using System.Collections.Generic;
using System;

public static class SimplePool
{
    const int DEFAULT_POOL_SIZE = 3;

    static Dictionary<int, Pool> pools = new Dictionary<int, Pool>();

    static Dictionary<PoolType, GameUnit> poolTypes = new Dictionary<PoolType, GameUnit>();

    private static GameUnit[] gameUnitResources;

    private static Transform root;

    public static Transform Root
    {
        get
        {
            if (root == null)
            {
                root = GameObject.FindObjectOfType<PoolControler>().transform;

                if (root == null)
                {
                    root = new GameObject("Pool").transform;
                }
            }

            return root;
        }
    }

    class Pool
    {
        Transform m_sRoot = null;

        bool m_collect;

        Queue<GameUnit> inactive;

        //collect obj active ingame
        List<GameUnit> active;

        // The prefab that we are pooling
        GameUnit prefab;

        bool m_clamp;
        int m_Amount;

        public bool isCollect { get => m_collect; }

        // Constructor
        public Pool(GameUnit prefab, int initialQty, Transform parent, bool collect, bool clamp)
        {
            inactive = new Queue<GameUnit>(initialQty);
            m_sRoot = parent;
            this.prefab = prefab;
            m_collect = collect;
            this.m_clamp = clamp;
            if (m_collect) active = new List<GameUnit>();
            if (m_clamp) m_Amount = initialQty;
        }
        public int Count {
            get { return inactive.Count;}
        }
        // Spawn an object from our pool
        public GameUnit Spawn(Vector3 pos, Quaternion rot)
        {
            GameUnit obj = Spawn();

            obj.TF.SetPositionAndRotation( pos, rot);

            return obj;
        }

        public GameUnit Spawn()
        {
            GameUnit obj;
            if (inactive.Count == 0)
            {
                obj = (GameUnit)GameObject.Instantiate(prefab, m_sRoot);

                if (!pools.ContainsKey(obj.GetInstanceID()))
                    pools.Add(obj.GetInstanceID(), this);
            }
            else
            {
                // Grab the last object in the inactive array
                obj = inactive.Dequeue();

                if (obj == null)
                {
                    return Spawn();
                }
            }

            if (m_collect) active.Add(obj);
            if (m_clamp && active.Count >= m_Amount) Despawn(active[0]);

            obj.gameObject.SetActive(true);

            return obj;
        }

        // Return an object to the inactive pool.
        public void Despawn(GameUnit obj)
        {
            if (obj != null /*&& !inactive.Contains(obj)*/)
            {
                obj.gameObject.SetActive(false);
                inactive.Enqueue(obj);
            }

            if (m_collect) active.Remove(obj);
        }

        public void Clamp(int amount) {
            while(inactive.Count> amount) {
                GameUnit go = inactive.Dequeue();
                GameObject.DestroyImmediate(go);
            }
        }
        public void Release() {
            while(inactive.Count>0) {
                GameUnit go = inactive.Dequeue();
                GameObject.DestroyImmediate(go);
            }
            inactive.Clear();
        }

        public void Collect()
        {
            while (active.Count > 0)
            {
                Despawn(active[0]);
            }
        }
    }

    // All of our pools
    static Dictionary<int, Pool> poolInstanceID = new Dictionary<int, Pool>();

    static void Init(GameUnit prefab = null, int qty = DEFAULT_POOL_SIZE, Transform parent = null, bool collect = false, bool clamp = false)
    {
        if (prefab != null && !IsHasPool(prefab.GetInstanceID()))
        {
            poolInstanceID.Add(prefab.GetInstanceID(), new Pool(prefab, qty, parent, collect, clamp));
        }
    }

    static public bool IsHasPool(int instanceID)
    {
        return poolInstanceID.ContainsKey(instanceID);
    }

    static public void Preload(GameUnit prefab, int qty = 1, Transform parent = null, bool collect = false, bool clamp = false)
    {
        if (!poolTypes.ContainsKey(prefab.poolType))
        {
            poolTypes.Add(prefab.poolType, prefab);
        }

        if (prefab == null)
        {
            Debug.LogError(parent.name + " : IS EMPTY!!!");
            return;
        }

        Init(prefab, qty, parent, collect, clamp);

        // Make an array to grab the objects we're about to pre-spawn.
        GameUnit[] obs = new GameUnit[qty];
        for (int i = 0; i < qty; i++)
        {
            obs[i] = Spawn(prefab);        
        }

        // Now despawn them all.
        for (int i = 0; i < qty; i++)
        {
            Despawn(obs[i]);
        }
    }

    static public T Spawn<T>(PoolType poolType, Vector3 pos, Quaternion rot) where T : GameUnit
    {
        return Spawn(GetGameUnitByType(poolType), pos, rot) as T;
    }
    
    static public T Spawn<T>(PoolType poolType) where T : GameUnit
    {
        return Spawn<T>(GetGameUnitByType(poolType));
    }

    static public T Spawn<T>(GameUnit obj, Vector3 pos, Quaternion rot) where T : GameUnit
    {
        return Spawn(obj, pos, rot) as T;
    }

    static public T Spawn<T>(GameUnit obj) where T : GameUnit
    {
        return Spawn(obj) as T;
    }

    static public GameUnit Spawn(GameUnit obj, Vector3 pos, Quaternion rot)
    {
        if (!poolInstanceID.ContainsKey(obj.GetInstanceID()))
        {
            Transform newRoot = new GameObject(obj.name).transform;
            newRoot.SetParent(Root);
            Preload(obj, 1, newRoot, true, false);
        }

        return poolInstanceID[obj.GetInstanceID()].Spawn(pos, rot);
    }

    public static GameUnit Spawn(GameUnit obj)
    {
        if (!poolInstanceID.ContainsKey(obj.GetInstanceID()))
        {
            Transform newRoot = new GameObject(obj.name).transform;
            newRoot.SetParent(Root);
            Preload(obj, 1, newRoot, true, false);
        }

        return poolInstanceID[obj.GetInstanceID()].Spawn();
    }

    static public void Despawn(GameUnit obj)
    {
        if (obj.gameObject.activeSelf)
        {
            if (pools.ContainsKey(obj.GetInstanceID()))
                pools[obj.GetInstanceID()].Despawn(obj);
            else
                GameObject.Destroy(obj.gameObject);    
        }
    }

    static public void Release(GameUnit obj)
    {
        if (pools.ContainsKey(obj.GetInstanceID()))
        {
            pools[obj.GetInstanceID()].Release();
            pools.Remove(obj.GetInstanceID());
        }
        else
        {
            GameObject.DestroyImmediate(obj);
        }
    }

    static public void Collect(GameUnit obj)
    {
        if (poolInstanceID.ContainsKey(obj.GetInstanceID()))
            poolInstanceID[obj.GetInstanceID()].Collect();
    }

    static public void CollectAll()
    {
        foreach (var item in poolInstanceID)
        {
            if (item.Value.isCollect)
            {
                item.Value.Collect();
            }
        }
    }

    static GameUnit GetGameUnitByType(PoolType poolType)
    {
        if (gameUnitResources == null || gameUnitResources.Length == 0)
        {
            gameUnitResources = Resources.LoadAll<GameUnit>("Pool");
        }

        if (!poolTypes.ContainsKey(poolType) || poolTypes[poolType] == null)
        {
            GameUnit unit = null;

            for (int i = 0; i < gameUnitResources.Length; i++)
            {
                if (gameUnitResources[i].poolType == poolType)
                {
                    unit = gameUnitResources[i];
                    break;
                }
            }

            poolTypes.Add(poolType, unit);
        }

        return poolTypes[poolType];
    }
}

[System.Serializable]
public class PoolAmount
{
    [Header("-- Pool Amount --")]
    public Transform root;
    public GameUnit prefab;
    public int amount;
    public bool collect;
    public bool clamp;
}

public enum IngameType 
{ 
    PLAYER, 
    ENEMY,
    None,
    HpBar,
}


public enum PoolType
{
    Bot,
    Brick,
}
