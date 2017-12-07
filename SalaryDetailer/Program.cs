using System;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Diagnostics;

namespace SalaryDetailer
{
    internal class Program
    {
        private static void Main()
        {
            /*
            * Read user input for salary package amount. 
            * Strip known valid characters.
            * try parse; loop whilst false.
            */
            Console.Write("Enter your salary package amount: ");
            var input = Console.ReadLine();
            input = string.IsNullOrEmpty(input) ? string.Empty : input;
            input = input.Replace(CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator, "");
            input = input.Replace(CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol, "");

            decimal grossSalary;
            while (!decimal.TryParse(input, out grossSalary))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Please enter a valid salary package amount.\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Enter your salary package amount: ");
                input = Console.ReadLine();
                input = string.IsNullOrEmpty(input) ? string.Empty : input;
                input = input.Replace(CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator, "");
                input = input.Replace(CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol, "");
            }

            /*
            * Read user input for pay frequency. 
            * Massage the input, grab the first character.
            * Validate input; loop whilst false.
            */
            Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");
            input = Console.ReadLine();
            var payFreq = string.IsNullOrEmpty(input) ? '\0' : input.ToUpper()[0];
            while (payFreq != 'W' && payFreq != 'F' && payFreq != 'M')
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Please enter a valid pay frequency.\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");

                input = Console.ReadLine();
                payFreq = string.IsNullOrEmpty(input) ? '\0' : input.ToUpper()[0];
            }

            /*
            * Get super percentage from App Settings. Considering that the RuleFiles are separated, perhaps the Super Amount should be too?
            * Ideally, super percentage would be another user input, but this is to spec.
            */
            decimal.TryParse(ConfigurationManager.AppSettings["superannuationPercentage"], out var superannuationPercentage);

            /*
             * Paths to rule files
             */
            var folder = Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName) + @"\RuleFiles\";
            var medicareLevyRulesPath = folder + "MedicareLevyRules.csv";
            var budgetRepairLevyRulesPath = folder + "BudgetRepairLevyRules.csv";
            var incomeTaxPath = folder + "IncomeTaxRules.csv";

            /*
            * Instantiate new Salary class / object
            * Calculations are performed on construction
            */
            var salary = new Salary(grossSalary, superannuationPercentage, payFreq, medicareLevyRulesPath, budgetRepairLevyRulesPath, incomeTaxPath);

            Console.WriteLine("\nCalculating salary details...\n");
            Console.WriteLine("Gross package: " + salary.GrossSalary.ToString("C", CultureInfo.CurrentCulture));
            Console.WriteLine("Superannuation: " + salary.Superannuation.ToString("C", CultureInfo.CurrentCulture));
            Console.WriteLine("\nTaxable income: " + salary.TaxableIncome.ToString("C", CultureInfo.CurrentCulture));

            Console.WriteLine("\nDeductions:");
            Console.WriteLine("Medicare Levy: " + salary.MedicareLevy.ToString("C", CultureInfo.CurrentCulture));
            Console.WriteLine("Budget Repair Levy: " + salary.BudgetRepairLevy.ToString("C", CultureInfo.CurrentCulture));

            Console.WriteLine("Income Tax: " + salary.IncomeTax.ToString("C", CultureInfo.CurrentCulture));

            Console.WriteLine("\nNet income: " + salary.NetIncome.ToString("C", CultureInfo.CurrentCulture));

            Console.WriteLine("Pay packet: " + salary.PayPacket);

            Console.WriteLine("\nPress any key to end...");
            Console.ReadKey();
        }
    }
}
