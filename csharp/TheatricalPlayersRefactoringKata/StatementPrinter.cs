using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        private readonly PrintType printType;
        private readonly CultureInfo cultureInfo;

        public StatementPrinter(PrintType printType, CultureInfo cultureInfo)
        {
            this.printType = printType;
            this.cultureInfo = cultureInfo;
        }

        public StatementPrinter(PrintType printType) : this(printType, new CultureInfo("en-US"))
        {
        }

        public StatementPrinter() : this(PrintType.Text, new CultureInfo("en-US"))
        {
        }

        private readonly Dictionary<PrintType, (string statement, string playInfo, string price, string credits)> TextFormats = new()
        {
            {
                PrintType.Text, ("Statement for {0}\n", "  {0}: {1:C} ({2} seats)\n", "Amount owed is {0:C}\n", "You earned {0} credits\n")
            }
        };


        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;
            var credits = 0;
            var result = string.Format(TextFormats[printType].statement, invoice.Customer);

            foreach(var performance in invoice.Performances) 
            {
                var play = plays[performance.PlayID];
                var performanceAmount = 0;
                switch (play.Type) 
                {
                    case "tragedy":
                        performanceAmount = 40000;
                        if (performance.Audience > 30) {
                            performanceAmount += 1000 * (performance.Audience - 30);
                        }
                        break;
                    case "comedy":
                        performanceAmount = 30000;
                        if (performance.Audience > 20) {
                            performanceAmount += 10000 + 500 * (performance.Audience - 20);
                        }
                        performanceAmount += 300 * performance.Audience;
                        break;
                    default:
                        throw new Exception("unknown type: " + play.Type);
                }
                // add volume credits
                credits += Math.Max(performance.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == play.Type) credits += (int)Math.Floor((decimal)performance.Audience / 5);

                // print line for this order
                result += String.Format(cultureInfo, TextFormats[printType].playInfo, play.Name, Convert.ToDecimal(performanceAmount / 100), performance.Audience);
                totalAmount += performanceAmount;
            }
            result += String.Format(cultureInfo, TextFormats[printType].price, Convert.ToDecimal(totalAmount / 100));
            result += String.Format(TextFormats[printType].credits, credits);
            return result;
        }
    }
}
