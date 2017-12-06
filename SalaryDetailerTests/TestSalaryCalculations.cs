using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SalaryDetailer;

namespace SalaryDetailerTests
{
    [TestClass]
    public class TestSalaryCalculations
    {
        private decimal CalculateTaxableIncome(decimal grossSalary, decimal superPercentage)
        {
            decimal taxableIncome = (grossSalary / ((100 + superPercentage) / 100));
            return taxableIncome;
        }

        [TestMethod]
        public void TestTaxableIncomeIsGrossSalaryMinusSuperPercentage()
        {
            decimal grossSalary = 65000;
            decimal superPercentage = (decimal)9.5;

            Salary s = new Salary(grossSalary, superPercentage, 'F');
            Assert.AreEqual((grossSalary / ((100 + superPercentage) / 100)), s.TaxableIncome);
        }

        [TestMethod]
        public void TestSuperannuationIsPercentageOfGross()
        {
            decimal grossSalary = 65000;
            decimal superPercentage = (decimal)9.5;

            Salary s = new Salary(grossSalary, superPercentage, 'F');
            Assert.AreEqual(grossSalary - (grossSalary / ((100 + superPercentage) / 100)), s.Superannuation);
        }

        /*
         * Medicare Levy Tests
         */        
        [TestMethod]
        public void TestMedicareLevelIsZeroWhenTaxableIncomeIsLessThan21335()
        {
            //decimal 
            Salary s = new Salary((decimal)15000, 10, 'F');
            Assert.AreEqual(0, s.MedicareLevy);
        }

        [TestMethod]
        public void TestMedicareLevelIsTenPercentOverWhenTaxableIncomeIsLessThan26668()
        {
            //decimal 

            Salary s = new Salary((decimal)26000, 10, 'F');
            //Assert.AreEqual(, s.MedicareLevy);
        }

        [TestMethod]
        public void TestMedicareLevelIsTenPercentOverWhenTaxableIncomeIsOver26669()
        {

        }

        /*
         * Budget Repair Levy Tests
         */
        [TestMethod]
        public void TestBudgetRepairLevyIsZeroWhenTaxableIncomeIsLessThan180000()
        {

        }

        [TestMethod]
        public void TestBudgetRepairLevyIsTwoPercentOverWhenTaxableIncomeIsOver180001()
        {

        }

        /*
         * Income Tax
         */
        [TestMethod]
        public void TestIncomeTaxIsZeroWhenTaxableIncomeIsLessThan18200()
        {

        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsLessThan37000()
        {

        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsLessThan87000()
        {

        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsLessThan18000()
        {

        }

        [TestMethod]
        public void TestIncomeTaxWhenTaxableIncomeIsGreaterThan180001()
        {

        }

        /*
         * Pay Packet Tests
         */
        [TestMethod]
        public void TestWeeklyPayPacket()
        {

        }

        [TestMethod]
        public void TestFortnightlyPayPacket()
        {

        }

        [TestMethod]
        public void TestMonthlyPayPacket()
        {

        }

        /*
         * Net Income Tests
         */
        [TestMethod]
        public void TestNetIncomeLessThan18200()
        {

        }

        public void TestNetIncomeLessThan26668()
        {

        }

        public void TestNetIncomeLessThan37000()
        {

        }

        public void TestNetIncomeLessThan87000()
        {

        }

        public void TestNetIncomeLessThan180000()
        {

        }

        public void TestNetIncomeGreaterThan180001()
        {

        }
    }
}
