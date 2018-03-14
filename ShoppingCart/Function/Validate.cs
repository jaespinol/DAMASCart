using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ShoppingCart.Function
{
    public static class Validate
    {
        public static async Task<bool> TextValidate(this Page page, string title, string accept, params ValidateItem[] items)
        {
            int total = items.Length;
            int count = 0;
            foreach (var item in items)
            {
                if (string.IsNullOrEmpty(item.Text ?? ""))
                {
                    await page.DisplayAlert(title, item.Message, accept);
                    return count == total;
                }
                else
                {
                    if (!string.IsNullOrEmpty(item.Regex) && !item.Match())
                    {
                        await page.DisplayAlert(title, item.RegexMessage, accept);
                        return count == total;
                    }
                    else
                    {
                        count++;
                    }
                }
            }
            return count == total;
        }
    }

    public class ValidateItem
    {
        public string Text { get; set; }
        public string Message { get; set; }
        public string Regex { get; set; }
        public string RegexMessage { get; set; }
        private Regex RegExpression { get; set; }
        private ValidationType Type { get; set; }

        public ValidateItem(string text, string message, string regex = "", string regexmessageonfail = "")
        {
            Text = text;
            Message = message;
            Regex = regex;
            RegExpression = new Regex(Regex);
            RegexMessage = regexmessageonfail;
        }

        public ValidateItem(string text, string message, ValidationType validationtype, string regexmessageonfail = "")
        {
            Text = text;
            Message = message;
            Type = validationtype;
            Regex = "";
            if (validationtype == ValidationType.Email)
            {
                Regex = "@";
            }
            else if (validationtype == ValidationType.Phone)
            {
                Regex = @"^(?:(?:\+?1\s*(?:[.-]\s*)?)?(?:\(\s*([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9])\s*\)|([2-9]1[02-9]|[2-9][02-8]1|[2-9][02-8][02-9]))\s*(?:[.-]\s*)?)?([2-9]1[02-9]|[2-9][02-9]1|[2-9][02-9]{2})\s*(?:[.-]\s*)?([0-9]{4})(?:\s*(?:#|x\.?|ext\.?|extension)\s*(\d+))?$";
            }
            RegExpression = new Regex(Regex);
            RegexMessage = regexmessageonfail;
        }

        public bool Match()
        {
            if (Type == ValidationType.Email)
            {
                var emails = new string[]
                {
                    "@gmail", "@hotmail", "@live", "@yahoo", "@"
                };
                var ends = new string[]
                {
                    ".com", ".net", ".mx", ".es"
                };
                return emails.Any(e => Text.Contains(e)) && ends.Any(e => Text.EndsWith(e));
            }
            else if(Type == ValidationType.CreditCardNumber)
            {
                var textchars = Text.ToCharArray();
                var creditcard = textchars.ToList();
                int sumOfDigits = creditcard.Where((e) => e >= '0' && e <= '9')
                    .Reverse()
                    .Select((e, i) => ((int)e - 48) * (i % 2 == 0 ? 1 : 2))
                    .Sum((e) => e / 10 + e % 10);
                return sumOfDigits % 10 == 0;
            }
            else if (Type == ValidationType.CreditCardDate)
            {
                if(Text.Length == 5)
                {
                    if (Text.Contains("/"))
                    {
                        var date = Text.Split('/');
                        if (date.Length == 2)
                        {
                            var month = 0;
                            var year = 0;
                            if(int.TryParse(date[0], out month))
                            {
                                if (month > 0 && month < 13)
                                {
                                    if (date[1].Length == 4)
                                    {
                                        if (int.TryParse(date[1], out year))
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (Type == ValidationType.CreditCardCVV)
            {
                if (Text.Length == 3)
                {
                    var cvv = 0;
                    return int.TryParse(Text, out cvv);
                }
            }
            else if(Type == ValidationType.Phone)
            {
                return RegExpression.IsMatch(Text);
            }
            else
            {
                return RegExpression.IsMatch(Text);
            }
            return false;
        }
    }

    public enum ValidationType
    {
        Email, CreditCardNumber, CreditCardDate, CreditCardCVV,
        Phone
    }
}
