﻿(function (e) { "function" == typeof define && define.amd ? define(["jquery", "moment"], e) : e(jQuery, moment); })(function (e, t) {
    function n(e) { return 11 === e % 100 ? !0 : 1 === e % 10 ? !1 : !0; }

    function i(e, t, i, r) {
        var a = e + " ";
        switch (i) {
            case "s":
                return t || r ? "nokkrar sekúndur" : "nokkrum sekúndum";
            case "m":
                return t ? "mínúta" : "mínútu";
            case "mm":
                return n(e) ? a + (t || r ? "mínútur" : "mínútum") : t ? a + "mínúta" : a + "mínútu";
            case "hh":
                return n(e) ? a + (t || r ? "klukkustundir" : "klukkustundum") : a + "klukkustund";
            case "d":
                return t ? "dagur" : r ? "dag" : "degi";
            case "dd":
                return n(e) ? t ? a + "dagar" : a + (r ? "daga" : "dögum") : t ? a + "dagur" : a + (r ? "dag" : "degi");
            case "M":
                return t ? "mánuður" : r ? "mánuð" : "mánuði";
            case "MM":
                return n(e) ? t ? a + "mánuðir" : a + (r ? "mánuði" : "mánuðum") : t ? a + "mánuður" : a + (r ? "mánuð" : "mánuði");
            case "y":
                return t || r ? "ár" : "ári";
            case "yy":
                return n(e) ? a + (t || r ? "ár" : "árum") : a + (t || r ? "ár" : "ári");
        }
    }

    (t.defineLocale || t.lang).call(t, "is", { months: "janúar_febrúar_mars_apríl_maí_júní_júlí_ágúst_september_október_nóvember_desember".split("_"), monthsShort: "jan_feb_mar_apr_maí_jún_júl_ágú_sep_okt_nóv_des".split("_"), weekdays: "sunnudagur_mánudagur_þriðjudagur_miðvikudagur_fimmtudagur_föstudagur_laugardagur".split("_"), weekdaysShort: "sun_mán_þri_mið_fim_fös_lau".split("_"), weekdaysMin: "Su_Má_Þr_Mi_Fi_Fö_La".split("_"), longDateFormat: { LT: "H:mm", L: "DD/MM/YYYY", LL: "D. MMMM YYYY", LLL: "D. MMMM YYYY [kl.] LT", LLLL: "dddd, D. MMMM YYYY [kl.] LT" }, calendar: { sameDay: "[í dag kl.] LT", nextDay: "[á morgun kl.] LT", nextWeek: "dddd [kl.] LT", lastDay: "[í gær kl.] LT", lastWeek: "[síðasta] dddd [kl.] LT", sameElse: "L" }, relativeTime: { future: "eftir %s", past: "fyrir %s síðan", s: i, m: i, mm: i, h: "klukkustund", hh: i, d: i, dd: i, M: i, MM: i, y: i, yy: i }, ordinal: "%d.", week: { dow: 1, doy: 4 } }), e.fullCalendar.datepickerLang("is", "is", { closeText: "Loka", prevText: "&#x3C; Fyrri", nextText: "Næsti &#x3E;", currentText: "Í dag", monthNames: ["Janúar", "Febrúar", "Mars", "Apríl", "Maí", "Júní", "Júlí", "Ágúst", "September", "Október", "Nóvember", "Desember"], monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "Maí", "Jún", "Júl", "Ágú", "Sep", "Okt", "Nóv", "Des"], dayNames: ["Sunnudagur", "Mánudagur", "Þriðjudagur", "Miðvikudagur", "Fimmtudagur", "Föstudagur", "Laugardagur"], dayNamesShort: ["Sun", "Mán", "Þri", "Mið", "Fim", "Fös", "Lau"], dayNamesMin: ["Su", "Má", "Þr", "Mi", "Fi", "Fö", "La"], weekHeader: "Vika", dateFormat: "dd.mm.yy", firstDay: 0, isRTL: !1, showMonthAfterYear: !1, yearSuffix: "" }), e.fullCalendar.lang("is", { defaultButtonText: { month: "Mánuður", week: "Vika", day: "Dagur", list: "Dagskrá" }, allDayHtml: "Allan<br/>daginn", eventLimitText: "meira" });
});