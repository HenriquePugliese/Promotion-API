using Acropolis.Application.Base.Extensions;

namespace Acropolis.Application.Features.Promotions.Validators
{
    public static class CnpjValidator
    {
        private static readonly int[] _multi1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        private static readonly int[] _multi2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        public static bool Validate(string cnpj)
        {
            if (cnpj == null)
                return false;

            const int cnpjLengthValid = 14;

            cnpj = cnpj.OnlyNumbers();

            if (cnpj.Length != cnpjLengthValid || cnpj.All(c => c == '0'))
                return false;

            var tempCnpj = cnpj[..12];
            var sum = 0;

            for (int i = 0; i < 12; i++)
            {
                sum += (tempCnpj[i] - '0') * _multi1[i];
            }

            var rest = (sum % 11);
            
            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            var digit = rest.ToString();
            
            tempCnpj += digit;
            sum = 0;

            for (int i = 0; i < 13; i++)
                sum += (tempCnpj[i] - '0') * _multi2[i];

            rest = (sum % 11);

            if (rest < 2)
                rest = 0;
            else
                rest = 11 - rest;

            digit += rest;

            return cnpj.EndsWith(digit);
        }
    }
}
