using System;
using System.Collections.Generic;
using UnityEngine;

namespace Jacameno
{
    /// <summary>
    /// PlayerFinanceV2 integrates HomeInstance, BusinessInstance, and Loan records.
    /// - Starts with configurable capital (default 100k).
    /// - Methods: PurchaseHome, PurchaseBusiness, TakeLoan, CalculateMonthlyExpenses, ApplyMonthlyTick.
    /// </summary>
    public class PlayerFinanceV2 : MonoBehaviour
    {
        [Header("Starting capital")]
        [SerializeField] private float startingCapital = 100000f;
        public float CurrentCapital { get; private set; }

        [Header("Owned assets")]
        public List<HomeInstance> OwnedHomes = new List<HomeInstance>();
        public List<BusinessInstance> OwnedBusinesses = new List<BusinessInstance>();
        public List<Loan> ActiveLoans = new List<Loan>();

        private void Awake()
        {
            CurrentCapital = startingCapital;
        }

        public bool PurchaseHome(HomeTemplate template, int price, bool useMortgage = false, int mortgageTermMonths = 360, float mortgageRate = 0.05f)
        {
            if (template == null) { Debug.LogWarning("PurchaseHome: template null"); return false; }

            if (!useMortgage && price > CurrentCapital)
            {
                Debug.Log($"Insufficient capital to buy home for {price}");
                return false;
            }

            var instance = new HomeInstance(template, price, DateTime.UtcNow);

            if (useMortgage)
            {
                // Create mortgage for 80% of purchase price by default
                float downPayment = Mathf.Max(0f, price * 0.2f);
                float loanAmount = price - downPayment;
                if (downPayment > CurrentCapital)
                {
                    Debug.Log($"Insufficient capital for down payment ({downPayment})");
                    return false;
                }
                CurrentCapital -= downPayment;
                Loan mortgage = Banking.CreateLoan(loanAmount, mortgageTermMonths, mortgageRate);
                instance.Mortgage = mortgage;
                ActiveLoans.Add(mortgage);
                Debug.Log($"Purchased home {template.DisplayName} with mortgage. Down {downPayment}, loan {loanAmount}");
            }
            else
            {
                CurrentCapital -= price;
                Debug.Log($"Purchased home {template.DisplayName} outright for {price}");
            }

            OwnedHomes.Add(instance);
            return true;
        }

        public bool PurchaseBusiness(BusinessType businessType)
        {
            if (businessType == null) { Debug.LogWarning("PurchaseBusiness: null type"); return false; }
            if (businessType.PurchaseCost > CurrentCapital)
            {
                Debug.Log($"Insufficient capital to buy business {businessType.DisplayName} ({businessType.PurchaseCost})");
                return false;
            }

            CurrentCapital -= businessType.PurchaseCost;
            var inst = new BusinessInstance(businessType, businessType.PurchaseCost);
            OwnedBusinesses.Add(inst);
            Debug.Log($"Purchased business {businessType.DisplayName} for {businessType.PurchaseCost}. Remaining capital {CurrentCapital}");
            return true;
        }

        public Loan TakeLoan(float amount, int termMonths, float annualInterestRate)
        {
            var loan = Banking.CreateLoan(amount, termMonths, annualInterestRate);
            ActiveLoans.Add(loan);
            CurrentCapital += amount;
            Debug.Log($"Took loan {loan.LoanId} for {amount}. New capital {CurrentCapital}");
            return loan;
        }

        public float CalculateMonthlyExpenses()
        {
            float total = 0f;

            // Homes upkeep
            foreach (var h in OwnedHomes) total += h.MonthlyUpkeep;

            // Business operating + manager salary
            foreach (var b in OwnedBusinesses)
            {
                total += b.MonthlyOperatingCost;
                if (b.ManagerHired) total += b.ManagerSalary;
            }

            // Loan payments (estimate monthly payment)
            foreach (var loan in ActiveLoans)
            {
                if (loan.IsActive) total += loan.MonthlyPayment();
            }

            Debug.Log($"Calculated monthly expenses: {total}");
            return total;
        }

        /// <summary>
        /// Called by MonthlyTick to advance finances by one month.
        /// </summary>
        public void ApplyMonthlyTick()
        {
            // Advance loans
            float loanOutflow = 0f;
            for (int i = ActiveLoans.Count - 1; i >= 0; i--)
            {
                var loan = ActiveLoans[i];
                if (!loan.IsActive)
                {
                    ActiveLoans.RemoveAt(i);
                    continue;
                }

                float payment = loan.AdvanceMonth();
                loanOutflow += payment;
            }

            // Compute business net income
            float businessNet = 0f;
            foreach (var b in OwnedBusinesses)
            {
                businessNet += b.CalculateMonthlyNetProfit();
            }

            // Sum home upkeep
            float homeUpkeep = 0f;
            foreach (var h in OwnedHomes) homeUpkeep += h.MonthlyUpkeep;

            // Subtract expenses and add business net
            CurrentCapital += businessNet - homeUpkeep - loanOutflow;

            Debug.Log($"Monthly tick: businessNet {businessNet}, homeUpkeep {homeUpkeep}, loanPayments {loanOutflow}, newCapital {CurrentCapital}");
        }

        // Utility: calculate total net worth (cash + homes + businesses + outstanding loan principal (subtracted))
        public float CalculateNetWorth()
        {
            float net = CurrentCapital;
            foreach (var h in OwnedHomes) net += h.CurrentMarketValue();
            foreach (var b in OwnedBusinesses) net += b.PurchasePrice; // simple business valuation (could be refined)
            foreach (var loan in ActiveLoans) net -= loan.Principal;
            return net;
        }
    }
}