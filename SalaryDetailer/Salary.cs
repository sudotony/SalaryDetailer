using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;

namespace SalaryDetailer
{
    public class Salary
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

        private List<CalculationRule> _medicareLevyRules;
        private List<CalculationRule> _budgetRepairLevyRules;
        private List<CalculationRule> _incomeTaxRules;

        /*
         * A basic struct to store the calculation rules.
         * lower and upper thresholds specify the taxable income range
         * The expression is a mathematical string with a substring TI that is to be replaced with the taxable income. 
         */
        public struct CalculationRule
        {
            public int lowerThreshold;
            public int upperThreshold;
            public string expression;

            public CalculationRule(int p1, int p2, string p3)
            {
                lowerThreshold = p1;
                upperThreshold = p2;
                expression = p3;
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
                _grossSalary = grossSalary;
                _superannuationPercentage = superannuationPercentage;
                _payFrequency = payFrequency;

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
        private void LoadCalculationRules(string filepath, ref List<CalculationRule> calculationRules)
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
                        var values = line.Split(',');

                        try
                        {
                            CalculationRule calculationRule = new CalculationRule(int.Parse(values[0]), int.Parse(values[1]), values[2]);
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
            _taxableIncome = _grossSalary / ((100 + _superannuationPercentage) / 100);
        }

        /*
         * Very basic superannuation calculation
         */
        private void CalculateSuperannuation()
        {
            _superannuation = _grossSalary - _taxableIncome;
        }

        /*
         * The Rule Files have been designed in such a way that the same function can perform the calculations for all three Medicare, Budget and Income.
         */
        private void CalculateRule(List<CalculationRule> rules, ref decimal deduction)
        {
            foreach (CalculationRule cr in rules)
            {
                // where the upper threshold is 0, there is no upper limit 
                if (_taxableIncome >= cr.lowerThreshold && (_taxableIncome <= cr.upperThreshold || cr.upperThreshold == 0))
                {
                    string expression = cr.expression.Replace("TI", _taxableIncome.ToString());

                    DataTable dt = new DataTable();
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
        }

        /*
         * Medicare Levy utilises Rule Files
         */
        private void CalculateMedicareLevy()
        {
            CalculateRule(_medicareLevyRules, ref _medicareLevy);
        }

        /*
         * Bduget Repair Levy utilises Rule Files
         */
        private void CalculateBudgetRepairLevy()
        {
            CalculateRule(_budgetRepairLevyRules, ref _budgetRepairLevy);
        }

        /*
         * Income Tax Levy utilises Rule Files
         */
        private void CalculateIncomeTax()
        {
            CalculateRule(_incomeTaxRules, ref _incomeTax);
        }

        /*
         * Very basic Net Income calculation
         */
        private void CalculateNetIncome()
        {
            decimal netIncome = _grossSalary - _superannuation - _medicareLevy - _budgetRepairLevy - _incomeTax;
            _netIncome = netIncome;
        }

        /*
         * This pay packet function assumes that the pay packet character has been validated and massaged (ToUpper).
         */
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