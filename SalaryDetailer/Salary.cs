using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

namespace SalaryDetailer
{
    public class Salary
    {
        public decimal GrossSalary { get; set; }
        public decimal SuperannuationPercentage { get; set; }
        public char PayFrequency { get; set; }
        public decimal TaxableIncome { get; set; }
        public decimal Superannuation { get; set; }
        public decimal MedicareLevy { get; set; }
        public decimal BudgetRepairLevy { get; set; }
        public decimal IncomeTax { get; set; }
        public decimal NetIncome { get; set; }
        public string PayPacket { get; set; }

        private readonly List<CalculationRule> _medicareLevyRules;
        private readonly List<CalculationRule> _budgetRepairLevyRules;
        private readonly List<CalculationRule> _incomeTaxRules;

        /*
         * A basic struct to store the calculation rules.
         * lower and upper thresholds specify the taxable income range
         * The expression is a mathematical string with a substring TI that is to be replaced with the taxable income. 
         */
        public struct CalculationRule
        {
            public int LowerThreshold;
            public int UpperThreshold;
            public string Expression;

            public CalculationRule(int p1, int p2, string p3)
            {
                LowerThreshold = p1;
                UpperThreshold = p2;
                Expression = p3;
            }
        }

        /*
         * Constructor.
         * 
         * Everything is calculated on construction. This has been choosen because of the simplicity of the program. After all input has been entered, all details are then displayed to the user.
         * 
         * If the information were to be calculated at different times, perhaps these values would be generated at different times.
         */
        public Salary(decimal grossSalary, decimal superannuationPercentage, char payFrequency, string medicareLevyRulesPath, string budgetRepairLevyRulesPath, string incomeTaxPath)
        {
            try
            {
                GrossSalary = grossSalary;
                SuperannuationPercentage = superannuationPercentage;
                PayFrequency = payFrequency;

                _medicareLevyRules = new List<CalculationRule>();
                _budgetRepairLevyRules = new List<CalculationRule>();
                _incomeTaxRules = new List<CalculationRule>();

                // I've chosen to pass the List of CalculationRules by ref to easily add new rules.
                LoadCalculationRules(medicareLevyRulesPath, ref _medicareLevyRules);
                LoadCalculationRules(budgetRepairLevyRulesPath, ref _budgetRepairLevyRules);
                LoadCalculationRules(incomeTaxPath, ref _incomeTaxRules);

                CalculateTaxableIncome();
                CalculateSuperannuation();
                CalculateMedicareLevy();
                CalculateBudgetRepairLevy();
                CalculateIncomeTax();
                CalculateNetIncome();
                CalculatePayPacket();
            }
            catch (Exception ex)
            {
                // for exceptions i haven't thought about :(
                Console.WriteLine("An unhandled exception has occurred. Please review the exception message and report to your administrator.");
                Console.WriteLine(ex.Message);
            }
        }

        /*
         * This same function is used for all of the different Rule Files. All of the Rule Files have the same syntax and format.
         */
        private static void LoadCalculationRules(string filepath, ref List<CalculationRule> calculationRules)
        {
            try
            {
                //https://stackoverflow.com/questions/5282999/reading-csv-file-and-storing-values-into-an-array
                //referencing stackoverflow for this specific code because it's almost verbatim.
                using (var reader = new StreamReader(filepath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = string.IsNullOrEmpty(line) ? string.Empty.Split(',') : line.Split(',');

                        try
                        {
                            var calculationRule = new CalculationRule(int.Parse(values[0]), int.Parse(values[1]), values[2]);
                            calculationRules.Add(calculationRule);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("There has been an exception trying to parse one of the calculation rules. Please check the file for any errors.");
                            Console.WriteLine(filepath);
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("There has been an exception trying to load one of the Rule Files. Please check if the file exists and if permissions are valid.");
                Console.WriteLine(filepath);
                Console.WriteLine(ex.Message);
            }
        }

        /*
         * Very basic taxable income calculation
         */
        private void CalculateTaxableIncome()
        {
            TaxableIncome = GrossSalary / ((100 + SuperannuationPercentage) / 100);
        }

        /*
         * Very basic superannuation calculation
         */
        private void CalculateSuperannuation()
        {
            Superannuation = GrossSalary - TaxableIncome;
        }

        /*
         * The Rule Files have been designed in such a way that the same function can perform the calculations for all three Medicare, Budget and Income.
         */
        private decimal CalculateRule(IEnumerable<CalculationRule> rules)
        {
            decimal deduction = 0;
            foreach (var cr in rules)
            {
                // where the upper threshold is 0, there is no upper limit 
                if (TaxableIncome >= cr.LowerThreshold && (TaxableIncome <= cr.UpperThreshold || cr.UpperThreshold == 0))
                {
                    var expression = cr.Expression.Replace("TI", TaxableIncome.ToString(CultureInfo.InvariantCulture));

                    var dt = new DataTable();
                    try
                    {
                        // where the expression 0, there is no formula to compute. 
                        deduction = expression.Equals("0") ? 0 : decimal.Ceiling((decimal)dt.Compute(expression, ""));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("There has been an exception trying to execute one of the Rule File expressions. Please check the expression and file for any errors.");
                        Console.WriteLine(expression);
                        Console.WriteLine(ex.Message);
                    }

                    // Not sure if I like using break. 
                    // But, once we've reached this part of the execution there's no longer any need to continue the loop.
                    // Doing so will do no harm, but breaking will also do no harm.
                    break;
                }
            }
            return deduction;
        }

        /*
         * Medicare Levy utilises Rule Files
         */
        private void CalculateMedicareLevy()
        {
            MedicareLevy = CalculateRule(_medicareLevyRules);
        }

        /*
         * Bduget Repair Levy utilises Rule Files
         */
        private void CalculateBudgetRepairLevy()
        {
            BudgetRepairLevy = CalculateRule(_budgetRepairLevyRules);
        }

        /*
         * Income Tax Levy utilises Rule Files
         */
        private void CalculateIncomeTax()
        {
            IncomeTax = CalculateRule(_incomeTaxRules);
        }

        /*
         * Very basic Net Income calculation
         */
        private void CalculateNetIncome()
        {
            var netIncome = GrossSalary - Superannuation - MedicareLevy - BudgetRepairLevy - IncomeTax;
            NetIncome = netIncome;
        }

        /*
         * This pay packet function assumes that the pay packet character has been validated (W,F,M) and massaged (ToUpper).
         */
        private void CalculatePayPacket()
        {
            string payPacket;
            switch (PayFrequency)
            {
                case 'W':
                    payPacket = ((NetIncome / 365) * 7).ToString("C", CultureInfo.CurrentCulture) + " per week";
                    break;
                case 'F':
                    payPacket = ((NetIncome / 365) * 14).ToString("C", CultureInfo.CurrentCulture) + " per fortnight";
                    break;
                case 'M':
                    payPacket = (NetIncome / 12).ToString("C", CultureInfo.CurrentCulture) + " per month";
                    break;
                default:
                    payPacket = "";
                    break;
            }

            PayPacket = payPacket;
        }
    }
}