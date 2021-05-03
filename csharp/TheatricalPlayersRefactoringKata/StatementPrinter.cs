﻿using System;
using System.Collections.Generic;
using System.Globalization;

namespace TheatricalPlayersRefactoringKata
{
    public class StatementPrinter
    {
        private readonly Dictionary<PrintType, (string statement, string playInfo, string price, string credits)> TextFormats = new()
        {
            {
                PrintType.Text, ("Statement for {0}\n", "  {0}: {1:C} ({2} seats)\n", "Amount owed is {0:C}\n", "You earned {0} credits\n")
            }
        };
        public string Print(Invoice invoice, Dictionary<string, Play> plays, PrintType printType = PrintType.Text)
        {
            var totalAmount = 0;
            var volumeCredits = 0;
            var result = string.Format(TextFormats[printType].statement, invoice.Customer);
            CultureInfo cultureInfo = new CultureInfo("en-US");

            foreach(var perf in invoice.Performances) 
            {
                var play = plays[perf.PlayID];
                var thisAmount = 0;
                switch (play.Type) 
                {
                    case "tragedy":
                        thisAmount = 40000;
                        if (perf.Audience > 30) {
                            thisAmount += 1000 * (perf.Audience - 30);
                        }
                        break;
                    case "comedy":
                        thisAmount = 30000;
                        if (perf.Audience > 20) {
                            thisAmount += 10000 + 500 * (perf.Audience - 20);
                        }
                        thisAmount += 300 * perf.Audience;
                        break;
                    default:
                        throw new Exception("unknown type: " + play.Type);
                }
                // add volume credits
                volumeCredits += Math.Max(perf.Audience - 30, 0);
                // add extra credit for every ten comedy attendees
                if ("comedy" == play.Type) volumeCredits += (int)Math.Floor((decimal)perf.Audience / 5);

                // print line for this order
                result += String.Format(cultureInfo, TextFormats[printType].playInfo, play.Name, Convert.ToDecimal(thisAmount / 100), perf.Audience);
                totalAmount += thisAmount;
            }
            result += String.Format(cultureInfo, TextFormats[printType].price, Convert.ToDecimal(totalAmount / 100));
            result += String.Format(TextFormats[printType].price, volumeCredits);
            return result;
        }
    }
}
