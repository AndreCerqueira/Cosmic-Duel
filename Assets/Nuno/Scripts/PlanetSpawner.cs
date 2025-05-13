using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    /* ---------- Inspector ---------- */

    [Header("Câmara-alvo (vazio → Camera.main)")]
    [SerializeField] private Camera targetCamera;

    [Header("Prefab do planeta")]
    [SerializeField] private GameObject planetPrefab;

    [Header("Sprites possíveis")]
    [SerializeField] private List<Sprite> planetSprites;

    [Header("Quantos planetas criar")]
    [SerializeField] private int planetCount = 6;
    
    public int PlanetCount => planetCount;

    [Header("Distância mínima entre planetas")]
    [SerializeField] private float minDistance = 1f;

    [Header("Padding")]
    [SerializeField] private Vector2 padding = new(1f, 1f);

    [Header("Dependências")]
    [SerializeField] private ShipMover ship;
    [SerializeField] private LineRenderer line;
    [SerializeField] private FuelSystem fuelSystem;

    [Header("Probabilidades")]
    [SerializeField] private float bossChance = 0.15f;
    [SerializeField] private float hiddenChance = 0.25f;

    [Header("LayerMask Blocking")]
    [SerializeField] private LayerMask blockingMask;

    /* ---------- limites ---------- */
    private float minX, maxX, minY, maxY;

    /* ---------- pool de sprites ---------- */
    private readonly List<int> spritePool = new();   // guarda índices

    /* ---------- Awake ---------- */
    private void Awake()
    {
        if (!targetCamera) targetCamera = Camera.main;
        if (!targetCamera || !targetCamera.orthographic) { enabled = false; return; }

        float h = targetCamera.orthographicSize;
        float w = h * targetCamera.aspect;
        Vector3 c = targetCamera.transform.position;

        minX = c.x - w + padding.x; maxX = c.x + w - padding.x;
        minY = c.y - h + padding.y; maxY = c.y + h - padding.y;
    }

    /* ---------- Start ---------- */
    private void Start()
    {
        if (!enabled) return;

        var gm = GameManager.Instance;

        /* A) já existe estado salvo */
        if (gm.planets.Count > 0)
        {
            Rebuild(gm.planets);
            return;
        }

        /* B) primeiro load */
        if (planetSprites.Count == 0 || planetPrefab == null) return;

        for (int i = 0; i < planetCount; i++)
        {
            /* garante que há sprite para espreitar */
            if (spritePool.Count == 0) RefillAndShufflePool();

            int sprIdx = spritePool[0];               // PEEK (ainda não remove)
            Sprite spr = planetSprites[sprIdx];

            float radius = Mathf.Max(spr.bounds.size.x, spr.bounds.size.y) * 0.5f;
            float safetyR = radius + minDistance;

            /* tenta até 20 vezes achar posição livre para ESTE sprite */
            if (!TryFindFree(safetyR, out Vector3 pos))
            {
                Debug.LogWarning($"[PlanetSpawner] falhou posição para sprite {sprIdx}");
                spritePool.RemoveAt(0);   // descarta este sprite e continua
                i--;                      // não conta como planeta criado
                continue;
            }

            /* —— posição OK → agora sim removemos do pool —— */
            spritePool.RemoveAt(0);

            Planet.Difficulty diff = Random.value < bossChance
                ? Planet.Difficulty.Boss
                : (Planet.Difficulty)Random.Range(0, 3);

            bool hidden = Random.value < hiddenChance;

            CreateAndRegisterPlanet(pos, diff, hidden, spr, sprIdx);
        }

    }

    /* ---------- Rebuild ---------- */
    private void Rebuild(List<GameManager.PlanetState> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            var st = list[i];
            Sprite s = planetSprites[Mathf.Clamp(st.spriteIndex, 0, planetSprites.Count - 1)];

            Planet p = CreatePlanetVisual(st.position, st.difficulty, st.hidden, s, i);
            if (st.completed) p.MarkCompleted();
        }
    }

    /* ---------- sprite-pool helpers ---------- */
    private void RefillAndShufflePool()
    {
        spritePool.Clear();
        for (int i = 0; i < planetSprites.Count; i++) spritePool.Add(i);
        Shuffle(spritePool);
    }

    private int GetSpriteFromPool()
    {
        int idx = spritePool[0];
        spritePool.RemoveAt(0);
        return idx;
    }

    /* ---------- create helpers ---------- */
    private bool TryFindFree(float r, out Vector3 pos)
    {
        for (int t = 0; t < 20; t++)
        {
            pos = RandomPoint();
            if (!Physics2D.OverlapCircle(pos, r, blockingMask)) return true;
        }
        pos = Vector3.zero; return false;
    }

    private Planet CreatePlanetVisual(Vector3 pos, Planet.Difficulty diff, bool hidden,
                                      Sprite sprite, int index)
    {
        GameObject go = Instantiate(planetPrefab, pos, Quaternion.identity);
        Planet p = go.GetComponent<Planet>();

        p.PlanetIndex = index;
        p.SetName(NamePool.GetUniqueName());
        p.Setup(ship, line, fuelSystem, diff, hidden);
        go.GetComponent<SpriteRenderer>().sprite = sprite;
        return p;
    }

    private void CreateAndRegisterPlanet(Vector3 pos, Planet.Difficulty diff, bool hidden,
                                         Sprite sprite, int sprIdx)
    {
        var gm = GameManager.Instance;
        Planet p = CreatePlanetVisual(pos, diff, hidden, sprite, gm.planets.Count);

        gm.planets.Add(new GameManager.PlanetState
        {
            position = pos,
            difficulty = diff,
            hidden = hidden,
            completed = false,
            spriteIndex = sprIdx
        });
    }

    /* ---------- util ---------- */
    private Vector3 RandomPoint() =>
        new(Random.Range(minX, maxX), Random.Range(minY, maxY), 0f);

    private void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (list[i], list[j]) = (list[j], list[i]);
        }
    }
}
