using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace SalaryDetailer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Enter your salary package amount: ");
            String input = Console.ReadLine();

            /*
             * Number group separators (commas) are acceptable. 
             * If copied and pasted from a source with commas, replace with nothing.
             */
            input.Replace(CultureInfo.CurrentUICulture.NumberFormat.NumberGroupSeparator, "");

            decimal grossSalary;
            if (decimal.TryParse(input, out grossSalary))
            {
                char payFreq = 'W';

                Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");
                payFreq = Console.ReadLine().ToUpper()[0];
                while (payFreq != 'W' && payFreq != 'F' && payFreq != 'M')
                {
                    Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");
                    payFreq = Console.ReadLine().ToUpper()[0];
                }

                // get super percentage from App Settings
                decimal superannuationPercentage;
                decimal.TryParse(ConfigurationManager.AppSettings["superannuationPercentage"], out superannuationPercentage);

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

                Console.WriteLine("Pay packet: " + Salary.PayPacket.ToString("C", CultureInfo.CurrentCulture));
            }
            else
            {
                Console.WriteLine("The entered amount is not a valid salary value.");
            }

            Console.Write("\nPress any key to end...");
            Console.ReadKey();
        }
    }
}
