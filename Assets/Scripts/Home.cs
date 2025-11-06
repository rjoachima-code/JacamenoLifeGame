using System;
using UnityEngine;

namespace Jacameno
{
    public enum HomeStyle
    {
        StudioApartment,
        OneBedroomCondo,
        SingleStoryHome,
        Villa,
        Mansion
    }

    /// <summary>
    /// HomeTemplate: data-driven template for property types (ScriptableObject-friendly if needed).
    /// Use these templates to present purchasable homes in the UI.
    /// </summary>
    [Serializable]
    public class HomeTemplate
    {
        public HomeStyle Style;
        public string DisplayName;
        public int MinPrice;
        public int MaxPrice;
        public float MonthlyUpkeep;
        public int ComfortScore;
        public bool CanStartFamily;
        public string Description;

        public HomeTemplate() { }

        public HomeTemplate(HomeStyle style, string displayName, int minPrice, int maxPrice, float monthlyUpkeep, int comfort, bool canStartFamily, string description = "")
        {
            Style = style;
            DisplayName = displayName;
            MinPrice = minPrice;
            MaxPrice = maxPrice;
            MonthlyUpkeep = monthlyUpkeep;
            ComfortScore = comfort;
            CanStartFamily = canStartFamily;
            Description = description;
        }

        public int GetSuggestedPrice()
        {
            return (MinPrice + MaxPrice) / 2;
        }
    }

    /// <summary>
    /// HomeInstance: representation of an owned home (purchase price, mortgage state).
    /// Not a ScriptableObject — runtime instance attached to PlayerFinance.
    /// </summary>
    [Serializable]
    public class HomeInstance
    {
        public HomeTemplate Template;
        public int PurchasePrice;
        public DateTime PurchaseDate;
        public Loan Mortgage; // null if no mortgage

        public HomeInstance(HomeTemplate template, int purchasePrice, DateTime purchaseDate)
        {
            Template = template;
            PurchasePrice = purchasePrice;
            PurchaseDate = purchaseDate;
        }

        public float MonthlyUpkeep => Template != null ? Template.MonthlyUpkeep : 0f;

        public float CurrentMarketValue(float marketMultiplier = 1f)
        {
            // Simple market model: purchase price * multiplier - simple depreciation over time (e.g., 0.2% per month)
            var monthsOwned = (float)(DateTime.UtcNow - PurchaseDate).TotalDays / 30f;
            float depreciation = 1f - (0.002f * monthsOwned); // 0.2% per month
            depreciation = Mathf.Clamp(depreciation, 0.5f, 1f); // don't drop below 50% baseline
            return PurchasePrice * marketMultiplier * depreciation;
        }
    }
}
