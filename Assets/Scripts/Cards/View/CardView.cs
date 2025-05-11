using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Cards.Models;
using Cards.View;
using Match;
using Project.Runtime.Scripts.Game.Matches;
using TMPro;
using UnityEngine;

namespace Cards.View
{
    public class CardView : MonoBehaviour
    {
        private static MatchPlayer SelfMatchPlayer => MatchController.Instance.SelfPlayer;
        
        [NonSerialized] public CardInputHandler InputHandler;
        
        [Header("Borders")]
        [SerializeField] private Sprite _goldBorder;
        [SerializeField] private Sprite _silverBorder;
        [SerializeField] private Sprite _bronzeBorder;
        
        [Header("Card")]
        
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private SpriteRenderer _artwork;
        [SerializeField] private SpriteRenderer _border;
        [SerializeField] private GameObject _wrapper;
        [SerializeField] private GameObject _outline;
        
        [SerializeField] private GameObject _redDetail;
        [SerializeField] private GameObject _blueDetail;
        
        [SerializeField] private List<Transform> _costs; // in order
        
        public Card Card { get; private set; }
        
        private void OnEnable()
        {
            SelfMatchPlayer.OnEnergyChanged += UpdateOutlineState;
        }

        private void OnDisable()
        {
            try
            {
                SelfMatchPlayer.OnEnergyChanged -= UpdateOutlineState;
            }
            catch (Exception _)
            {
                // ignored
            }
        }

        public void Setup(Card card)
        {
            Card = card;
            _name.text = card.Name;
            _description.text = CardDescriptionFormatter.Resolve(card, SelfMatchPlayer);// FormatDescriptionWithEffects(card.Description, card.Effects);
            
            for (int i = 0; i < _costs.Count; i++)
            {
                _costs[i].gameObject.SetActive(i == card.Cost);
            }
            
            _border.sprite = card.BorderType switch
            {
                BorderType.Golden => _goldBorder,
                BorderType.Silver => _silverBorder,
                BorderType.Copper => _bronzeBorder,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _redDetail.SetActive(card.HaveRedDetail);
            _blueDetail.SetActive(card.HaveBlueDetail);
            
            _artwork.sprite = card.Artwork;
            
            InputHandler = GetComponent<CardInputHandler>();

            UpdateOutlineState(SelfMatchPlayer.Energy);
        }
        

        private void UpdateOutlineState(int energy)
        {
            if (energy >= Card.Cost)
            {
                _outline.SetActive(true);
            }
            else
            {
                _outline.SetActive(false);
            }
        }

        public static string ReplaceTextIcons(string description)
        {
            // Substituir {Text-Icons/defense.png} pelo quad e carregar a textura correspondente
            description = description.Replace("{Text-Icons/defense.png}", " <sprite name=\"defense\">");
            description = description.Replace("{Text-Icons/attack.png}", " <sprite name=\"attack\">");
            description = description.Replace("{Text-Icons/attack-temp.png}", " <sprite name=\"attack-temp\">");
            description = description.Replace("{Text-Icons/coin.png}", " <sprite name=\"coin\">");
            description = description.Replace("{Text-Icons/energy.png}", " <sprite name=\"energy\">");

            return description;
        }
        
        public static string HighlightNumbers(string text)
        {
            // Substitui números inteiros (não parte de palavras) por versão colorida
            return Regex.Replace(text, @"\b(\d+)\b", "<color=#FFA500>$1</color>");
        }
        
    }
}


public static class CardDescriptionFormatter
{
    /// <summary>
    /// Substitui {0}, {1}… pelos valores dos efeitos, depois aplica
    /// as outras formatações (keywords, icones, cor dos números, \n, …).
    /// </summary>
    public static string Resolve(Card card, MatchPlayer owner)
    {
        // 1. Constrói o array com os amounts já modificados.
        object[] values = card.Effects
            .Select(e => (object)(
                e is DamageEffectPlain ? e.amount + StatusManager.Instance.DamageBonus :
                e is GainArmorEffect ? e.amount + StatusManager.Instance.ArmorBonus :
                e.amount
            ))
            .ToArray();

        // 2. Faz o string.Format.  Se faltar ou sobrar placeholder,
        //    capturamos para não explodir em runtime.
        string raw;
        try
        {
            raw = string.Format(card.Description, values);
        }
        catch (FormatException)
        {
            Debug.LogError(
                $"Descrição «{card.Description}» não bate com " +
                $"{card.Effects.Count} efeito(s).");
            raw = card.Description; // devolve mesmo assim
        }

        // 3. Devolve já passado pelos teus efeitos visuais.
        return CardViewStatic.FormatDescriptionStatic(raw);
    }
}

public static class CardViewStatic
{
    public static string FormatDescriptionStatic(string description)
    {
        description = CardView.ReplaceTextIcons(description);
        description = CardView.HighlightNumbers(description);
        return description;
    }
}