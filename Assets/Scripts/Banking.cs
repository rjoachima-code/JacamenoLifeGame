using System;
using UnityEngine;

namespace Jacameno
{
    [Serializable]
    public class Loan
    {
        public string LoanId;
        public float Principal;
        public int TermMonths;
        public float AnnualInterestRate; // e.g., 0.08 = 8%
        public int MonthsElapsed;

        public Loan(string loanId, float principal, int termMonths, float annualInterestRate)
        {
            LoanId = loanId;
            Principal = principal;
            TermMonths = termMonths;
            AnnualInterestRate = annualInterestRate;
            MonthsElapsed = 0;
        }

        public float MonthlyInterestRate => AnnualInterestRate / 12f;

        /// <summary>
        /// Monthly payment using standard amortization formula.
        /// </summary>
        public float MonthlyPayment()
        {
            if (Principal <= 0 || TermMonths <= 0) return 0f;
            float r = MonthlyInterestRate;
            float n = TermMonths;
            float numerator = Principal * r * Mathf.Pow(1f + r, n);
            float denominator = Mathf.Max(1e-6f, Mathf.Pow(1f + r, n) - 1f);
            return numerator / denominator;
        }

        /// <summary>
        /// Advance one month: returns payment amount (interest + principal portion).
        /// Updates internal MonthsElapsed and reduces principal accordingly.
        /// </summary>
        public float AdvanceMonth()
        {
            if (Principal <= 0f || MonthsElapsed >= TermMonths) return 0f;

            float payment = MonthlyPayment();
            float interest = Principal * MonthlyInterestRate;
            float principalPaid = Mathf.Clamp(payment - interest, 0f, Principal);
            Principal -= principalPaid;
            MonthsElapsed++;
            // If final payment rounding leaves negligible remainder, clear it
            if (MonthsElapsed >= TermMonths || Principal < 0.01f) Principal = 0f;
            return interest + principalPaid;
        }

        public bool IsActive => Principal > 0f && MonthsElapsed < TermMonths;
    }

    /// <summary>
    /// Banking: lightweight bank manager for issuing loans/mortgages.
    /// </summary>
    public static class Banking
    {
        private static int loanCounter = 0;

        public static Loan CreateLoan(float amount, int termMonths, float annualInterestRate)
        {
            loanCounter++;
            string id = $"loan_{loanCounter}_{DateTime.UtcNow.Ticks}";
            return new Loan(id, amount, termMonths, annualInterestRate);
        }

        /// <summary>
        /// Helper: compute estimated mortgage monthly payment for UI preview
        /// </summary>
        public static float EstimateMonthlyPayment(float principal, int termMonths, float annualInterestRate)
        {
            if (termMonths <= 0) return 0f;
            float r = annualInterestRate / 12f;
            float n = termMonths;
            if (Mathf.Approximately(r, 0f)) return principal / n;
            float numerator = principal * r * Mathf.Pow(1f + r, n);
            float denominator = Mathf.Pow(1f + r, n) - 1f;
            return numerator / denominator;
        }
    }
}