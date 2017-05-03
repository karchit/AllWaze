﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllWaze.Objects
{
    public static class Currency
    {
        public static Dictionary<string, string> currencies = new Dictionary<string, string>
        {
            { "AED", "د.إ.‏"},
            {"AFN", "؋"},
            {"ALL", "Lekë"},
            {"AMD", "֏"},
            {"ARS", "$"},
            {"AUD", "$"},
            {"AZN", "₼"},
            {"BAM", "КМ"},
            {"BDT", "৳"},
            {"BGN", "лв."},
            {"BHD", "د.ب.‏"},
            {"BND", "$"},
            {"BOB", "Bs"},
            {"BRL", "R$"},
            {"BTN", "Nu."},
            {"BWP", "P"},
            {"BYN", "Br"},
            {"BZD", "$"},
            {"CAD", "$"},
            {"CDF", "FC"},
            {"CHF", "CHF"},
            {"CLP", "$"},
            {"CNY", "¥"},
            {"COP", "$"},
            {"CRC", "₡"},
            {"CUP", "$"},
            {"CZK", "Kč"},
            {"DKK", "kr."},
            {"DOP", "$"},
            {"DZD", "د.ج.‏"},
            {"EGP", "ج.م.‏"},
            {"ERN", "Nfk"},
            {"ETB", "Br"},
            {"EUR", "€"},
            {"GBP", "£"},
            {"GEL", "₾"},
            {"GTQ", "Q"},
            {"HKD", "$"},
            {"HNL", "L"},
            {"HRK", "kn"},
            {"HTG", "G"},
            {"HUF", "Ft"},
            {"IDR", "Rp"},
            {"ILS", "₪"},
            {"INR", "₹"},
            {"IQD", "د.ع.‏"},
            {"IRR", "ريال"},
            {"ISK", "ISK"},
            {"JMD", "$"},
            {"JOD", "د.ا.‏"},
            {"JPY", "¥"},
            {"KES", "Ksh"},
            {"KGS", "сом"},
            {"KHR", "៛"},
            {"KRW", "₩"},
            {"KWD", "د.ك.‏"},
            {"KZT", "₸"},
            {"LAK", "₭"},
            {"LBP", "ل.ل.‏"},
            {"LKR", "රු."},
            {"LYD", "د.ل.‏"},
            {"MAD", "د.م.‏"},
            {"MDL", "L"},
            {"MKD", "ден"},
            {"MMK", "K"},
            {"MNT", "₮"},
            {"MOP", "MOP"},
            {"MVR", "ރ."},
            {"MXN", "$"},
            {"MYR", "RM"},
            {"NGN", "₦"},
            {"NIO", "C$"},
            {"NOK", "kr"},
            {"NPR", "रु"},
            {"NZD", "$"},
            {"OMR", "ر.ع.‏"},
            {"PAB", "B/."},
            {"PEN", "S/"},
            {"PHP", "₱"},
            {"PKR", "Rs"},
            {"PLN", "zł"},
            {"PYG", "₲"},
            {"QAR", "ر.ق.‏"},
            {"RON", "lei"},
            {"RSD", "дин."},
            {"RUB", "₽"},
            {"RWF", "RF"},
            {"SAR", "ر.س.‏"},
            {"SEK", "kr"},
            {"SGD", "$"},
            {"SOS", "S"},
            {"SYP", "ل.س.‏"},
            {"THB", "฿"},
            {"TJS", "смн"},
            {"TMT", "m."},
            {"TND", "د.ت.‏"},
            {"TRY", "₺"},
            {"TTD", "$"},
            {"TWD", "NT$"},
            {"UAH", "₴"},
            {"USD", "$"},
            {"UYU", "$"},
            {"UZS", "сўм"},
            {"VEF", "Bs."},
            {"VND", "₫"},
            {"XAF", "FCFA"},
            {"XCD", "EC$"},
            {"XDR", "XDR"},
            {"XOF", "CFA"},
            {"YER", "ر.ي.‏"},
            {"ZAR", "R"}
        };
    }
}