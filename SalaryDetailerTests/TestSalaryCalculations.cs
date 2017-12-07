using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalaryDetailer;

namespace SalaryDetailerTests
{
    [TestClass]
    public class TestSalaryCalculations
    {
        private string _medicareLevyRulesPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\RuleFiles\MedicareLevyRules.csv";
        private string _budgetRepairLevyRulesPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\RuleFiles\BudgetRepairLevyRules.csv";
        private string _incomeTaxPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\RuleFiles\IncomeTaxRules.csv";

        private decimal CalculateTaxableIncome(decimal grossSalary, decimal superPercentage)
        {
            decimal taxableIncome = (grossSalary / ((100 + superPercentage) / 100));
            return taxableIncome;
        }

        /*
         * Taxable Income Test
         */
        [TestMethod]
        public void TestTaxableIncomeIsGrossSalaryMinusSuperPercentage()
        {
            decimal grossSalary = 65000;
            decimal superPercentage = (decimal)9.5;

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(CalculateTaxableIncome(grossSalary, superPercentage), s.TaxableIncome);
        }

        /*
         * Superannuation Test
         */
        [TestMethod]
        public void TestSuperannuationIsPercentageOfGross()
        {
            decimal grossSalary = 65000;
            decimal superPercentage = (decimal)9.5;
            decimal superannuation = grossSalary - (grossSalary / ((100 + superPercentage) / 100));

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(superannuation, s.Superannuation);
        }

        /*
         * Medicare Levy Tests
         */        
        [TestMethod]
        public void TestMedicareLevelIsZeroWhenTaxableIncomeIsLessThan21335()
        {
            decimal grossSalary = 22000;
            decimal superPercentage = (decimal)9.5;

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(0, s.MedicareLevy);
        }

        [TestMethod]
        public void TestMedicareLevelIsTenPercentOverWhenTaxableIncomeIsLessThan26668()
        {
            decimal grossSalary = 28000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal medicareLevy = decimal.Ceiling((taxableIncome - 21335) * (decimal)0.1);

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(medicareLevy, s.MedicareLevy);
        }

        [TestMethod]
        public void TestMedicareLevelIsTwoPercentWhenTaxableIncomeIsOver26669()
        {
            decimal grossSalary = 50000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(medicareLevy, s.MedicareLevy);
        }

        /*
         * Budget Repair Levy Tests
         */
        [TestMethod]
        public void TestBudgetRepairLevyIsZeroWhenTaxableIncomeIsLessThan180000()
        {
            decimal grossSalary = 50000;
            decimal superPercentage = (decimal)9.5;

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(0, s.BudgetRepairLevy);
        }

        [TestMethod]
        public void TestBudgetRepairLevyIsTwoPercentOverWhenTaxableIncomeIsOver180001()
        {
            decimal grossSalary = 250000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal budgetRepairLevy = decimal.Ceiling((taxableIncome - 180000) * (decimal)0.02);

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(budgetRepairLevy, s.BudgetRepairLevy);
        }

        /*
         * Income Tax Tests
         */
        [TestMethod]
        public void TestIncomeTaxIsZeroWhenTaxableIncomeIsLessThan18200()
        {
            decimal grossSalary = 18000;
            decimal superPercentage = (decimal)9.5;

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(0, s.IncomeTax);
        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsLessThan37000()
        {
            decimal grossSalary = 27375;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal incomeTax = decimal.Ceiling((taxableIncome - 18200) * (decimal)0.19);

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(incomeTax, s.IncomeTax);
        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsLessThan87000()
        {
            decimal grossSalary = 50000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal incomeTax = 3572 + decimal.Ceiling((taxableIncome - 37000) * (decimal)0.325);

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(incomeTax, s.IncomeTax);
        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsLessThan180000()
        {
            decimal grossSalary = 104025;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal incomeTax = 19822 + decimal.Ceiling((taxableIncome - 87000) * (decimal)0.37);

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(incomeTax, s.IncomeTax);
        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsGreaterThan180001()
        {
            decimal grossSalary = 219000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal incomeTax = 54232 + decimal.Ceiling((taxableIncome - 180000) * (decimal)0.47);

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(incomeTax, s.IncomeTax);
        }

        /*
         * Pay Packet Tests
         * 
         * These tests assume that the gross salary of $15,000 will not have deductions. This is to simplify the test.
         */
        [TestMethod]
        public void TestWeeklyPayPacket()
        {
            decimal grossSalary = 15000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            string payPacket = ((decimal)((taxableIncome / 365) * 7)).ToString("C", CultureInfo.CurrentCulture) + " per week";

            Salary s = new Salary(grossSalary, superPercentage, 'W', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(payPacket, s.PayPacket);
        }

        [TestMethod]
        public void TestFortnightlyPayPacket()
        {
            decimal grossSalary = 15000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            string payPacket = ((decimal)((taxableIncome / 365) * 14)).ToString("C", CultureInfo.CurrentCulture) + " per fortnight";

            Salary s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(payPacket, s.PayPacket);
        }

        [TestMethod]
        public void TestMonthlyPayPacket()
        {
            decimal grossSalary = 15000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            string payPacket = ((decimal)(taxableIncome / 12)).ToString("C", CultureInfo.CurrentCulture) + " per month";

            Salary s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(payPacket, s.PayPacket);
        }

        /*
         * Net Income Tests
         */
        [TestMethod]
        public void TestNetIncomeLessThan18200()
        {
            decimal grossSalary = 15000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);

            Salary s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(taxableIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeLessThan26668()
        {
            decimal grossSalary = 25000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);

            decimal superannuation = grossSalary - taxableIncome;
            decimal medicareLevy = decimal.Ceiling((taxableIncome - 21335) * (decimal)0.1);
            decimal incomeTax = decimal.Ceiling((taxableIncome - 18200) * (decimal)0.19);
            decimal netIncome = grossSalary - superannuation - medicareLevy - incomeTax;

            Salary s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeLessThan37000()
        {
            decimal grossSalary = 35000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal superannuation = grossSalary - taxableIncome;

            decimal medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);
            decimal incomeTax = decimal.Ceiling((taxableIncome - 18200) * (decimal)0.19);
            decimal netIncome = grossSalary - superannuation - medicareLevy - incomeTax;

            Salary s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeLessThan87000()
        {
            decimal grossSalary = 80000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal superannuation = grossSalary - taxableIncome;

            decimal medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);
            decimal incomeTax = 3572 + decimal.Ceiling((taxableIncome - 37000) * (decimal)0.325);
            decimal netIncome = grossSalary - superannuation - medicareLevy - incomeTax;

            Salary s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeLessThan180000()
        {
            decimal grossSalary = 150000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal superannuation = grossSalary - taxableIncome;

            decimal medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);
            decimal incomeTax = 19822 + decimal.Ceiling((taxableIncome - 87000) * (decimal)0.37);
            decimal netIncome = grossSalary - superannuation - medicareLevy - incomeTax;

            Salary s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeGreaterThan180001()
        {
            decimal grossSalary = 250000;
            decimal superPercentage = (decimal)9.5;
            decimal taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            decimal superannuation = grossSalary - taxableIncome;

            decimal medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);
            decimal budgetRepairLevy = decimal.Ceiling((taxableIncome - 180000) * (decimal)0.02);
            decimal incomeTax = 54232 + decimal.Ceiling((taxableIncome - 180000) * (decimal)0.47);
            decimal netIncome = grossSalary - superannuation - medicareLevy - budgetRepairLevy - incomeTax;

            Salary s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }
    }
}
