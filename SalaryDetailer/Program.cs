using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.IO;

namespace SalaryDetailer
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
                * Read user input for salary package amount. 
                * Strip known valid characters.
                * try parse; whilst false
                */

            Console.Write("Enter your salary package amount: ");
            string input = Console.ReadLine();
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
                input = input.Replace(CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator, "");
                input = input.Replace(CultureInfo.CurrentUICulture.NumberFormat.CurrencySymbol, "");
            }

            /*
            * Read user input for pay frequency. 
            * Massage the input, grab the first character.
            * Validate input; whilst false
            */
            Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");
            char payFreq = Console.ReadLine().ToUpper()[0];
            while (payFreq != 'W' && payFreq != 'F' && payFreq != 'M')
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Please enter a valid pay frequency.\n");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");
                payFreq = Console.ReadLine().ToUpper()[0];
            }
            /*
            * Get super percentage from App Settings
            * Instantiate new 
            */
            decimal.TryParse(ConfigurationManager.AppSettings["superannuationPercentage"], out decimal superannuationPercentage);
            Salary Salary = new Salary(grossSalary, superannuationPercentage, payFreq);

            Console.WriteLine("\nCalculating salary details...\n");
            Console.WriteLine("Gross package: " + Salary.GrossSalary.ToString("C", CultureInfo.CurrentCulture));
            Console.WriteLine("Superannuation: " + Salary.Superannuation.ToString("C", CultureInfo.CurrentCulture));
            Console.WriteLine("\nTaxable income: " + Salary.TaxableIncome.ToString("C", CultureInfo.CurrentCulture));

            Console.WriteLine("\nDeductions:");
            Console.WriteLine("Medicare Levy: " + Salary.MedicareLevy.ToString("C", CultureInfo.CurrentCulture));
            Console.WriteLine("Budget Repair Levy: " + Salary.BudgetRepairLevy.ToString("C", CultureInfo.CurrentCulture));

            Console.WriteLine("Income Tax: " + Salary.IncomeTax.ToString("C", CultureInfo.CurrentCulture));

            Console.WriteLine("\nNet income: " + Salary.NetIncome.ToString("C", CultureInfo.CurrentCulture));

            Console.WriteLine("Pay packet: " + Salary.PayPacket);

            Console.Write("\nPress any key to end...");
            Console.ReadKey();
        }
    }
}
