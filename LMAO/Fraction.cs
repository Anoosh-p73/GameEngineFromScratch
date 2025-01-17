using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CPI311.Labs
{
    internal class Fraction
    {

        private int numerator;
        private int denominator;

        public int Numerator {
            get { return numerator; }
            set { numerator = value; Simplify(); }
        }
        int Denominator {
            get { return denominator; }
            set { denominator = value; Simplify(); }
        }

        public Fraction(int n = 0, int d = 1) { 
            numerator = n;
            denominator = d == 0 ? 1 : d;
            Simplify();
        }

        /*public static Fraction multiply(Fraction a, Fraction b)
        {
            return new Fraction(a.numerator * b.numerator, a.denominator * b.denominator);
        }*/


        public static Fraction operator *(Fraction a, Fraction b)
        {
            return new Fraction(a.numerator * b.numerator, a.denominator * b.denominator);
        }


        public static Fraction operator +(Fraction a, Fraction b)
        {
            return new Fraction(a.numerator * b.denominator + a.denominator * b.numerator, a.denominator * b.denominator);
        }

        public static Fraction operator -(Fraction a, Fraction b)
        {
            Fraction temp =  new Fraction(a.numerator * b.denominator - a.denominator * b.numerator, a.denominator * b.denominator);
            if (temp.denominator < 0)
            {
                temp.denominator *= -1;
                temp.numerator *= -1;   

            }
            return temp;
        }

        public static Fraction operator /(Fraction a, Fraction b)
        {
            return new Fraction(a.numerator * b.denominator, a.denominator * b.numerator);
        }

        public override string ToString()
        {
            return numerator + "/" + denominator;

        }
        private void Simplify()
        {
            if (denominator < 0)
            {
                denominator *= -1;
                numerator *= -1;
            }
            int gcd = GCD(Math.Max(numerator, denominator), Math.Min(numerator, denominator));
            numerator /= gcd;
            denominator /= gcd;
        }

        private static int GCD(int bigger, int smaller) {

            if(smaller == 0)
                return bigger;
            return GCD(smaller, bigger % smaller);

        }

    }
}
