using System;
using UnityEngine;

namespace Jacameno
{
    /// <summary>
    /// BusinessInstance: runtime wrapper created when player purchases a BusinessType.
    /// Tracks dynamic current profit, manager state, and provides business operations.
    /// </summary>
    [Serializable]
    public class BusinessInstance
    {
        public BusinessType Template;
        public string InstanceId;
        public int PurchasePrice;
        public DateTime PurchaseDate;

        public float CurrentMonthlyRevenue;
        public float MonthlyOperatingCost;
        public float ManagerSalary;
        public bool ManagerHired;
        public float PlayerInvolvementMultiplier;

        public BusinessInstance(BusinessType template, int purchasePrice)
        {
            Template = template;
            InstanceId = $"{template.BusinessId}_{DateTime.UtcNow.Ticks}";
            PurchasePrice = purchasePrice;
            PurchaseDate = DateTime.UtcNow;
            CurrentMonthlyRevenue = template.NominalMonthlyRevenue;
            MonthlyOperatingCost = template.MonthlyOperatingCost;
            ManagerSalary = template.DefaultManagerSalary;
            ManagerHired = false;
            PlayerInvolvementMultiplier = template.PlayerInvolvementMultiplier;
        }

        public float CalculateMonthlyNetProfit()
        {
            float revenue = CurrentMonthlyRevenue;
            float costs = MonthlyOperatingCost + (ManagerHired ? ManagerSalary : 0f);
            return revenue - costs;
        }

        public void HireManager()
        {
            ManagerHired = true;
            Debug.Log($"Manager hired for {Template.DisplayName} ({InstanceId})");
        }

        public void FireManager()
        {
            ManagerHired = false;
            Debug.Log($"Manager fired for {Template.DisplayName} ({InstanceId})");
        }

        /// <summary>
        /// Simulate player involvement adding revenue this month.
        /// </summary>
        public void WorkOnBusiness(float effort)
        {
            effort = Mathf.Clamp01(effort);
            float bonus = Template.NominalMonthlyRevenue * (PlayerInvolvementMultiplier - 1f) * effort;
            CurrentMonthlyRevenue += bonus;
            Debug.Log($"Worked on {Template.DisplayName}: effort {effort}, revenue bonus {bonus}");
        }

        public void ApplyRevenueModifier(float percent)
        {
            CurrentMonthlyRevenue *= 1f + percent;
            Debug.Log($"Applied revenue modifier {percent * 100f}% to {Template.DisplayName}, now {CurrentMonthlyRevenue}");
        }
    }
}