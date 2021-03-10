using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SpawnArea {
  Agent,
  Random,
}

public enum ScaleMethod {
  None,
  Equal,
  Random,
}

[ExecuteAlways]
public class Spawner : MonoBehaviour {
  public ObjectPooler objectPooler;
  public List<GameObject> spawnAreas;

  public bool SpawnAreaTest() {
    bool passed = true;

    // TODO: Make test to enforce correct spawn areas
    // Maybe automate or make spawn areas safer instead?

    if (!passed) Debug.Log("Some warning (Spawner.SpawnAreaTest())");
    return passed;
  }

  private void Start() {
    if (!SpawnAreaTest()) return;
    Spawn();
  }

  public void Spawn() {
    foreach (var pool in objectPooler.data.pools) {
      StartCoroutine(PeriodicObjectPoolSpawn(pool.poolName, 0.001f));
    }
  }

  public GameObject SpawnFromPool(string poolName, Vector3? position = null, Quaternion? rotation = null) {

    if (objectPooler.data == null || objectPooler.data.poolQueues == null) return null;
    if (!objectPooler.data.poolQueues.ContainsKey(poolName)) {
      Debug.Log("Pool named \"" + poolName + "\" doesn't exist.");
      return null;
    }
    if (objectPooler.data.poolQueues[poolName].Count.Equals(0)) return null;

    GameObject obj = objectPooler.data.poolQueues[poolName].Dequeue();

    if (obj == null) return null;

    obj.SetActive(true);

    Randomize(obj, objectPooler.data.poolOptions[poolName]);
    ResetObject(obj);

    // data.poolDictionary[pool].Enqueue(objectToSpawn);

    return obj;
  }

  void Randomize(GameObject obj, ObjectPoolerData.PoolOptions options) {
    // TODO: Spawn in spawn areas
    // --------------------------
    if (options.allowedSpawnAreas.Count > 0) {
      if (options.forbiddenSpawnAreas.Count > 0) {

      }

    }
    // --------------------------

    obj.transform.rotation = Quaternion.Euler(
      Random.Range(options.minScaleFactor, options.maxScaleFactor),
      Random.Range(options.minScaleFactor, options.maxScaleFactor),
      Random.Range(options.minScaleFactor, options.maxScaleFactor)
    );

    switch (options.scaleMethod) {
      case ScaleMethod.None:
        break;
      case ScaleMethod.Equal:
        float scaleFactor = Random.Range(options.minScaleFactor, options.maxScaleFactor);
        obj.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        break;
      case ScaleMethod.Random:
        obj.transform.localScale = RandomVector(options.minScaleFactor, options.maxScaleFactor);
        break;
      default:
        break;
    }
  }

  void ResetObject(GameObject obj) {
    obj.transform.position = transform.position;
    obj.transform.rotation = transform.rotation;
    if (!obj.isStatic) {
      var rb = GetComponent<Rigidbody>();
      if (rb != null) {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
      }
    }
  }

  private Vector3 RandomVector(float min, float max) {
    return new Vector3(
      Random.Range(min, max),
      Random.Range(min, max),
      Random.Range(min, max)
  );
  }

  IEnumerator PeriodicObjectPoolSpawn(string pool, float period) {
    for (; ; ) {
      if (objectPooler == null || SpawnFromPool(pool) == null) yield break;
      yield return new WaitForSeconds(period);
    }
  }
}
