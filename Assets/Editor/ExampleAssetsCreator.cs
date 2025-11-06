#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Jacameno.EditorTools
{
    /// <summary>
    /// Editor helper to programmatically create example BusinessType assets (Gas Station, Small Restaurant).
    /// Put this file in Assets/Editor and use the menu item to create assets for the prototype.
    /// </summary>
    public static class ExampleAssetsCreator
    {
        [MenuItem("Jacameno/Create Example Business Types")]
        public static void CreateExampleBusinessTypes()
        {
            var gas = ScriptableObject.CreateInstance<BusinessType>();
            gas.BusinessId = "gas_station";
            gas.DisplayName = "Gas Station Franchise";
            gas.PurchaseCost = 60000;
            gas.NominalMonthlyRevenue = 8000;
            gas.MonthlyOperatingCost = 3000;
            gas.DefaultManagerSalary = 2500;
            gas.PlayerInvolvementMultiplier = 1.10f;
            gas.Description = "High upfront cost, steady fuel sales and convenience store revenue.";
            AssetDatabase.CreateAsset(gas, "Assets/Resources/BusinessTypes/GasStation.asset");

            var restaurant = ScriptableObject.CreateInstance<BusinessType>();
            restaurant.BusinessId = "small_restaurant";
            restaurant.DisplayName = "Small Restaurant";
            restaurant.PurchaseCost = 80000;
            restaurant.NominalMonthlyRevenue = 12000;
            restaurant.MonthlyOperatingCost = 6000;
            restaurant.DefaultManagerSalary = 3500;
            restaurant.PlayerInvolvementMultiplier = 1.25f;
            restaurant.Description = "Higher potential income, seasonal demand, requires good staff.";
            AssetDatabase.CreateAsset(restaurant, "Assets/Resources/BusinessTypes/SmallRestaurant.asset");

            AssetDatabase.SaveAssets();
            Debug.Log("Example business type assets created under Assets/Resources/BusinessTypes/");
        }
    }
}
#endif