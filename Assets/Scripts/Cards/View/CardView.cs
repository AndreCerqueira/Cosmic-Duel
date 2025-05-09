using System;
using System.Text.RegularExpressions;
using Cards.Models;
using TMPro;
using UnityEngine;

namespace Cards.View
{
    public class CardView : MonoBehaviour
    {
        [NonSerialized] public CardInputHandler InputHandler;
        
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private TMP_Text _energyCost;
        [SerializeField] private SpriteRenderer _artwork;
        [SerializeField] private GameObject _wrapper;
        
        public Card Card { get; private set; }

        public void Setup(Card card)
        {
            Card = card;
            _name.text = card.Name;
            _description.text = FormatDescription(card.Description);
            
            _energyCost.text = card.Cost.ToString();
            
            _artwork.sprite = card.Artwork;
            
            InputHandler = GetComponent<CardInputHandler>();
        }

        private string FormatDescription(string description)
        {
            var pattern = @"\{keyword:([^{}]+?)\}";
            description = Regex.Replace(description, pattern, "<color=#DDBE78>$1</color>");
            description = description.Replace(@"\n", "\n");
            description = ReplaceTextIcons(description);
            return description;
        }

        private string ReplaceTextIcons(string description)
        {
            // Substituir {Text-Icons/defense.png} pelo quad e carregar a textura correspondente
            description = description.Replace("{Text-Icons/defense.png}", " <sprite name=\"defense\">");
            description = description.Replace("{Text-Icons/attack.png}", " <sprite name=\"attack\">");
            description = description.Replace("{Text-Icons/attack-temp.png}", " <sprite name=\"attack-temp\">");
            description = description.Replace("{Text-Icons/coin.png}", " <sprite name=\"coin\">");
            description = description.Replace("{Text-Icons/energy.png}", " <sprite name=\"energy\">");

            return description;
        }
    }
}
