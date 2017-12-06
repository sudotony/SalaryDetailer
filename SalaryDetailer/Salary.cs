using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SalaryDetailer
{
    class Salary
    {
        private decimal _grossSalary;
        public decimal GrossSalary { get { return _grossSalary; } set { _grossSalary = value; } }

        private decimal _superannuationPercentage;
        public decimal SuperannuationPercentage { get { return _superannuationPercentage; } set { _superannuationPercentage = value; } }

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

        private string _payPacket;
        public string PayPacket { get { return _payPacket; } set { _payPacket = value; } }

        /*
         * TODO Create a data type to store the taxable income threshold & a string mathematical expression
         * e.g.
         * 21335,0
         * 26668,{TI}-(21335*0.1)
         */

        public Salary(decimal grossSalary, decimal superannuationPercentage, char payFrequency)
        {
            _grossSalary = grossSalary;
            _payFrequency = payFrequency;
            _superannuationPercentage = superannuationPercentage;

            CalculateTaxableIncome();
            CalculateSuperannuation();
            CalculateMedicareLevy();
            CalculateBudgetRepairLevy();
            CalculateIncomeTax();
            CalculateNetIncome();
            CalculatePayPacket();
        }

        private void CalculateTaxableIncome()
        {
            _taxableIncome = _grossSalary / ((100 + _superannuationPercentage) / 100);
        }

        private void CalculateSuperannuation()
        {
            _superannuation = _grossSalary - _taxableIncome;
        }

        private void CalculateMedicareLevy()
        {
            decimal medicareLevy = 0;
            if (_taxableIncome > 21335 && _taxableIncome <= 26668)
                medicareLevy = (_taxableIncome - 21335) * (decimal)0.1;
            else if (_taxableIncome > 26669)
                medicareLevy = _taxableIncome * (decimal)0.02;

            _medicareLevy = decimal.Ceiling(medicareLevy);
        }

        private void CalculateBudgetRepairLevy()
        {
            decimal budgetRepairLevy = 0;
            if (_taxableIncome > 180000)
                budgetRepairLevy = (_taxableIncome - 180000) * (decimal)0.02;

            _budgetRepairLevy = decimal.Ceiling(budgetRepairLevy);
        }

        private void CalculateIncomeTax()
        {
            decimal incomeTax = 0;
            if (_taxableIncome > 18200 && _taxableIncome <= 37000)
                incomeTax = (_taxableIncome - 18200) * (decimal)0.19;
            else if (_taxableIncome > 37000 && _taxableIncome <= 87000)
                incomeTax = 3572 + ((_taxableIncome - 37000) * (decimal)0.325);
            else if (_taxableIncome > 87000 && _taxableIncome <= 180000)
                incomeTax = 19822 + ((_taxableIncome - 87000) * (decimal)0.37);
            else if (_taxableIncome > 180001)
                incomeTax = 54232 + ((_taxableIncome - 180000) * (decimal)0.47);

            _incomeTax = decimal.Ceiling(incomeTax);
        }

        private void CalculateNetIncome()
        {
            decimal netIncome = _grossSalary - _superannuation - _medicareLevy - _budgetRepairLevy - _incomeTax;
            _netIncome = netIncome;
        }

        private void CalculatePayPacket()
        {
            string payPacket;
            switch (PayFrequency)
            {
                case 'W':
                    payPacket = ((decimal)((NetIncome / 365) * 7)).ToString("C", CultureInfo.CurrentCulture) + " per week";
                    break;
                case 'F':
                    payPacket = ((decimal)((NetIncome / 365) * 14)).ToString("C", CultureInfo.CurrentCulture) + " per fortnight";
                    break;
                case 'M':
                default:
                    payPacket = ((decimal)(NetIncome / 12)).ToString("C", CultureInfo.CurrentCulture) + " per month";
                    break;
            }

            _payPacket = payPacket;
        }
    }
}