using System.Collections.Generic;
using UnityEngine;

public class ChunkLoader : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform _player;
    [SerializeField] DestroyArea _destroyArea;

    [Header("Chunk Settings")]
    [SerializeField] Chunk[] _chunkPrefabs;
    [SerializeField] int _preloadCount = 4;

    Queue<Chunk> _activeChunks = new Queue<Chunk>();

    float _nextSpawnY = 0f;

    void Start()
    {
        for (int i = 0; i < _preloadCount; i++)
        {
            SpawnChunk();
        }
    }

    void Update()
    {
        RemoveOldChunks();
    }

    void SpawnChunk()
    {
        Chunk prefab = _chunkPrefabs[Random.Range(0, _chunkPrefabs.Length)];

        Chunk chunk = Instantiate(
            prefab,
            new Vector3(transform.position.x, _nextSpawnY, 0f),
            Quaternion.identity,
            transform
        );

        _activeChunks.Enqueue(chunk);
        _nextSpawnY += chunk._height;
    }

    void RemoveOldChunks()
    {
        if (_activeChunks.Count == 0) return;

        Chunk bottom = _activeChunks.Peek();

        if (bottom._topPoint.position.y < _destroyArea.TopY)
        {
            Destroy(_activeChunks.Dequeue().gameObject);
            SpawnChunk();
        }
    }
}
