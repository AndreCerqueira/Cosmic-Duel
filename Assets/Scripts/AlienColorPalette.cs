using Project.Runtime.Scripts.General;
using UnityEngine;

public class AlienColorPalette : Singleton<AlienColorPalette>
{
    
    private readonly string[] hexColors =
    {
        "#6A8F7B", "#4C6954", "#839C8A", "#95B1A0", "#B7CEC2", "#3A5447",
        "#2F5E5E", "#357A7F", "#508C8F", "#6DA5A8", "#8EB9BC", "#A7C9CC",
        "#B3B3B3", "#39354A", "#4C4664", "#625C7A", "#7A7592", "#978FAF", "#B4ABC7",
        "#27394D", "#304B63", "#3E607D", "#4E768F", "#608AA3", "#7A9FBA",
        "#95B3CC", "#B0C6D7", "#776F5F", "#8A8071", "#A19789", "#B8B1A5",
        "#D0C9BC", "#E5DED1", "#5C4631", "#745E46", "#8F7559", "#AB9371",
        "#50614A", "#5E7058", "#6E8268", "#729C6B", "#8BBF7D", "#ADCFA1",
        "#CCCCCC", "#E0E0E0", "#F4F4F4", "#4C7E89", "#6A9AA3"
    };
    
    private System.Random rng;
    
    protected override void Awake()
    {
        base.Awake();
        rng = new System.Random(); // inicializa o gerador de números aleatórios
    }

    // ---------- API pública ----------
    /// <summary>
    /// Devolve uma cor UnityEngine.Color aleatória da paleta.
    /// </summary>
    public Color GetRandomColor()
    {
        string hex = hexColors[rng.Next(hexColors.Length)];
        Color color;
        if (ColorUtility.TryParseHtmlString(hex, out color))
            return color;

        // Fallback improvável
        return Color.white;
    }

    /// <summary>
    /// Devolve apenas o código HEX (#RRGGBB) aleatório da paleta.
    /// </summary>
    public string GetRandomHex()
    {
        return hexColors[rng.Next(hexColors.Length)];
    }

}
