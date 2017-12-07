using System;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalaryDetailer;

namespace SalaryDetailerTests
{
    [TestClass]
    public class TestSalaryCalculations
    {
        private readonly string _medicareLevyRulesPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\RuleFiles\MedicareLevyRules.csv";
        private readonly string _budgetRepairLevyRulesPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\RuleFiles\BudgetRepairLevyRules.csv";
        private readonly string _incomeTaxPath = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + @"\RuleFiles\IncomeTaxRules.csv";

        private decimal CalculateTaxableIncome(decimal grossSalary, decimal superPercentage)
        {
            var taxableIncome = (grossSalary / ((100 + superPercentage) / 100));
            return taxableIncome;
        }

        /*
         * Taxable Income Test
         */
        [TestMethod]
        public void TestTaxableIncomeIsGrossSalaryMinusSuperPercentage()
        {
            decimal grossSalary = 65000;
            var superPercentage = (decimal)9.5;

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(CalculateTaxableIncome(grossSalary, superPercentage), s.TaxableIncome);
        }

        /*
         * Superannuation Test
         */
        [TestMethod]
        public void TestSuperannuationIsPercentageOfGross()
        {
            decimal grossSalary = 65000;
            var superPercentage = (decimal)9.5;
            var superannuation = grossSalary - (grossSalary / ((100 + superPercentage) / 100));

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(superannuation, s.Superannuation);
        }

        /*
         * Medicare Levy Tests
         */        
        [TestMethod]
        public void TestMedicareLevelIsZeroWhenTaxableIncomeIsLessThan21335()
        {
            decimal grossSalary = 22000;
            var superPercentage = (decimal)9.5;

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(0, s.MedicareLevy);
        }

        [TestMethod]
        public void TestMedicareLevelIsTenPercentOverWhenTaxableIncomeIsLessThan26668()
        {
            decimal grossSalary = 28000;
            var superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var medicareLevy = decimal.Ceiling((taxableIncome - 21335) * (decimal)0.1);

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(medicareLevy, s.MedicareLevy);
        }

        [TestMethod]
        public void TestMedicareLevelIsTwoPercentWhenTaxableIncomeIsOver26669()
        {
            decimal grossSalary = 50000;
            var superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(medicareLevy, s.MedicareLevy);
        }

        /*
         * Budget Repair Levy Tests
         */
        [TestMethod]
        public void TestBudgetRepairLevyIsZeroWhenTaxableIncomeIsLessThan180000()
        {
            decimal grossSalary = 50000;
            var superPercentage = (decimal)9.5;

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(0, s.BudgetRepairLevy);
        }

        [TestMethod]
        public void TestBudgetRepairLevyIsTwoPercentOverWhenTaxableIncomeIsOver180001()
        {
            decimal grossSalary = 250000;
            var superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var budgetRepairLevy = decimal.Ceiling((taxableIncome - 180000) * (decimal)0.02);

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(budgetRepairLevy, s.BudgetRepairLevy);
        }

