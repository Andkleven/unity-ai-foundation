using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
[CreateAssetMenu]
public class RandomTerrainData : ScriptableObject {
  [Range(0, 500f)] public float scale = 100f;
}