using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class Planet : MonoBehaviour
{
    /* ───────── Enums & dados externos ───────── */
    public enum Difficulty { Easy, Medium, Hard, Boss }

    [Header("Referências externas")]
    [SerializeField] private ShipMover ship;
    [SerializeField] private LineRenderer line;
    [SerializeField] private FuelSystem fuel;

    [Header("Popup")]
    [SerializeField] private PlanetPopup popupPrefab;   // arrasta o prefab
    [SerializeField] private Vector3 popupOffset = new(1.6f, 1.2f, 0f);

    public void SetPopupPrefab(PlanetPopup prefab) => popupPrefab = prefab;


    [Header("Dificuldade Ícones")]
    [SerializeField] private string[] diffSpriteNames = { "easy", "medium", "hard", "boss" };
    [SerializeField] private string unknownSprite = "unknown";

    private PlanetPopup popupInst;

    [Header("Shader Outline")]
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color idleColor = new(1, 1, 1, 0);
    private Material auraMat;
    private static readonly int AURA_ID = Shader.PropertyToID("_OutlineColor");
    private static readonly int THICKNESS = Shader.PropertyToID("_OutlineThickness");

    [Header("Custos")]
    [SerializeField] private float costPerUnit = 10f;   // créditos por unidade

    /* ───────── estado ───────── */
    public int PlanetIndex { get; set; }
    public Difficulty difficulty;
    public bool hidden;

    public string PlanetName { get; private set; } = "Planet-X";   // valor default
    public void SetName(string n) => PlanetName = n;

    private Vector3 popupLocalOffset;

    private bool hovering, selected, completed, initialised;

    private void Awake()
    {
        auraMat = GetComponent<SpriteRenderer>().material;
        SetAura(idleColor);

        /* calcula o deslocamento: canto sup-dir + pequeno “margem” */
        var sr = GetComponent<SpriteRenderer>();
        float margin = 0.15f;                       // afasta um pouco (unidades)
        popupLocalOffset = new Vector3(sr.bounds.extents.x + margin,
                                       sr.bounds.extents.y + margin,
                                       0f);
    }

    /* ───────── Hover ───────── */
    private void OnMouseEnter()
    {
        if (completed) return;
        hovering = true;
        if (!selected) SetAura(hoverColor);

        if (popupInst == null)
            popupInst = Instantiate(popupPrefab, transform, false);

        popupInst.gameObject.SetActive(true);
        RefreshPopup();
    }

    private void OnMouseExit()
    {
        hovering = false;
        if (!selected) SetAura(idleColor);

        if (popupInst != null)
            popupInst.gameObject.SetActive(false);
    }

    /* ───────── Clique / movimento ───────── */
    private void OnMouseDown()
    {
        if (completed) return;

        float dist = Vector3.Distance(transform.position, ship.transform.position);
        if (!fuel.TryConsumeForDistance(dist)) return;

        GameManager.Instance.shipPosition = ship.transform.position;
        ship.MoveTo(transform.position, OnShipArrived);

        selected = true;
        SetAura(selectedColor);
    }

    private void OnShipArrived()
    {
        GameManager.Instance.shipPosition = transform.position;
        GameManager.Instance.EnterPlanet(PlanetIndex);
    }

    /* ───────── Loop (actualiza popup) ───────── */
    private void Update()
    {
        if (!hovering || !initialised || popupInst == null || !popupInst.gameObject.activeSelf)
            return;

        RefreshPopup();
    }

    private void RefreshPopup()
    {
        Vector3 shipPos = ship.transform.position;
        float dist = Vector3.Distance(shipPos, transform.position);
        float fuelN = dist * fuel.FuelPerUnit;

        string distText = $"{dist:0} u";
        string fuelText = $"-{fuelN:0} C";

        string spriteName = hidden ? unknownSprite : diffSpriteNames[(int)difficulty];
        string diffName = hidden ? "unknown" : difficulty.ToString();

        popupInst.SetData(PlanetName, distText, fuelText, spriteName, diffName);

        popupInst.transform.localPosition = popupLocalOffset;
        popupInst.transform.localRotation = Quaternion.identity;

    }

    /* ───────── Concluir planeta ───────── */
    public void MarkCompleted()
    {
        completed = true;
        hidden = false;                           // revela dificuldade
        GetComponent<Collider2D>().enabled = false;  // bloqueia novas interacções

        if (popupInst != null) popupInst.gameObject.SetActive(false);

        var st = GameManager.Instance.planets[PlanetIndex];
        st.completed = true;
        st.hidden = false;
    }

    /* ───────── Helpers ───────── */
    private void SetAura(Color c)
    {
        auraMat.SetColor(AURA_ID, c);
        auraMat.SetFloat(THICKNESS, c.a < 0.01f ? 0f : 1f);
    }

    /* ───────── Setup pelo spawner ───────── */
    public void Setup(ShipMover s, LineRenderer l, FuelSystem f,
                      Difficulty diff, bool hidden)
    {
        ship = s;
        line = l;
        fuel = f;
        difficulty = diff;
        this.hidden = hidden;
        initialised = true;
    }
}