        /*
         * Income Tax Tests
         */
        [TestMethod]
        public void TestIncomeTaxIsZeroWhenTaxableIncomeIsLessThan18200()
        {
            decimal grossSalary = 18000;
            var superPercentage = (decimal)9.5;

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(0, s.IncomeTax);
        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsLessThan37000()
        {
            decimal grossSalary = 27375;
            var superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var incomeTax = decimal.Ceiling((taxableIncome - 18200) * (decimal)0.19);

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(incomeTax, s.IncomeTax);
        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsLessThan87000()
        {
            decimal grossSalary = 50000;
            var superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var incomeTax = 3572 + decimal.Ceiling((taxableIncome - 37000) * (decimal)0.325);

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(incomeTax, s.IncomeTax);
        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsLessThan180000()
        {
            decimal grossSalary = 104025;
            var superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var incomeTax = 19822 + decimal.Ceiling((taxableIncome - 87000) * (decimal)0.37);

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(incomeTax, s.IncomeTax);
        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsGreaterThan180001()
        {
            decimal grossSalary = 219000;
            var superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var incomeTax = 54232 + decimal.Ceiling((taxableIncome - 180000) * (decimal)0.47);

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
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
            var superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var payPacket = ((taxableIncome / 365) * 7).ToString("C", CultureInfo.CurrentCulture) + " per week";

            var s = new Salary(grossSalary, superPercentage, 'W', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(payPacket, s.PayPacket);
        }

        [TestMethod]
        public void TestFortnightlyPayPacket()
        {
            decimal grossSalary = 15000;
            const decimal superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var payPacket = ((taxableIncome / 365) * 14).ToString("C", CultureInfo.CurrentCulture) + " per fortnight";

            var s = new Salary(grossSalary, superPercentage, 'F', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(payPacket, s.PayPacket);
        }

        [TestMethod]
        public void TestMonthlyPayPacket()
        {
            decimal grossSalary = 15000;
            const decimal superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var payPacket = (taxableIncome / 12).ToString("C", CultureInfo.CurrentCulture) + " per month";

            var s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(payPacket, s.PayPacket);
        }

        /*
         * Net Income Tests
         */
        [TestMethod]
        public void TestNetIncomeLessThan18200()
        {
            decimal grossSalary = 15000;
            const decimal superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);

            var s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(taxableIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeLessThan26668()
        {
            decimal grossSalary = 25000;
            const decimal superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);

            var superannuation = grossSalary - taxableIncome;
            var medicareLevy = decimal.Ceiling((taxableIncome - 21335) * (decimal)0.1);
            var incomeTax = decimal.Ceiling((taxableIncome - 18200) * (decimal)0.19);
            var netIncome = grossSalary - superannuation - medicareLevy - incomeTax;

            var s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeLessThan37000()
        {
            decimal grossSalary = 35000;
            const decimal superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var superannuation = grossSalary - taxableIncome;

            var medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);
            var incomeTax = decimal.Ceiling((taxableIncome - 18200) * (decimal)0.19);
            var netIncome = grossSalary - superannuation - medicareLevy - incomeTax;

            var s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeLessThan87000()
        {
            decimal grossSalary = 80000;
            const decimal superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var superannuation = grossSalary - taxableIncome;

            var medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);
            var incomeTax = 3572 + decimal.Ceiling((taxableIncome - 37000) * (decimal)0.325);
            var netIncome = grossSalary - superannuation - medicareLevy - incomeTax;

            var s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeLessThan180000()
        {
            decimal grossSalary = 150000;
            const decimal superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var superannuation = grossSalary - taxableIncome;

            var medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);
            var incomeTax = 19822 + decimal.Ceiling((taxableIncome - 87000) * (decimal)0.37);
            var netIncome = grossSalary - superannuation - medicareLevy - incomeTax;

            var s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }

        [TestMethod]
        public void TestNetIncomeGreaterThan180001()
        {
            decimal grossSalary = 250000;
            decimal superPercentage;
            superPercentage = (decimal)9.5;
            var taxableIncome = CalculateTaxableIncome(grossSalary, superPercentage);
            var superannuation = grossSalary - taxableIncome;

            var medicareLevy = decimal.Ceiling(taxableIncome * (decimal)0.02);
            var budgetRepairLevy = decimal.Ceiling((taxableIncome - 180000) * (decimal)0.02);
            var incomeTax = 54232 + decimal.Ceiling((taxableIncome - 180000) * (decimal)0.47);
            var netIncome = grossSalary - superannuation - medicareLevy - budgetRepairLevy - incomeTax;

            var s = new Salary(grossSalary, superPercentage, 'M', _medicareLevyRulesPath, _budgetRepairLevyRulesPath, _incomeTaxPath);
            Assert.AreEqual(netIncome, s.NetIncome);
        }
    }
}
