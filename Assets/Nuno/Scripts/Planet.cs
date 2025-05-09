using UnityEngine;
using TMPro;

[RequireComponent(typeof(Collider2D))]
public class Planet : MonoBehaviour
{
    /* ---------- Inspector ---------- */
    [Header("Referências")]
    [SerializeField] private ShipMover ship;       // arrasta a Nave
    [SerializeField] private LineRenderer line;      // arrasta o LineRenderer
    [SerializeField] private TextMeshPro costLabel;  // arrasta o TextMeshPro (3D)

    [Header("Economia")]
    [SerializeField] private float costPerUnit = 10f;   // créditos por unidade
    [SerializeField] private float dashLength = 0.25f; // tamanho de cada traço

    /* ---------- estado interno ---------- */
    private bool hovering;   // true enquanto o rato está sobre o planeta

    /* ---------- eventos de rato ---------- */
    private void OnMouseEnter()
    {
        hovering = true;
        line.enabled = true;
        costLabel.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        hovering = false;
        line.enabled = false;
        costLabel.gameObject.SetActive(false);
    }

    private void OnMouseDown() => ship.MoveTo(transform.position);

    /* ---------- actualização por frame ---------- */
    private void Update()
    {
        if (!hovering) return;                 // só calcula enquanto o rato está em cima

        Vector3 shipPos = ship.transform.position;
        Vector3 planetPos = transform.position;

        float distance = Vector3.Distance(shipPos, planetPos);
        float cost = distance * costPerUnit;

        // 1) linha tracejada
        UpdateLine(shipPos, planetPos, distance);

        // 2) texto no meio
        costLabel.text = $"{cost:0} C";
        costLabel.transform.position = planetPos;
        costLabel.transform.position += Vector3.up * 2f; // levanta o texto
    }

    /* ---------- utilitário ---------- */
    private void UpdateLine(Vector3 shipPos, Vector3 planetPos, float distance)
    {
        line.positionCount = 2;
        line.SetPosition(0, shipPos);
        line.SetPosition(1, planetPos);

        // repetir a textura ao longo do comprimento
        line.textureMode = LineTextureMode.Tile;
        line.material.mainTextureScale = new Vector2(distance / dashLength, 1f);
    }
}
