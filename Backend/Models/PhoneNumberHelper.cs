using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace EHisseler.Web.Models
{
    public record MobileOperator(string Code, string Name);

    public static class PhoneNumberHelper
    {
        public const string CountryCode = "994";

        // Operator prefiksləri — asan yenilənə bilən siyahı.
        // Yeni prefiks əlavə etmək üçün yalnız bu siyahını dəyişmək kifayətdir.
        public static readonly IReadOnlyList<MobileOperator> Operators = new List<MobileOperator>
        {
            new("10", "Azercell"),
            new("50", "Azercell"),
            new("51", "Azercell"),
            new("55", "Bakcell"),
            new("99", "Bakcell"),
            new("60", "Naxtel"),
            new("70", "Nar"),
            new("77", "Nar"),
        };

        public static IEnumerable<string> ValidPrefixes => Operators.Select(o => o.Code);

        // Abunəçi hissəsi: xxx-xx-xx (7 rəqəm)
        public static readonly Regex SubscriberPattern = new(@"^\d{3}-\d{2}-\d{2}$", RegexOptions.Compiled);

        /// <summary>
        /// Qeydiyyat formasından gələn operator kodu + "xxx-xx-xx" formatlı nömrəni yoxlayır.
        /// </summary>
        public static bool TryBuildFromParts(string? operatorCode, string? subscriber, out string? normalizedTel, out string? error)
        {
            normalizedTel = null;
            error = null;

            if (string.IsNullOrWhiteSpace(operatorCode) || !ValidPrefixes.Contains(operatorCode.Trim()))
            {
                error = "Operator kodu seçilməlidir.";
                return false;
            }

            var sub = (subscriber ?? string.Empty).Trim();
            if (!SubscriberPattern.IsMatch(sub))
            {
                error = "Nömrə xxx-xx-xx formatında olmalıdır (məs: 123-45-67).";
                return false;
            }

            normalizedTel = "+" + CountryCode + operatorCode.Trim() + sub.Replace("-", "");
            return true;
        }

        public static bool TryNormalizeAzNumber(string? input, out string? normalizedWa, out string? normalizedTel)
        {
            normalizedWa = null;
            normalizedTel = null;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Yalnız rəqəmləri alırıq (boşluq, defis, mötərizə və '+' işarəsi silinir)
            string digits = new string(input.Where(char.IsDigit).ToArray());

            string? numberWithoutCode = null;

            if (digits.StartsWith(CountryCode) && digits.Length == 12)
                numberWithoutCode = digits.Substring(3);
            else if (digits.StartsWith("0") && digits.Length == 10)
                numberWithoutCode = digits.Substring(1);
            else if (digits.Length == 9)
                numberWithoutCode = digits;

            if (numberWithoutCode == null || numberWithoutCode.Length != 9)
                return false;

            string prefix = numberWithoutCode.Substring(0, 2);
            if (!ValidPrefixes.Contains(prefix))
                return false;

            normalizedWa = CountryCode + numberWithoutCode;
            normalizedTel = "+" + CountryCode + numberWithoutCode;
            return true;
        }

        /// <summary>+994501234567 → "+994 50 123 45 67" (ekranda göstərmək üçün)</summary>
        public static string? FormatDisplay(string? input)
        {
            if (!TryNormalizeAzNumber(input, out var wa, out _)) return null;
            var n = wa!.Substring(3);
            return $"+994 {n.Substring(0, 2)} {n.Substring(2, 3)} {n.Substring(5, 2)} {n.Substring(7, 2)}";
        }
    }
}
