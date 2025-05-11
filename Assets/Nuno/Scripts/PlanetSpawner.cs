using System.Collections.Generic;
using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    /* ---------- Inspector ---------- */

    [Header("Câmara-alvo (vazio → Camera.main)")]
    [SerializeField] private Camera targetCamera;

    [Header("Prefab do planeta")]
    [SerializeField] private GameObject planetPrefab;

    [Header("Popup Prefab")]
    [SerializeField] private PlanetPopup popupPrefab;


    [Header("Sprites possíveis")]
    [SerializeField] private List<Sprite> planetSprites;

    [Header("Quantos planetas criar")]
    [SerializeField] private int planetCount = 6;

    [Header("Distância mínima entre planetas")]
    [SerializeField] private float minDistance = 1f;

    [Header("Padding")]
    [SerializeField] private Vector2 padding = new(1f, 1f);

    [Header("Dependências")]
    [SerializeField] private ShipMover ship;
    [SerializeField] private LineRenderer line;
    [SerializeField] private FuelSystem fuelSystem;

    [Header("Probabilidades")]
    [SerializeField] private float bossChance = 0.15f;  // 15 %
    [SerializeField] private float hiddenChance = 0.25f;  // 25 %

    [Header("LayerMask Blocking")]
    [SerializeField] private LayerMask blockingMask;

    /* ---------- limites ---------- */
    private float minX, maxX, minY, maxY;

    /* ---------- Awake ---------- */
    private void Awake()
    {
        if (!targetCamera) targetCamera = Camera.main;
        if (!targetCamera || !targetCamera.orthographic)
        { enabled = false; return; }

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

        /* A) – já existe estado */
        if (gm.planets.Count > 0)
        {
            Rebuild(gm.planets);
            fuelSystem.SetFuel(gm.currentFuel);
            return;
        }

        /* B) – primeiro load */
        if (planetSprites.Count == 0 || planetPrefab == null) return;

        List<Sprite> shuffled = new(planetSprites);
        Shuffle(shuffled);

        for (int i = 0; i < planetCount; i++)
        {
            Sprite spr = shuffled[i % shuffled.Count];
            float rad = Mathf.Max(spr.bounds.size.x, spr.bounds.size.y) * 0.5f;
            float safety = rad + minDistance;

            if (!TryFindFree(safety, out Vector3 pos)) continue;

            Planet.Difficulty diff = Random.value < bossChance
                ? Planet.Difficulty.Boss
                : (Planet.Difficulty)Random.Range(0, 3);   // Easy–Hard

            bool hidden = Random.value < hiddenChance;

            CreateAndRegisterPlanet(pos, diff, hidden, spr, planetSprites.IndexOf(spr));
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

    /* ---------- helpers ---------- */
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
        p.SetPopupPrefab(popupPrefab);
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
            difficulty = diff,     // <-- enum, não int
            hidden = hidden,
            completed = false,
            spriteIndex = sprIdx
        });
    }

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
