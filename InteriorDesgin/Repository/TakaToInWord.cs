using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace InteriorDesign.Repository
{
    public class TakaToInWord
    {
        public static string NumberToWords(int number , string cur)
        {
            if (number == 0)
                return "Zero";

            if (number < 0)
                return "Minus " + NumberToWords((Math.Abs(number)), cur);

            string words = "";


            if (cur == "TK" || cur == "TAKA" || cur == "RS" || cur == "RUPEE")
            {

                if ((number / 10000000) > 0)
                {
                    words += NumberToWords((number / 10000000), cur) + " Core ";
                    number %= 10000000;
                }


                if ((number / 100000) > 0)
                {
                    words += NumberToWords((number / 100000), cur) + " Lac ";
                    number %= 100000;
                }
            }
            else
            {

                if ((number / 1000000000) > 0)
                {
                    words += NumberToWords((number / 1000000000), cur) + " Billion ";
                    number %= 1000000000;
                }


                if ((number / 1000000) > 0)
                {
                    words += NumberToWords((number / 1000000), cur) + " Million ";
                    number %= 1000000;
                }
                

                    
            }

            if ((number / 1000) > 0)
            {
                words += NumberToWords((number / 1000) , cur )+ " Thousand ";
                number %= 1000;
            }

            if ((number / 100) > 0)
            {
                words += NumberToWords((number / 100), cur) + " Hundred ";
                number %= 100;
            }

            if (number > 0)
            {
                //if (words != "")
                //    words += " and ";

                var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
                var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

                if (number < 20)
                    words += unitsMap[number];
                else
                {
                    words += tensMap[number / 10];
                    if ((number % 10) > 0)
                        words += "-" + unitsMap[number % 10];
                }
            }

            return words;
        }


        public static string DecimalToWords(decimal d, string Cur)
        {
            //Grab a string form of your decimal value ("12.34")
            var formatted = d.ToString();
            int indexOfPecision = formatted.IndexOf(".");
            formatted = formatted.Substring(0, indexOfPecision + 3);

            string[] sides = formatted.Split('.');

            if (formatted.Contains("."))
            {
                //If it contains a decimal point, split it into both sides of the decimal
               
                //Process each side and append them with "and", "dot" or "point" etc.
                if (Cur.ToUpper() == ("Tk").ToUpper() || Cur.ToUpper() == ("Taka").ToUpper() || Cur.ToUpper() == ("Rs").ToUpper() || Cur.ToUpper() == ("Rupee").ToUpper())
                {
                    return "In Word ("+ Cur + "): " + NumberToWords(Int32.Parse(sides[0]), Cur.ToUpper()) + " and " + NumberToWords(Int32.Parse(sides[1]), Cur.ToUpper()) + " Paisa Only.";
                } else if (Cur.ToUpper() == ("USD").ToUpper())
                {
                    return "In Word (" + Cur + "): " + NumberToWords(Int32.Parse(sides[0]), Cur.ToUpper()) + " and " + NumberToWords(Int32.Parse(sides[1]), Cur.ToUpper()) + " Cent Only.";
                }
                else if (Cur.ToUpper() == ("EUR").ToUpper())
                {
                    return "In Word (" + Cur + "): " + NumberToWords(Int32.Parse(sides[0]), Cur.ToUpper()) + " and " + NumberToWords(Int32.Parse(sides[1]), Cur.ToUpper()) + " Cent Only.";
                }
                else
                {
                    return "In Word (" + Cur + "): " + NumberToWords(Int32.Parse(sides[0]), Cur.ToUpper()) + " Only.";
                }
            }
            else
            {

                return "In Word (" + Cur + "): " + NumberToWords(Int32.Parse(sides[0]), Cur.ToUpper()) + " Only.";
               
                //Else process as normal
               
            }
        }




    }
}