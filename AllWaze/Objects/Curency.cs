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
            {"AED", "ar-AE"},
            {"AFN", "prs-AF"},
            {"ALL", "sq-AL"},
            {"AMD", "hy-AM"},
            {"ARS", "es-AR"},
            {"AUD", "en-AU"},
            {"AZN", "az-Cyrl-AZ"},
            {"BAM", "bs-Cyrl-BA"},
            {"BDT", "bn-BD"},
            {"BGN", "bg-BG"},
            {"BHD", "ar-BH"},
            {"BND", "ms-BN"},
            {"BOB", "es-BO"},
            {"BRL", "es-BR"},
            {"BTN", "dz-BT"},
            {"BWP", "en-BW"},
            {"BYN", "be-BY"},
            {"BZD", "en-BZ"},
            {"CAD", "en-CA"},
            {"CDF", "fr-CD"},
            {"CHF", "de-CH"},
            {"CLP", "arn-CL"},
            {"CNY", "bo-CN"},
            {"COP", "es-CO"},
            {"CRC", "es-CR"},
            {"CUP", "es-CU"},
            {"CZK", "cs-CZ"},
            {"DKK", "da-DK"},
            {"DOP", "es-DO"},
            {"DZD", "ar-DZ"},
            {"EGP", "ar-EG"},
            {"ERN", "aa-ER"},
            {"ETB", "aa-ET"},
            {"EUR", "br-FR"},
            {"GBP", "cy-GB"},
            {"GEL", "ka-GE"},
            {"GTQ", "es-GT"},
            {"HKD", "en-HK"},
            {"HNL", "es-HN"},
            {"HRK", "hr-HR"},
            {"HTG", "fr-HT"},
            {"HUF", "hu-HU"},
            {"IDR", "en-ID"},
            {"ILS", "ar-IL"},
            {"INR", "as-IN"},
            {"IQD", "ar-IQ"},
            {"IRR", "fa-IR"},
            {"ISK", "is-IS"},
            {"JMD", "en-JM"},
            {"JOD", "ar-JO"},
            {"JPY", "ja-JP"},
            {"KES", "dav-KE"},
            {"KGS", "ky-KG"},
            {"KHR", "km-KH"},
            {"KRW", "ko-KR"},
            {"KWD", "ar-KW"},
            {"KZT", "kk-KZ"},
            {"LAK", "lo-LA"},
            {"LBP", "ar-LB"},
            {"LKR", "si-LK"},
            {"LYD", "ar-LY"},
            {"MAD", "ar-MA"},
            {"MDL", "ro-MD"},
            {"MKD", "mk-MK"},
            {"MMK", "my-MM"},
            {"MNT", "mn-MN"},
            {"MOP", "en-MO"},
            {"MVR", "dv-MV"},
            {"MXN", "es-MX"},
            {"MYR", "en-MY"},
            {"NGN", "bin-NG"},
            {"NIO", "es-NI"},
            {"NOK", "nb-NO"},
            {"NPR", "ne-NP"},
            {"NZD", "en-NZ"},
            {"OMR", "ar-OM"},
            {"PAB", "es-PA"},
            {"PEN", "es-PE"},
            {"PHP", "en-PH"},
            {"PKR", "en-PK"},
            {"PLN", "pl-PL"},
            {"PYG", "es-PY"},
            {"QAR", "ar-QA"},
            {"RON", "ro-RO"},
            {"RSD", "sr-Cyrl-RS"},
            {"RUB", "ba-RU"},
            {"RWF", "en-RW"},
            {"SAR", "ar-SA"},
            {"SEK", "en-SE"},
            {"SGD", "en-SG"},
            {"SOS", "ar-SO"},
            {"SYP", "ar-SY"},
            {"THB", "th-TH"},
            {"TJS", "tg-Cyrl-TJ"},
            {"TMT", "tk-TM"},
            {"TND", "ar-TN"},
            {"TRY", "tr-TR"},
            {"TTD", "en-TT"},
            {"TWD", "zh-TW"},
            {"UAH", "ru-UA"},
            {"USD", "chr-Cher-US"},
            {"UYU", "es-UY"},
            {"UZS", "uz-Cyrl-UZ"},
            {"VEF", "es-VE"},
            {"VND", "vi-VN"},
            {"XAF", "agq-CM"},
            {"XCD", "en-029"},
            {"XDR", "es-419"},
            {"XOF", "dyo-SN"},
            {"YER", "ar-YE"},
            {"ZAR", "af-ZA"}
        };

        public static Dictionary<string, string> CurrencyNames = new Dictionary<string, string>
        {
            {"AED", "United Arab Emirates Dirham"},
            {"AFN", "Afghanistan Afghani"},
            {"ALL", "Albania Lek"},
            {"AMD", "Armenia Dram"},
            {"ANG", "Netherlands Antilles Guilder"},
            {"AOA", "Angola Kwanza"},
            {"ARS", "Argentina Peso"},
            {"AUD", "Australia Dollar"},
            {"AWG", "Aruba Guilder"},
            {"AZN", "Azerbaijan New Manat"},
            {"BAM", "Bosnia and Herzegovina Convertible Marka"},
            {"BBD", "Barbados Dollar"},
            {"BDT", "Bangladesh Taka"},
            {"BGN", "Bulgaria Lev"},
            {"BHD", "Bahrain Dinar"},
            {"BIF", "Burundi Franc"},
            {"BMD", "Bermuda Dollar"},
            {"BND", "Brunei Darussalam Dollar"},
            {"BOB", "Bolivia Bolíviano"},
            {"BRL", "Brazil Real"},
            {"BSD", "Bahamas Dollar"},
            {"BTN", "Bhutan Ngultrum"},
            {"BWP", "Botswana Pula"},
            {"BYN", "Belarus Ruble"},
            {"BZD", "Belize Dollar"},
            {"CAD", "Canada Dollar"},
            {"CDF", "Congo/Kinshasa Franc"},
            {"CHF", "Switzerland Franc"},
            {"CLP", "Chile Peso"},
            {"CNY", "China Yuan Renminbi"},
            {"COP", "Colombia Peso"},
            {"CRC", "Costa Rica Colon"},
            {"CUC", "Cuba Convertible Peso"},
            {"CUP", "Cuba Peso"},
            {"CVE", "Cape Verde Escudo"},
            {"CZK", "Czech Republic Koruna"},
            {"DJF", "Djibouti Franc"},
            {"DKK", "Denmark Krone"},
            {"DOP", "Dominican Republic Peso"},
            {"DZD", "Algeria Dinar"},
            {"EGP", "Egypt Pound"},
            {"ERN", "Eritrea Nakfa"},
            {"ETB", "Ethiopia Birr"},
            {"EUR", "Euro Member Countries"},
            {"FJD", "Fiji Dollar"},
            {"FKP", "Falkland Islands (Malvinas) Pound"},
            {"GBP", "United Kingdom Pound"},
            {"GEL", "Georgia Lari"},
            {"GGP", "Guernsey Pound"},
            {"GHS", "Ghana Cedi"},
            {"GIP", "Gibraltar Pound"},
            {"GMD", "Gambia Dalasi"},
            {"GNF", "Guinea Franc"},
            {"GTQ", "Guatemala Quetzal"},
            {"GYD", "Guyana Dollar"},
            {"HKD", "Hong Kong Dollar"},
            {"HNL", "Honduras Lempira"},
            {"HRK", "Croatia Kuna"},
            {"HTG", "Haiti Gourde"},
            {"HUF", "Hungary Forint"},
            {"IDR", "Indonesia Rupiah"},
            {"ILS", "Israel Shekel"},
            {"IMP", "Isle of Man Pound"},
            {"INR", "India Rupee"},
            {"IQD", "Iraq Dinar"},
            {"IRR", "Iran Rial"},
            {"ISK", "Iceland Krona"},
            {"JEP", "Jersey Pound"},
            {"JMD", "Jamaica Dollar"},
            {"JOD", "Jordan Dinar"},
            {"JPY", "Japan Yen"},
            {"KES", "Kenya Shilling"},
            {"KGS", "Kyrgyzstan Som"},
            {"KHR", "Cambodia Riel"},
            {"KMF", "Comoros Franc"},
            {"KPW", "Korea (North) Won"},
            {"KRW", "Korea (South) Won"},
            {"KWD", "Kuwait Dinar"},
            {"KYD", "Cayman Islands Dollar"},
            {"KZT", "Kazakhstan Tenge"},
            {"LAK", "Laos Kip"},
            {"LBP", "Lebanon Pound"},
            {"LKR", "Sri Lanka Rupee"},
            {"LRD", "Liberia Dollar"},
            {"LSL", "Lesotho Loti"},
            {"LYD", "Libya Dinar"},
            {"MAD", "Morocco Dirham"},
            {"MDL", "Moldova Leu"},
            {"MGA", "Madagascar Ariary"},
            {"MKD", "Macedonia Denar"},
            {"MMK", "Myanmar (Burma) Kyat"},
            {"MNT", "Mongolia Tughrik"},
            {"MOP", "Macau Pataca"},
            {"MRO", "Mauritania Ouguiya"},
            {"MUR", "Mauritius Rupee"},
            {"MVR", "Maldives (Maldive Islands) Rufiyaa"},
            {"MWK", "Malawi Kwacha"},
            {"MXN", "Mexico Peso"},
            {"MYR", "Malaysia Ringgit"},
            {"MZN", "Mozambique Metical"},
            {"NAD", "Namibia Dollar"},
            {"NGN", "Nigeria Naira"},
            {"NIO", "Nicaragua Cordoba"},
            {"NOK", "Norway Krone"},
            {"NPR", "Nepal Rupee"},
            {"NZD", "New Zealand Dollar"},
            {"OMR", "Oman Rial"},
            {"PAB", "Panama Balboa"},
            {"PEN", "Peru Sol"},
            {"PGK", "Papua New Guinea Kina"},
            {"PHP", "Philippines Peso"},
            {"PKR", "Pakistan Rupee"},
            {"PLN", "Poland Zloty"},
            {"PYG", "Paraguay Guarani"},
            {"QAR", "Qatar Riyal"},
            {"RON", "Romania New Leu"},
            {"RSD", "Serbia Dinar"},
            {"RUB", "Russia Ruble"},
            {"RWF", "Rwanda Franc"},
            {"SAR", "Saudi Arabia Riyal"},
            {"SBD", "Solomon Islands Dollar"},
            {"SCR", "Seychelles Rupee"},
            {"SDG", "Sudan Pound"},
            {"SEK", "Sweden Krona"},
            {"SGD", "Singapore Dollar"},
            {"SHP", "Saint Helena Pound"},
            {"SLL", "Sierra Leone Leone"},
            {"SOS", "Somalia Shilling"},
            {"SPL*", "Seborga Luigino"},
            {"SRD", "Suriname Dollar"},
            {"STD", "São Tomé and Príncipe Dobra"},
            {"SVC", "El Salvador Colon"},
            {"SYP", "Syria Pound"},
            {"SZL", "Swaziland Lilangeni"},
            {"THB", "Thailand Baht"},
            {"TJS", "Tajikistan Somoni"},
            {"TMT", "Turkmenistan Manat"},
            {"TND", "Tunisia Dinar"},
            {"TOP", "Tonga Pa'anga"},
            {"TRY", "Turkey Lira"},
            {"TTD", "Trinidad and Tobago Dollar"},
            {"TVD", "Tuvalu Dollar"},
            {"TWD", "Taiwan New Dollar"},
            {"TZS", "Tanzania Shilling"},
            {"UAH", "Ukraine Hryvnia"},
            {"UGX", "Uganda Shilling"},
            {"USD", "United States Dollar"},
            {"UYU", "Uruguay Peso"},
            {"UZS", "Uzbekistan Som"},
            {"VEF", "Venezuela Bolivar"},
            {"VND", "Viet Nam Dong"},
            {"VUV", "Vanuatu Vatu"},
            {"WST", "Samoa Tala"},
            {"XAF", "Communauté Financière Africaine (BEAC) CFA Franc BEAC"},
            {"XCD", "East Caribbean Dollar"},
            {"XDR", "International Monetary Fund (IMF) Special Drawing Rights"},
            {"XOF", "Communauté Financière Africaine (BCEAO) Franc"},
            {"XPF", "Comptoirs Français du Pacifique (CFP) Franc"},
            {"YER", "Yemen Rial"},
            {"ZAR", "South Africa Rand"},
            {"ZMW", "Zambia Kwacha"},
            {"ZWD", "Zimbabwe Dollar"}
        };
    }
}