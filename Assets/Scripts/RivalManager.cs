using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jacameno
{
    /// <summary>
    /// RivalManager: manages a set of AIRival instances and periodically makes them act.
    /// This is a prototype: extend to persist rival wealth and more complex behavior.
    /// </summary>
    public class RivalManager : MonoBehaviour
    {
        public List<AIRival> Rivals = new List<AIRival>();
        public float ActionIntervalDays = 10f; // how often rivals act (in game days) - controlled by MonthlyTick or a day system

        private float daysSinceLastAction = 0f;
        private System.Random rng = new System.Random();

        private void Start()
        {
            // Example: if none present, create a few prototype rivals
            if (Rivals == null || Rivals.Count == 0)
            {
                Rivals = new List<AIRival>
                {
                    new AIRival { RivalName = "Victoria Sterling", AggressionLevel = 70, PreferredDistrict = "North Joc", Wealth = 300000f },
                    new AIRival { RivalName = "Tony 'Grips' Marino", AggressionLevel = 85, PreferredDistrict = "South Joc", Wealth = 120000f },
                    new AIRival { RivalName = "Marisol Velasquez", AggressionLevel = 40, PreferredDistrict = "East Joc", Wealth = 90000f }
                };
            }
        }

        /// <summary>
        /// External driver calls this every in-game day; RivalManager accumulates days and runs actions.
        /// </summary>
        public void AdvanceDays(float days)
        {
            daysSinceLastAction += days;
            if (daysSinceLastAction >= ActionIntervalDays)
            {
                daysSinceLastAction = 0f;
                PerformRivalActions();
            }
        }

        private void PerformRivalActions()
        {
            foreach (var rival in Rivals)
            {
                int roll = rng.Next(0, 100);
                if (roll < rival.AggressionLevel / 2)
                {
                    // Sabotage chance (prototype: send an aggressive mail)
                    rival.SendEmail("Market Threat", $"I'm expanding in {rival.PreferredDistrict}. Watch yourself.");
                }
                else if (roll < rival.AggressionLevel)
                {
                    rival.SendEmail("Business Inquiry", "Interested in a partnership?");
                }
                else
                {
                    // benign action
                    rival.SendEmail("Social", "Lovely weather in the district. Let's meet.");
                }
            }
        }

        // Utility: get rival by name
        public AIRival GetRival(string name)
        {
            return Rivals.Find(r => r.RivalName == name);
        }
    }
}