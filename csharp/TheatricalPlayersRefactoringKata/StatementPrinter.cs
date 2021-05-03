using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        private readonly PrintType printType;
        private readonly CultureInfo cultureInfo;

        public StatementPrinter(CultureInfo cultureInfo, PrintType printType = PrintType.Text)
        {
            this.printType = printType;
            this.cultureInfo = cultureInfo;
        }

        public StatementPrinter() : this(new CultureInfo("en-US"))
        {
        }

        private delegate int CostComputer(Performance performance, ref int credits);

        private readonly Dictionary<PlayType, CostComputer> costFunctions = new()
        {
            {PlayType.Tragedy, ComputeTragedy},
            {PlayType.Comedy, ComputeComedy}
        };

        private readonly Dictionary<PrintType, (string statement, string playInfo, string price, string credits)> textFormats = new()
        {
            {
                PrintType.Text, ("Statement for {0}\n", "  {0}: {1:C} ({2} seats)\n", "Amount owed is {0:C}\n", "You earned {0} credits\n")
            },
            {
                PrintType.HTML, ("<html>\n  <h1>Statement for {0}</h1>\n  <table>\n    <tr><th>play</th><th>seats</th><th>cost</th></tr>\n",
                    "    <tr><td>{0}</td><td>{2}</td><td>{1:C}</td></tr>\n",
                    "  </table>\n  <p>Amount owed is <em>{0:C}</em></p>\n",
                    "  <p>You earned <em>{0}</em> credits</p>\n</html>")
            }
        };

        private static int ComputeTragedy(Performance performance, ref int credits)
        {
            var performanceAmount = 40000;
            if (performance.Audience > 30) {
                performanceAmount += 1000 * (performance.Audience - 30);
            }

            return performanceAmount;
        }

        private static int ComputeComedy(Performance performance, ref int credits)
        {
            var performanceAmount = 30000 + 300 * performance.Audience;
            if (performance.Audience > 20) {
                performanceAmount += 10000 + 500 * (performance.Audience - 20);
            }
            // add extra credit for every ten comedy attendees
            credits += (int)Math.Floor((decimal)performance.Audience / 5);
            return performanceAmount;
        }

        public string Print(Invoice invoice, Dictionary<string, Play> plays)
        {
            var totalAmount = 0;
            var credits = 0;
            var result = string.Format(textFormats[printType].statement, invoice.Customer);

            foreach(var performance in invoice.Performances) 
            {
                var play = plays[performance.PlayID];
                if (!costFunctions.ContainsKey(play.Type))
                {
                    throw new Exception("unknown type: " + play.Type);
                }

                var performanceAmount = costFunctions[play.Type](performance, ref credits);
                
                // add volume credits
                credits += Math.Max(performance.Audience - 30, 0);

                // print line for this order
                result += string.Format(cultureInfo, textFormats[printType].playInfo, play.Name, Convert.ToDecimal(performanceAmount / 100), performance.Audience);
                totalAmount += performanceAmount;
            }
            result += string.Format(cultureInfo, textFormats[printType].price, Convert.ToDecimal(totalAmount / 100));
            result += string.Format(textFormats[printType].credits, credits);
            return result;
        }
    }
}
