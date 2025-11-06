using UnityEngine;

namespace Jacameno
{
    /// <summary>
    /// BusinessType: ScriptableObject template describing a business type (Gas Station, Restaurant, Supermarket).
    /// Create assets for each business type and reference them in UI to purchase instances.
    /// </summary>
    [CreateAssetMenu(fileName = "BusinessType_", menuName = "Jacameno/BusinessType", order = 100)]
    public class BusinessType : ScriptableObject
    {
        public string BusinessId;
        public string DisplayName;
        [Tooltip("Upfront purchase cost to acquire/open this business.")]
        public int PurchaseCost;
        [Tooltip("Nominal monthly revenue before operating costs.")]
        public int NominalMonthlyRevenue;
        [Tooltip("Monthly operating cost (staff, supplies, utilities).")]
        public int MonthlyOperatingCost;
        [Tooltip("Default manager salary if hired.")]
        public int DefaultManagerSalary;
        [Tooltip("Player involvement multiplier (e.g., 1.15 means +15% when player is involved).")]
        public float PlayerInvolvementMultiplier = 1.15f;
        [TextArea] public string Description;
    }
}