using UnityEngine;
using System.Collections.Generic;

// A very simple object pooling class
public class SimpleObjectPool : MonoBehaviour
{
    // the GameObject that this object pool returns instances of
    public GameObject fanRow;
    // collection of currently inactive instances of the fanRow
    private Stack<GameObject> inactiveInstances = new Stack<GameObject>();

    // Returns an instance of the fanRow
    public GameObject GetObject()
    {
        GameObject spawnedGameObject;

        // if there is an inactive instance of the fanRow ready to return, return that
        if (inactiveInstances.Count > 0)
        {
            // remove the instance from teh collection of inactive instances
            spawnedGameObject = inactiveInstances.Pop();
        }
        // otherwise, create a new instance
        else
        {
            spawnedGameObject = (GameObject)GameObject.Instantiate(fanRow);

            // add the PooledObject component to the fanRow so we know it came from this pool
            PooledObject pooledObject = spawnedGameObject.AddComponent<PooledObject>();
            pooledObject.pool = this;
        }

        // put the instance in the root of the scene and enable it
        spawnedGameObject.transform.SetParent(null);
        spawnedGameObject.SetActive(true);

        // return a reference to the instance
        return spawnedGameObject;
    }

    // Return an instance of the fanRow to the pool
    public void ReturnObject(GameObject toReturn)
    {
        PooledObject pooledObject = toReturn.GetComponent<PooledObject>();

        // if the instance came from this pool, return it to the pool
        if (pooledObject != null && pooledObject.pool == this)
        {
            // make the instance a child of this and disable it
            toReturn.transform.SetParent(transform);
            toReturn.SetActive(false);

            // add the instance to the collection of inactive instances
            inactiveInstances.Push(toReturn);
        }
        // otherwise, just destroy it
        else
        {
            Debug.LogWarning(toReturn.name + " was returned to a pool it wasn't spawned from! Destroying.");
            Destroy(toReturn);
        }
    }
}

// a component that simply identifies the pool that a GameObject came from
public class PooledObject : MonoBehaviour
{
    public SimpleObjectPool pool;
}