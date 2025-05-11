using System.Collections.Generic;
using UnityEngine;

public class SpaceObjectSpawner : MonoBehaviour
{
    [Header("Câmara-alvo (vazio → Camera.main)")]
    [SerializeField] private Camera targetCamera;

    [Header("Prefab base (contém SpriteRenderer)")]
    [SerializeField] private GameObject objectPrefab;

    [Header("Sprites possíveis (serão ciclicamente exaustos)")]
    [SerializeField] private List<Sprite> objectSprites;

    [Header("Quantos objetos criar (primeiro load)")]
    [SerializeField] private int objectCount = 8;

    [Header("Distância mínima entre tudo")]
    [SerializeField] private float minDistance = 1f;

    [Header("Padding")]
    [SerializeField] private Vector2 padding = new Vector2(1f, 1f);

    [SerializeField] private LayerMask blockingMask;


    /* —— limites do frustum —— */
    private float minX, maxX, minY, maxY;

    /* —— pool de índices dos sprites —— */
    private readonly List<int> spritePool = new();

    /* ---------- Awake ---------- */
    private void Awake()
    {
        if (targetCamera == null) targetCamera = Camera.main;
        if (!targetCamera || !targetCamera.orthographic) { enabled = false; return; }

        float h = targetCamera.orthographicSize;
        float w = h * targetCamera.aspect;
        Vector3 cam = targetCamera.transform.position;

        minX = cam.x - w + padding.x; maxX = cam.x + w - padding.x;
        minY = cam.y - h + padding.y; maxY = cam.y + h - padding.y;
    }

    /* ---------- Start ---------- */
    private void Start()
    {
        if (!enabled) return;

        var gm = GameManager.Instance;
        var planets = gm.planets;
        var saved = gm.spaceObjects;

        /* A) — Reconstruir se já há estado */
        if (saved.Count > 0)
        {
            foreach (var s in saved)
                CreateVisual(s.position, s.spriteIndex);
            return;
        }

        /* B) — Primeira vez */
        if (objectPrefab == null || objectSprites.Count == 0) return;

        // 1) cria “pool” de índices e baralha
        RefillAndShufflePool();

        // 2) lista de posições ocupadas (planetas + objetos já criados)
        List<Vector3> occupied = new();
        foreach (var p in planets) occupied.Add(p.position);

        for (int i = 0; i < objectCount; i++)
        {
            if (!TryGeneratePosition(occupied, out Vector3 pos)) continue;

            int sprIdx = GetSpriteFromPool();
            CreateVisual(pos, sprIdx);

            occupied.Add(pos);
            saved.Add(new GameManager.SpaceObjectState
            {
                position = pos,
                spriteIndex = sprIdx
            });
        }
    }

    /* ---------- tentativa de posição livre ---------- */
    /* ---------- tentativa de posição livre ---------- */
    private bool TryGeneratePosition(List<Vector3> occupied, out Vector3 pos)
    {
        for (int attempts = 0; attempts < 20; attempts++)
        {
            pos = RandomPoint();

            /* A)  distância aos que já estão na lista */
            bool ok = true;
            foreach (Vector3 v in occupied)
            {
                if ((v - pos).sqrMagnitude < minDistance * minDistance)
                { ok = false; break; }
            }

            /* B)  distância a QUALQUER collider 2D na Layer “Blocking” */
            if (ok)
            {
                // usa o mesmo raio – adiciona margem se quiseres
                ok = !Physics2D.OverlapCircle(pos, minDistance, blockingMask);
            }

            if (ok) return true;           // posição válida! sai do ciclo
        }

        pos = Vector3.zero;                // falhou 20 tentativas
        return false;
    }


    /* ---------- Sprite pool helpers ---------- */
    private void RefillAndShufflePool()
    {
        spritePool.Clear();
        for (int i = 0; i < objectSprites.Count; i++) spritePool.Add(i);
        Shuffle(spritePool);
    }

    private int GetSpriteFromPool()
    {
        if (spritePool.Count == 0) RefillAndShufflePool();
        int idx = spritePool[0];
        spritePool.RemoveAt(0);
        return idx;
    }

    /* ---------- cria visual ---------- */
    private void CreateVisual(Vector3 pos, int spriteIdx)
    {
        GameObject go = Instantiate(objectPrefab, pos, Quaternion.identity);
        go.GetComponent<SpriteRenderer>().sprite = objectSprites[spriteIdx];
    }

    /* ---------- util ---------- */
    private Vector3 RandomPoint() =>
        new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
