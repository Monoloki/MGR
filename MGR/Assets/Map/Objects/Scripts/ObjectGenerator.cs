using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    public int gridWidth = 14; 
    public int gridHeight = 8; 
    public float cellSize = 1.0f; 
    public GameObject[] prefabsToSpawn; 
    public int[] numberOfObjects;
    public Transform parent;

    private void Start() {
        if (prefabsToSpawn.Length != numberOfObjects.Length) {
            Debug.LogError("No matching objects and numbers");
            return;
        }

        SpawnRandomObjects();
    }

    private void SpawnRandomObjects() {
        for (int i = 0; i < prefabsToSpawn.Length; i++) {
            SpawnObjects(numberOfObjects[i], prefabsToSpawn[i]);
        }
    }

    private void SpawnObjects(int amount, GameObject gameObject) {
        var occupiedPositions = new System.Collections.Generic.HashSet<Vector2Int>();
        for (int i = 0; i < amount; i++) {
            Vector2Int randomPosition;
            do {
                randomPosition = new Vector2Int(
                    Random.Range(-gridWidth, gridWidth),
                    Random.Range(-gridHeight, gridHeight)
                );
            }
            while (occupiedPositions.Contains(randomPosition));

            occupiedPositions.Add(randomPosition);

            Vector3 worldPosition = new Vector3(
                randomPosition.x * cellSize,
                randomPosition.y * cellSize,
                0
            );

            GameObject spawnedObject = Instantiate(gameObject, worldPosition, Quaternion.identity);

            if (parent != null) {
                spawnedObject.transform.SetParent(parent);
            }
        }
    }
}
