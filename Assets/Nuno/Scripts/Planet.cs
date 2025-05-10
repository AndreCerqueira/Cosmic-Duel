using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class Planet : MonoBehaviour
{
    /* ---------- Inspector ---------- */
    [Header("Referências externas")]
    [SerializeField] private ShipMover ship;
    [SerializeField] private LineRenderer line;
    [SerializeField] private TextMeshPro costLabel;
    [SerializeField] private FuelSystem fuel;
    [SerializeField] private TextMeshPro fuelCostLabel;

    [Header("Shader property")]
    [SerializeField] private string auraColorProperty = "_OutlineColor";

    [Header("Cores")]
    [SerializeField] private Color hoverColor = Color.white;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color idleColor = new Color(1, 1, 1, 0);

    [Header("Economia")]
    [SerializeField] private float costPerUnit = 10f;
    [SerializeField] private float dashLength = 0.25f;

    [Header("UI do custo de combustível")]
    [SerializeField] private Color fuelLabelColor = Color.red;

    /* ---------- estado ---------- */
    private bool hovering;
    private bool selected;
    private bool isInitialized;

    private Material auraMat;
    private static Planet currentTarget;
    private static readonly int AURA_ID = Shader.PropertyToID("_OutlineColor");

    /* ---------- cache de texto ---------- */
    private string cachedCostText = "";
    private string cachedFuelText = "";

    /* ---------- SETUP ---------- */
    private void Awake()
    {
        auraMat = GetComponent<SpriteRenderer>().material;
        SetAura(idleColor);
    }

    /* ---------- EVENTOS DE RATO ---------- */
    private void OnMouseEnter()
    {
        hovering = true;
        if (!selected) SetAura(hoverColor);

        line.enabled = true;
        costLabel.gameObject.SetActive(true);
        fuelCostLabel.gameObject.SetActive(true);

        RefreshInfo();                 // <-- calcula só uma vez
    }

    private void OnMouseExit()
    {
        hovering = false;
        if (!selected) SetAura(idleColor);

        line.enabled = false;
        costLabel.gameObject.SetActive(false);
        fuelCostLabel.gameObject.SetActive(false);
    }

    private void OnMouseDown()
    {
        float distance = Vector3.Distance(transform.position, ship.transform.position);

        if (!fuel.TryConsumeForDistance(distance)) return;

        if (currentTarget != null && currentTarget != this)
            currentTarget.Deselect();

        currentTarget = this;
        selected = true;
        SetAura(selectedColor);

        ship.MoveTo(transform.position);

        //RefreshInfo();                 // recalcula se quiseres valor exacto após clique
    }

    /* ---------- LOOP ---------- */
    private void Update()
    {
        if (!hovering || !isInitialized) return;

        Vector3 shipPos = ship.transform.position;
        Vector3 planetPos = transform.position;

        // Actualiza apenas posições da UI e linha
        fuelCostLabel.transform.position = shipPos + Vector3.up * 1f;
        costLabel.transform.position = planetPos + Vector3.down * 20f;

        float distReal = Vector3.Distance(shipPos, planetPos);
        UpdateLine(shipPos, planetPos, distReal);
    }

    /* ---------- cálculo pontual ---------- */
    private void RefreshInfo()
    {
        Vector3 shipPos = ship.transform.position;
        Vector3 planetPos = transform.position;
        float dist = Vector3.Distance(shipPos, planetPos);

        // custo em créditos
        float cost = dist * costPerUnit;
        cachedCostText = $"{cost:0} ×10¹² km";
        costLabel.text = cachedCostText;

        // combustível
        float fuelNeeded = dist * fuel.FuelPerUnit;
        float percentSpend = fuelNeeded / fuel.MaxFuel * 100f;
        int percInt = Mathf.CeilToInt(percentSpend);
        cachedFuelText = $"-{percInt} <voffset=0.9em><sprite name=fuel></voffset>";
        fuelCostLabel.text = cachedFuelText;
    }

    /* ---------- HELPERS ---------- */
    private void Deselect()
    {
        selected = false;
        SetAura(hovering ? hoverColor : idleColor);
    }

    private void SetAura(Color c) => auraMat.SetColor(AURA_ID, c);

    private void UpdateLine(Vector3 a, Vector3 b, float dist)
    {
        line.positionCount = 2;
        line.SetPosition(0, a);
        line.SetPosition(1, b);

        line.textureMode = LineTextureMode.Tile;
        line.material.mainTextureScale = new Vector2(dist / dashLength, 1f);
    }

    /* ---------- API usado pelo Spawner ---------- */
    public void Setup(ShipMover s, LineRenderer l, TextMeshPro label,
                      FuelSystem f, TextMeshPro fuelCostLabel)
    {
        ship = s;
        line = l;
        costLabel = label;
        fuel = f;
        this.fuelCostLabel = fuelCostLabel;
        this.fuelCostLabel.color = fuelLabelColor;

        isInitialized = true;
    }
}
