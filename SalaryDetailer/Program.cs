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
             * Basic string manipulation. If copied and pasted from a source with commas, replace with nothing.
             * Commas are acceptable.
             */
            input.Replace(",", "");

            /*
             * decimal probably most appropriate type for grossSalary
             */
            float grossSalary;
            if (float.TryParse(input, out grossSalary))
            {
                //good
                Console.Write("Enter your pay frequency (W for weekly, F for fortnightly, M for monthly): ");
                String payFreq = Console.ReadLine();
                // TODO parse the pay frequency 

                // get super percentage from App Settings
                float superannuationPercentage;
                float.TryParse(ConfigurationManager.AppSettings["superannuationPercentage"], out superannuationPercentage);

                // calculate Taxable income
                float taxableIncome = grossSalary / ((100 + superannuationPercentage) / 100);
                float superannuation = grossSalary - taxableIncome;

                Console.WriteLine("\nCalculating salary details...\n");
                Console.WriteLine("Gross package: " + grossSalary.ToString("C", CultureInfo.CurrentCulture));
                Console.WriteLine("Superannuation: " + superannuation.ToString("C", CultureInfo.CurrentCulture));
                Console.WriteLine("\nTaxable income: " + taxableIncome.ToString("C", CultureInfo.CurrentCulture));

                Console.WriteLine("\nDeductions:");

                // calculate Medicare Levy
                float medicareLevy = 0;
                if (taxableIncome > 21335 && taxableIncome <= 26668)
                    medicareLevy = (taxableIncome - 21335) * (float)0.1;
                else if (taxableIncome > 26669)
                    medicareLevy = taxableIncome * (float)0.02;
                Console.WriteLine("Medicare Levy: " + medicareLevy.ToString("C", CultureInfo.CurrentCulture));

                // calculate Budget Repair Levy
                float budgetRepairLevy = 0;
                if (taxableIncome > 180000)
                    budgetRepairLevy = (taxableIncome - 180000) * (float)0.02;
                Console.WriteLine("Budget Repair Levy: " + budgetRepairLevy.ToString("C", CultureInfo.CurrentCulture));

                // calculate Income Tax
                float incomeTax = 0;
                if (taxableIncome > 18200 && taxableIncome <= 37000)
                    incomeTax = (taxableIncome - 18200) * (float)0.19;
                else if (taxableIncome > 37000 && taxableIncome <= 87000)
                    incomeTax = 3572 + ((taxableIncome - 37000) * (float)0.325);
                else if (taxableIncome > 87000 && taxableIncome <= 180000)
                    incomeTax = 19822 + ((taxableIncome - 87000) * (float)0.37);
                else if (taxableIncome > 180001)
                    incomeTax = 54232 + ((taxableIncome - 180000) * (float)0.47);
                Console.WriteLine("Income Tax: " + incomeTax.ToString("C", CultureInfo.CurrentCulture));

                // calculate Net income
                float netIncome = grossSalary - superannuation - medicareLevy - budgetRepairLevy - incomeTax;
                Console.WriteLine("\nNet income: " + netIncome.ToString("C", CultureInfo.CurrentCulture));

                // calculate Pay packet
                float payPacket = 0;
                switch (payFreq.ToUpper())
                {
                    case "W":
                        payPacket = (netIncome / 365) * 7;
                        break;
                    case "F":
                        payPacket = (netIncome / 365) * 14;
                        break;
                    case "M":
                        payPacket = netIncome / 12;
                        break;
                    default:
                        Console.Write("An invalid choice was selected for Pay Frequency.\n");
                        break;
                }
                Console.WriteLine("Pay packet: " + payPacket.ToString("C", CultureInfo.CurrentCulture));
            }
            else
            {
                //bad
                Console.WriteLine("The entered amount is not a valid salary value.");
            }

            Console.Write("\nPress any key to end...");
            Console.ReadKey();
        }
    }
}
