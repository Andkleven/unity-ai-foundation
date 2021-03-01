// using UnityEngine;

// [RequireComponent(typeof(MeshFilter))]
// [ExecuteAlways]
// public class Ground : MonoBehaviour {
//   Mesh mesh;
//   MeshCollider meshCollider;
//   float xScale;
//   float zScale;
//   int xNodes;
//   int zNodes;
//   Vector3[] vertices;
//   int[] triangles;

//   [Header("Dependencies")]
//   public EnvironmentSettings env;

//   [Header("Mesh Options")]
//   public float resolution = 0.25f;
//   [Range(0, 1f)] public float unevenness = 0.1f;
//   public bool centered = true;
//   public float maxDisplacement = -1.25f;
//   private float displacement;

//   [Header("Mesh Borders")]
//   public bool borders = true;
//   public float borderHeight = 0;
//   public int borderThickness = 1;

//   void Awake() {
//     if (env == null) env = GetComponentInParent<EnvironmentSettings>();
//   }

//   void Start() {
//     Generate();
//   }

//   public void Generate() {
//     bool simple = Random.Range(0, 1f) > 0.5f ? true : false;

//     if (simple) {
//       xNodes = 2;
//       zNodes = 2;
//     } else {
//       float[] resolutionFactors = { 1f, 0.5f, 0.25f };
//       xNodes = (int)Mathf.Round(resolution * env.Width);
//       zNodes = (int)Mathf.Round(resolution * env.Length);
//     }

//     xScale = (env.Width / xNodes);
//     zScale = (env.Length / zNodes);

//     mesh = new Mesh();
//     GetComponent<MeshFilter>().mesh = mesh;

//     bool flat = Random.Range(0, 1f) > 0.5f ? true : false;
//     displacement = flat ? 0 : Random.Range(0, maxDisplacement);

//     CreateShape();
//     UpdateMesh();

//     meshCollider = new MeshCollider();
//     GetComponent<MeshCollider>().sharedMesh = mesh;
//   }

//   void CreateShape() {
//     vertices = new Vector3[(xNodes + 1) * (zNodes + 1)];

//     float xStart = centered ? -zNodes / 2 : 0;
//     float xEnd = centered ? zNodes / 2 : zNodes;
//     float zStart = centered ? -xNodes / 2 : 0;
//     float zEnd = centered ? xNodes / 2 : xNodes;

//     for (float i = 0, z = zStart; z <= zEnd; z++) {
//       for (float x = xStart; x <= xEnd; x++) {
//         float y;

//         if (
//           borders &
//           x <= xStart + borderThickness |
//           x >= xEnd - borderThickness |
//           z <= zStart + borderThickness |
//           z >= zEnd - borderThickness
//         ) {
//           y = borderHeight;
//         } else {
//           y = Mathf.PerlinNoise(x * unevenness, z * unevenness) * maxDisplacement * env.difficulty;
//         }

//         vertices[(int)i] = new Vector3(x * xScale, y, z * zScale);
//         i++;
//       }
//     }

//     triangles = new int[xNodes * zNodes * 6];
//     int vertexCount = 0;
//     int triangleCount = 0;

//     for (int z = 0; z < zNodes; z++) {
//       for (int x = 0; x < xNodes; x++) {
//         triangles[triangleCount + 0] = vertexCount + 0;
//         triangles[triangleCount + 1] = vertexCount + xNodes + 1;
//         triangles[triangleCount + 2] = vertexCount + 1;
//         triangles[triangleCount + 3] = vertexCount + 1;
//         triangles[triangleCount + 4] = vertexCount + xNodes + 1;
//         triangles[triangleCount + 5] = vertexCount + xNodes + 2;

//         vertexCount++;
//         triangleCount += 6;
//       }
//       vertexCount++;
//     }

//   }

//   void UpdateMesh() {
//     mesh.Clear();

//     mesh.vertices = vertices;
//     mesh.triangles = triangles;

//     mesh.RecalculateNormals();
//   }
// }