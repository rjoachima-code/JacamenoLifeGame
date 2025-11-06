using System;
using UnityEngine;

namespace Jacameno
{
    /// <summary>
    /// MonthlyTick: drives the monthly progression for the prototype.
    /// Attach to a GameManager object and wire PlayerFinanceV2 and RivalManager.
    /// </summary>
    public class MonthlyTick : MonoBehaviour
    {
        [Tooltip("Days per month used by the tick (default 30).")]
        public int DaysPerMonth = 30;

        [Tooltip("Reference to the finance manager")]
        public PlayerFinanceV2 Finance;

        [Tooltip("Reference to rival manager (optional)")]
        public RivalManager RivalManager;

        // Simulated day counter
        private int currentDay = 1;

        // Example: press key or call AdvanceOneMonth()
        public void AdvanceOneMonth()
        {
            // Advance loans, business results, etc.
            if (Finance != null)
            {
                Finance.ApplyMonthlyTick();
            }

            // Let rivals act on a crude schedule: approximate days passed
            if (RivalManager != null)
            {
                RivalManager.AdvanceDays(DaysPerMonth);
            }

            currentDay = 1;
            Debug.Log($"Advanced one month. New capital: {(Finance != null ? Finance.CurrentCapital : 0f)}");
        }

        // Example development testing: advance with key press
        private void Update()
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.M))
            {
                AdvanceOneMonth();
            }
#endif
        }
    }
}