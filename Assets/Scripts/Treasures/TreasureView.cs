using System;
using System.Text.RegularExpressions;
using JetBrains.Annotations;
using Match;
using TMPro;
using UnityEngine;

namespace Treasures
{
    public class TreasureView : MonoBehaviour
    {
        private static MatchPlayer SelfMatchPlayer => MatchController.Instance.SelfPlayer;
        
        public TreasureInputHandler InputHandler => GetComponent<TreasureInputHandler>();
        
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

        public TreasureDataSO TreasureDataSO;

        public void Start() // Setup(TreasureDataSO data)
        {
            _name.text = TreasureDataSO.Name;
            _description.text = FormatDescription(TreasureDataSO.Description);
            
            _border.sprite = TreasureDataSO.BorderType switch
            {
                BorderType.Golden => _goldBorder,
                BorderType.Silver => _silverBorder,
                BorderType.Copper => _bronzeBorder,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _redDetail.SetActive(TreasureDataSO.HaveRedDetail);
            _blueDetail.SetActive(TreasureDataSO.HaveBlueDetail);
            
            _artwork.sprite = TreasureDataSO.Artwork;
        }
        
        // setup
        public void Setup(TreasureDataSO data)
        {
            TreasureDataSO = data;
            _name.text = TreasureDataSO.Name;
            _description.text = FormatDescription(TreasureDataSO.Description);
            
            _border.sprite = TreasureDataSO.BorderType switch
            {
                BorderType.Golden => _goldBorder,
                BorderType.Silver => _silverBorder,
                BorderType.Copper => _bronzeBorder,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            _redDetail.SetActive(TreasureDataSO.HaveRedDetail);
            _blueDetail.SetActive(TreasureDataSO.HaveBlueDetail);
            
            _artwork.sprite = TreasureDataSO.Artwork;
        }

        private string FormatDescription(string description)
        {
            var pattern = @"\{keyword:([^{}]+?)\}";
            description = Regex.Replace(description, pattern, "<color=#DDBE78>$1</color>");
            description = description.Replace(@"\n", "\n");
            description = HighlightNumbers(description);
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
        
        private string HighlightNumbers(string text)
        {
            // Substitui números inteiros (não parte de palavras) por versão colorida
            return Regex.Replace(
                text,
                @"(?<![\w])([+-]?\d+%?)(?![\w])",
                "<color=#FFA500>$1</color>"
            );
        }
    }
}
