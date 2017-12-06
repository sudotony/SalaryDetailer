using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryDetailer
{
    class Salary
    {
        private decimal _grossSalary;
        public decimal GrossSalary { get { return _grossSalary; } set { _grossSalary = value; } }

        private char _payFrequency;
        public char PayFrequency { get { return _payFrequency; } set { _payFrequency = value; } }

        private decimal _taxableIncome;
        public decimal TaxableIncome { get { return _taxableIncome; } set { _taxableIncome = value; } }

        private decimal _superannuation;
        public decimal Superannuation { get { return _superannuation; } set { _superannuation = value; } }

        private decimal _medicareLevy;
        public decimal MedicareLevy { get { return _medicareLevy; } set { _medicareLevy = value; } }

        private decimal _budgetRepairLevy;
        public decimal BudgetRepairLevy { get { return _budgetRepairLevy; } set { _budgetRepairLevy = value; } }

        private decimal _incomeTax;
        public decimal IncomeTax { get { return _incomeTax; } set { _incomeTax = value; } }

        private decimal _netIncome;
        public decimal NetIncome { get { return _netIncome; } set { _netIncome = value; } }

        private decimal _payPacket;
        public decimal PayPacket { get { return _payPacket; } set { _payPacket = value; } }

        public Salary(decimal grossSalary, decimal superannuationPercentage, char payFrequency)
        {
            GrossSalary = grossSalary;
            PayFrequency = payFrequency; 
            CalculateTaxableIncome(superannuationPercentage);
            CalculateSuperannuation();
            CalculateMedicareLevy();
            CalculateBudgetRepairLevy();
            CalculateIncomeTax();
            CalculateNetIncome();
            CalculatePayPacket();
        }

        private void CalculateTaxableIncome(decimal superannuationPercentage)
        {
            TaxableIncome = GrossSalary / ((100 + superannuationPercentage) / 100);
        }

        private void CalculateSuperannuation()
        {
            Superannuation = GrossSalary - TaxableIncome;
        }

        private void CalculateMedicareLevy()
        {
            decimal medicareLevy = 0;
            if (TaxableIncome > 21335 && TaxableIncome <= 26668)
                medicareLevy = (TaxableIncome - 21335) * (decimal)0.1;
            else if (TaxableIncome > 26669)
                medicareLevy = TaxableIncome * (decimal)0.02;

            MedicareLevy = medicareLevy;
        }

        private void CalculateBudgetRepairLevy()
        {
            decimal budgetRepairLevy = 0;
            if (TaxableIncome > 180000)
                budgetRepairLevy = (TaxableIncome - 180000) * (decimal)0.02;

            BudgetRepairLevy = budgetRepairLevy;
        }

        private void CalculateIncomeTax()
        {
            decimal incomeTax = 0;
            if (TaxableIncome > 18200 && TaxableIncome <= 37000)
                incomeTax = (TaxableIncome - 18200) * (decimal)0.19;
            else if (TaxableIncome > 37000 && TaxableIncome <= 87000)
                incomeTax = 3572 + ((TaxableIncome - 37000) * (decimal)0.325);
            else if (TaxableIncome > 87000 && TaxableIncome <= 180000)
                incomeTax = 19822 + ((TaxableIncome - 87000) * (decimal)0.37);
            else if (TaxableIncome > 180001)
                incomeTax = 54232 + ((TaxableIncome - 180000) * (decimal)0.47);

            IncomeTax = incomeTax;
        }

        private void CalculateNetIncome()
        {
            decimal netIncome = GrossSalary - Superannuation - MedicareLevy - BudgetRepairLevy - IncomeTax;
            NetIncome = netIncome;
        }

        private void CalculatePayPacket()
        {
            decimal payPacket = 0;
            switch (PayFrequency)
            {
                case 'W':
                    payPacket = (NetIncome / 365) * 7;
                    break;
                case 'F':
                    payPacket = (NetIncome / 365) * 14;
                    break;
                case 'M':
                    payPacket = NetIncome / 12;
                    break;
            }

            PayPacket = payPacket;
        }
    }
}