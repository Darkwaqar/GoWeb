/*
 * tagContainer Generator v5
 * Copyright Tag Commander
 * http://www.tagcommander.com/
 * Generated: 2016-04-12 10:15:42 Europe/Paris
 * ---
 * Version	: 3.02
 * IDTC 	: 5
 * IDS		: 2496
 */
/*!compressed by YUI*/
if (typeof tC == "undefined") {
    if (typeof document.domain == "undefined" || typeof document.referrer == "undefined") {
        document = window.document
    }
    (function (m, k) {
        var j, r, y = m.document, a = m.location, e = m.navigator, x = m.tC, v = m.$, H = Array.prototype.push, b = Array.prototype.slice, u = Array.prototype.indexOf, g = Object.prototype.toString, i = Object.prototype.hasOwnProperty, o = String.prototype.trim, c = function (J, K) {
            return new c.fn.init(J, K, j)
        }, B = /[\-+]?(?:\d*\.|)\d+(?:[eE][\-+]?\d+|)/.source, q = /\S/, t = /\s+/, d = /^[\s\uFEFF\xA0]+|[\s\uFEFF\xA0]+$/g, w = /^(?:[^#<]*(<[\w\W]+>)[^>]*$|#([\w\-]*)$)/, l = /^<(\w+)\s*\/?>(?:<\/\1>|)$/, D = /^[\],:{}\s]*$/, z = /(?:^|:|,)(?:\s*\[)+/g, G = /\\(?:["\\\/bfnrt]|u[\da-fA-F]{4})/g, E = /"[^"\\\r\n]*"|true|false|null|-?(?:\d\d*\.|)\d+(?:[eE][\-+]?\d+|)/g, I = /^-ms-/, p = /-([\da-z])/gi, F = function (J, K) {
            return (K + "").toUpperCase()
        }, f = {};
        c.fn = c.prototype = {
            constructor: c, init: function (J, M, P) {
                var L, N, K, O;
                if (!J) {
                    return this
                }
                if (J.nodeType) {
                    this.context = this[0] = J;
                    this.length = 1;
                    return this
                }
                if (typeof J === "string") {
                    if (J.charAt(0) === "<" && J.charAt(J.length - 1) === ">" && J.length >= 3) {
                        L = [null, J, null]
                    } else {
                        L = w.exec(J)
                    }
                    if (L && (L[1] || !M)) {
                        if (L[1]) {
                            M = M instanceof c ? M[0] : M;
                            O = (M && M.nodeType ? M.ownerDocument || M : y);
                            J = c.parseHTML(L[1], O, true);
                            if (l.test(L[1]) && c.isPlainObject(M)) {
                                this.attr.call(J, M, true)
                            }
                            return c.merge(this, J)
                        } else {
                            N = y.getElementById(L[2]);
                            if (N && N.parentNode) {
                                if (N.id !== L[2]) {
                                    return P.find(J)
                                }
                                this.length = 1;
                                this[0] = N
                            }
                            this.context = y;
                            this.selector = J;
                            return this
                        }
                    } else {
                        if (!M || M.tC) {
                            return (M || P).find(J)
                        } else {
                            return this.constructor(M).find(J)
                        }
                    }
                } else {
                    if (c.isFunction(J)) {
                        return P.ready(J)
                    }
                }
                if (J.selector !== k) {
                    this.selector = J.selector;
                    this.context = J.context
                }
                return c.makeArray(J, this)
            }, each: function (K, J) {
                return c.each(this, K, J)
            }, ready: function (J) {
                c.ready.promise(J);
                return this
            }
        };
        c.fn.init.prototype = c.fn;
        c.extend = c.fn.extend = function () {
            var S, L, J, K, P, Q, O = arguments[0] || {}, N = 1, M = arguments.length, R = false;
            if (typeof O === "boolean") {
                R = O;
                O = arguments[1] || {};
                N = 2
            }
            if (typeof O !== "object" && !c.isFunction(O)) {
                O = {}
            }
            if (M === N) {
                O = this;
                --N
            }
            for (; N < M; N++) {
                if ((S = arguments[N]) != null) {
                    for (L in S) {
                        J = O[L];
                        K = S[L];
                        if (O === K) {
                            continue
                        }
                        if (R && K && (c.isPlainObject(K) || (P = c.isArray(K)))) {
                            if (P) {
                                P = false;
                                Q = J && c.isArray(J) ? J : []
                            } else {
                                Q = J && c.isPlainObject(J) ? J : {}
                            }
                            O[L] = c.extend(R, Q, K)
                        } else {
                            if (K !== k) {
                                O[L] = K
                            }
                        }
                    }
                }
            }
            return O
        };
        c.extend({
            ssl: (("https:" == y.location.protocol) ? "https://manager." : "http://redirect2496."),
            randOrd: function () {
                return (Math.round(Math.random()) - 0.5)
            },
            nodeNames: "abbr|article|aside|audio|bdi|canvas|data|datalist|details|figcaption|figure|footer|header|hgroup|mark|meter|nav|output|progress|section|summary|time|video",
            rnocache: /<(?:script|object|embed|option|style)/i,
            rnoshimcache: new RegExp("<(?:" + c.nodeNames + ")[\\s/>]", "i"),
            rchecked: /checked\s*(?:[^=]|=\s*.checked.)/i,
            containersLaunched: {}
        });
        c.extend({
            inArray: function (N, K, M) {
                var J, L = Array.prototype.indexOf;
                if (K) {
                    if (L) {
                        return L.call(K, N, M)
                    }
                    J = K.length;
                    M = M ? M < 0 ? Math.max(0, J + M) : M : 0;
                    for (; M < J; M++) {
                        if (M in K && K[M] === N) {
                            return M
                        }
                    }
                }
                return -1
            }, isFunction: function (J) {
                return c.type(J) === "function"
            }, isArray: Array.isArray || function (J) {
                return c.type(J) === "array"
            }, isWindow: function (J) {
                return J != null && J == J.window
            }, isNumeric: function (J) {
                return !isNaN(parseFloat(J)) && isFinite(J)
            }, type: function (J) {
                return J == null ? String(J) : f[g.call(J)] || "object"
            }, each: function (O, P, L) {
                var K, M = 0, N = O.length, J = N === k || c.isFunction(O);
                if (L) {
                    if (J) {
                        for (K in O) {
                            if (P.apply(O[K], L) === false) {
                                break
                            }
                        }
                    } else {
                        for (; M < N;) {
                            if (P.apply(O[M++], L) === false) {
                                break
                            }
                        }
                    }
                } else {
                    if (J) {
                        for (K in O) {
                            if (P.call(O[K], K, O[K]) === false) {
                                break
                            }
                        }
                    } else {
                        for (; M < N;) {
                            if (P.call(O[M], M, O[M++]) === false) {
                                break
                            }
                        }
                    }
                }
                return O
            }, log: function (J, K) {
                try {
                    if (c.getCookie("tCdebugLib") && console) {
                        console[K ? K : "log"](J)
                    }
                } catch (L) {
                }
            }
        });
        c.each("Boolean Number String Function Array Date RegExp Object".split(" "), function (K, J) {
            f["[object " + J + "]"] = J.toLowerCase()
        });
        j = c(y);
        var h = {};

        function C(K) {
            var J = h[K] = {};
            c.each(K.split(t), function (M, L) {
                J[L] = true
            });
            return J
        }

        c.buildFragment = function (M, N, K) {
            var L, J, O, P = M[0];
            N = N || y;
            N = !N.nodeType && N[0] || N;
            N = N.ownerDocument || N;
            if (M.length === 1 && typeof P === "string" && P.length < 512 && N === y && P.charAt(0) === "<" && !c.rnocache.test(P) && (c.support.checkClone || !c.rchecked.test(P)) && (c.support.html5Clone || !c.rnoshimcache.test(P))) {
                J = true;
                L = jQuery.fragments[P];
                O = L !== k
            }
            if (!L) {
                L = N.createDocumentFragment();
                c.clean(M, N, L, K);
                if (J) {
                    c.fragments[P] = O && L
                }
            }
            return {fragment: L, cacheable: J}
        };
        var s = a.hostname, n = s.split("."), A = "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]).){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
        if (n.length < 2 || s.match(A)) {
            c.maindomain = s
        } else {
            c.maindomain = n[n.length - 2] + "." + n[n.length - 1]
        }
        m.tC = c
    })(window)
}
tC.extend({
    internalvars: typeof tC.internalvars != "undefined" ? tC.internalvars : {},
    internalFunctions: typeof tC.internalFunctions != "undefined" ? tC.internalFunctions : {},
    privacyVersion: "",
    containerVersion: "3.02",
    id_container: "5",
    id_site: "2496",
    generatorVersion: "1.0.0",
    dedup_done: typeof tC.dedup_done != "undefined" ? tC.dedup_done : false
});
tC.extend({
    launchTag: function (e, c, d, a, b) {
        tC.array_launched_tags.push(c);
        tC.array_launched_tags_keys.push(e);
        tC.containersLaunched[a][b].t.push({id: e, label: c, idTpl: d});
        window.postMessage('TC.EX:{"id":"' + e + '","idc":"' + b + '","idt":"' + d + '","ids":"' + a + '","lb":"' + c.replace(/"/g, '\\"') + '"}', "*")
    }
});
if (tC.containersLaunched === undefined) {
    tC.containersLaunched = {}
}
if (tC.containersLaunched[2496] === undefined) {
    tC.containersLaunched[2496] = {}
}
tC.containersLaunched[2496][5] = {v: "3.02", t: []};
tC.extend({
    domReady: false, isDOMReady: function () {
        if (document.readyState == "complete" || document.readyState == "loaded") {
            return true
        }
        if (document.readyState != "interactive") {
            return false
        }
        if (!document.documentElement.doScroll) {
            return true
        }
        try {
            document.documentElement.doScroll("left");
            return true
        } catch (a) {
            return false
        }
    }, waitingOnDomReadyCallBacks: tC.waitingOnDomReadyCallBacks || [], excuteOnDomReadyCallBacks: function () {
        for (var a = 0; a < tC.waitingOnDomReadyCallBacks.length; a++) {
            tC.waitingOnDomReadyCallBacks[a]()
        }
        tC.waitingOnDomReadyCallBacks = []
    }, onDomReady: function (b) {
        if (this.domReady) {
            b();
            return
        }
        tC.waitingOnDomReadyCallBacks.push(b);
        var a = false;
        if (document.addEventListener) {
            a = true;
            document.addEventListener("DOMContentLoaded", function () {
                document.removeEventListener("DOMContentLoaded", arguments.callee, false);
                tC.excuteOnDomReadyCallBacks()
            }, false)
        } else {
            if (document.attachEvent) {
                a = true;
                document.attachEvent("onreadystatechange", function () {
                    if (document.readyState === "complete") {
                        document.detachEvent("onreadystatechange", arguments.callee);
                        tC.excuteOnDomReadyCallBacks()
                    }
                });
                if (document.documentElement.doScroll && window == window.top) {
                    (function () {
                        if (tC.domReady) {
                            return
                        }
                        try {
                            document.documentElement.doScroll("left")
                        } catch (c) {
                            setTimeout(arguments.callee, 0);
                            return
                        }
                        tC.excuteOnDomReadyCallBacks()
                    })()
                }
            }
        }
        if (!a) {
            window.onload = tC.excuteOnDomReadyCallBacks
        }
    }
});
if (tC.isDOMReady()) {
    tC.domReady = true
} else {
    tC.onDomReady(function () {
        tC.domReady = true
    })
}
tC.extend({
    isCurrentVersion: function () {
        var a = tC.getCookie("tc_mode_test"), b = "testModeIncludeReplaceThisByTrue";
        return a != "1" || (a == "1" && b == "true")
    }
});
tC.extend({
    pixelTrack: {
        add: function (a, b) {
            a = a || 0;
            b = b || "img";
            tC.onDomReady(function () {
                if (b == "iframe") {
                    var c = document.createElement(b);
                    c.src = a;
                    c.width = 1;
                    c.height = 1;
                    c.style.display = "none";
                    document.body.appendChild(c)
                } else {
                    var c = new Image();
                    c.src = a
                }
            })
        }
    }
});
tC.extend({
    tc_hdoc: false, domain: function () {
        this.tc_hdoc = document;
        try {
            try {
                this.tc_hdoc = top.document
            } catch (d) {
                this.tc_hdoc = document
            }
            var a = typeof this.tc_hdoc.domain != "undefined" ? this.tc_hdoc.domain.toLowerCase().split(".") : false, g = "^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5]).){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$";
            if (a.length < 2 || this.tc_hdoc.domain.match(g)) {
                return ""
            } else {
                var f = a[a.length - 3], c = a[a.length - 2], b = a[a.length - 1];
                if (c == "co" || c == "com") {
                    return "." + f + "." + c + "." + b
                } else {
                    return "." + c + "." + b
                }
            }
        } catch (d) {
            tC.log(["tC.domain error : ", d], "warn")
        }
    }
});
tC.domain();
tC.extend({
    removeCookie: function (a) {
        this.setCookie(a, "", -1)
    }, setCookie: function (c, e, a, h, d, g) {
        if (!d) {
            d = tC.domain()
        }
        var b = new Date();
        b.setTime(b.getTime());
        if (a) {
            a = a * 1000 * 60 * 60 * 24
        }
        var f = new Date(b.getTime() + (a));
        document.cookie = c + "=" + escape(e) + ((a) ? ";expires=" + f.toGMTString() : "") + ((h) ? ";path=" + h : ";path=/") + ((d) ? ";domain=" + d : "") + ((g) ? ";secure" : "")
    }, getCookie: function (a) {
        return (result = new RegExp("(?:^|; )" + encodeURIComponent(a) + "=([^;]*)").exec(document.cookie)) ? unescape(result[1]) : ""
    }
});
tC.extend({
    storage: {
        has: function () {
            try {
                if ("localStorage" in window && window.localStorage !== null) {
                    window.localStorage.setItem("TC_CHECK", "1");
                    window.localStorage.removeItem("TC_CHECK");
                    return true
                }
                return false
            } catch (a) {
                return false
            }
        }, get: function (a) {
            return this.has() ? window.localStorage.getItem(a) : false
        }, set: function (b, a) {
            return this.has() ? (window.localStorage.setItem(b, a) || true) : false
        }, remove: function (a) {
            return this.has() ? (window.localStorage.removeItem(a) || true) : false
        }
    }
});
tC.extend({
    _R: {
        cR: function (a) {
            tC.storage.set("tC_Sync", a);
            tC.pixelTrack.add("//engage.commander1.com/reach?tc_s=2496")
        }, rR: function () {
            if (tC.storage.has()) {
                var a = new Date().getTime();
                var b = tC.storage.get("tC_Sync") || 0;
                b = parseInt(b);
                if (b == 0 || a - b > 604800000) {
                    this.cR(a)
                }
            }
        }
    }
});
tC.onDomReady(function () {
    tC._R.rR()
});
tC.extend({
    hitCounter: function () {
        if (Math.floor(Math.random() * parseInt(20)) == 0) {
            tC.pixelTrack.add("//manager.tagcommander.com/utils/hit.php?id=5&site=2496&version=3.02&frequency=20&position=" + tC.container_position + "&rand=" + Math.random())
        }
    }
});
tC.container_position = (typeof tc_container_position !== "undefined") ? tc_container_position : (typeof tC.container_position !== "undefined") ? tC.container_position : 0;
tC.container_position++;
if (typeof tc_container_position !== "undefined") {
    tc_container_position++
}
tC.hitCounter();
tC.extend({
    script: {
        add: function (d, f, c) {
            var a = (document.getElementsByTagName("body")[0] || document.getElementsByTagName("script")[0].parentNode), b = document.createElement("script");
            b.type = "text/javascript";
            b.async = true;
            b.src = d;
            b.charset = "utf-8";
            if (a) {
                if (f) {
                    if (b.addEventListener) {
                        b.addEventListener("load", function () {
                            f()
                        }, false)
                    } else {
                        b.onreadystatechange = function () {
                            if (b.readyState in {loaded: 1, complete: 1}) {
                                b.onreadystatechange = null;
                                f()
                            }
                        }
                    }
                }
                if (c && typeof c == "number") {
                    setTimeout(function () {
                        if (a && b.parentNode) {
                            a.removeChild(b)
                        }
                    }, c)
                }
                a.insertBefore(b, a.firstChild)
            } else {
                tC.log("tC.script error : the element <script> or <body> is not found ! the file " + d + " is not implemented !", "warn")
            }
        }
    }
});
tC.extend({
    match: function (a, d, b) {
        try {
            return (a.match(new RegExp(d, b)))
        } catch (c) {
            tC.log("the tC.match error ! message : " + c.message, "data : " + a, "p : " + d, "flag : " + b, "warn")
        }
    }
});
tC2496_5 = tC;
/* RETRO COMPATIBILITY FUNCTIONS */


if (typeof tc_vars == 'undefined')var tc_vars = [];
(function () {
    var l = 'env_template|env_work|env_channel|env_version|nav_division|nav_storic_countryCode|nav_countryCode|nav_languageCode|nav_currency|nav_dept|nav_pagetype|nav_section|nav_subsection|nav_section_dept|nav_subsection_dept|nav_page|nav_historicalTP|nav_sessionTP|sr_items|sr_color|sr_size|sr_pricerange|sr_searchoptions|sr_textsearch|product_cod8|product_cod10|product_unitprice_ati|product_discount_ati|product_discount_tf|product_unitprice_tf|user_Id|user_logged|user_totalorders|user_type|user_email|cart_ispickupinstoreselected|cart_PaymentType|cart_itemsNo|cart_items|cart_Id|cart_total|cart_subtotal|order_DeliveryType|order_PaymentType|order_id|order_itemsNo|order_promotion|order_items|order_amount_ati_without_sf|order_amount_ati_with_sf|order_amount_tf_without_sf|order_amount_tf_with_sf|order_shipping'.split('|');
    for (var k in l) {
        if (!tc_vars.hasOwnProperty(l[k])) {
            tc_vars[l[k]] = '';
        }
    }
})();

/*DYNAMIC JS BLOCK 1*/


/*END DYNAMIC JS BLOCK 1*/

/*CUSTOM_JS_BLOCK1*/

/*END_CUSTOM_JS_BLOCK1*/
tC.array_launched_tags = [];
tC.array_launched_tags_keys = [];
tC.id_site = '2496';
if (tC.getCookie('tc_mode_test') == 1) {
    (function () {
        var tc_testmodescriptload = document.createElement('script');
        tc_testmodescriptload.type = 'text/javascript';
        tc_testmodescriptload.src = '//manager.tagcommander.com/utils/test_mode_include.php?id=5&site=2496&type=load&rand=' + Math.random() + '&version=';
        (document.getElementsByTagName('body')[0] || document.getElementsByTagName('head')[0] || document.getElementsByTagName('script')[0].parentNode).appendChild(tc_testmodescriptload);
    })();
} else {
    /*VARIABLES_BLOCK*/

    tC.internalvars.tc_fulldomain = window.location.hostname;
    if (tc_vars["nav_sessionTP"] !== "") {
        tC.internalvars.tpcode = tc_vars["nav_sessionTP"];
    }
    else {
        tC.internalvars.tpcode = tc_vars["nav_historicalTP"];
    }
    tC.internalvars.cookie_affiliation = '';
    function getCookie(c_name) {
        var i, x, y, ARRcookies = document.cookie.split(";");
        for (i = 0; i < ARRcookies.length; i++) {
            x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
            y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
            x = x.replace(/^\s+|\s+$/g, "");
            if (x == c_name) {
                return unescape(y);
            }
        }
    }

    tC.internalvars.cookieTmp = getCookie("AFFILIATION");
    if (tC.internalvars.cookieTmp !== undefined) {
        tC.internalvars.cookieTmp1 = tC.internalvars.cookieTmp.split("=");
        tC.internalvars.cookie_affiliation = tC.internalvars.cookieTmp1[1];
    }
    if (tC.match(String(tC.getCookie("PARTNER")), 'criteo', "") || tC.match(String(tC.getCookie("PARTNER")), 'Criteo', "") || tC.match(String(tC.getCookie("PARTNER")), 'CRITEO', "")) {
        tC.internalvars.criteodedup = "1";
    }
    else {
        tC.internalvars.criteodedup = "0";
    }
    if (tc_vars["env_channel"] == "desktop") {
        tC.internalvars.criteochannel = "d";
    }
    if (tc_vars["env_channel"] == "mobile") {
        tC.internalvars.criteochannel = "m";
    }
    if (tc_vars["env_channel"] == "tablet") {
        tC.internalvars.criteochannel = "t";
    }
    tC.internalvars.ls_siteId = "";
    if (typeof getCookie("AFFILIATION") !== "undefined") {
        var tc_cookieValue = getCookie("AFFILIATION");
        if (tc_cookieValue.indexOf("SITEID") > -1) {
            tC.internalvars.ls_siteId = tc_cookieValue.substr(tc_cookieValue.indexOf("SITEID=") + 7, 34);
        }
    }
    tC.internalvars.ls_landDate = "";
    if (typeof getCookie("AFFILIATION") !== "undefined") {
        var tc_cookieValue = getCookie("AFFILIATION");
        if (tc_cookieValue.indexOf("SITEID_DATE") > -1) {
            var tc_first_pos = tc_cookieValue.indexOf("#", tc_cookieValue.indexOf("SITEID_DATE="));
            var tc_second_pos = tc_cookieValue.indexOf("#", tc_first_pos + 1);
            var tc_full_string = tc_cookieValue.substr(tc_first_pos + 1, (tc_second_pos - tc_first_pos - 1));
            var tc_split_slash = tc_full_string.split("/");
            var tc_split_plus = tc_split_slash[2].split("+");
            var tc_split_colon = tc_split_plus[1].split(":");
            var tc_hour = new Date("1/1/2000 " + tc_split_colon[0] + ":00:00 " + tc_split_plus[2]).getHours();
            if (tc_hour.toString().length === 1) {
                tc_hour = "0" + tc_hour;
            }
            tC.internalvars.ls_landDate = tc_split_plus[0] + tc_split_slash[0] + tc_split_slash[1] + "_" + tc_hour + tc_split_colon[1];
            tC.log("ls_landDate:" + tC.internalvars.ls_landDate);
        }
    }
    var tc_internalvars_countries = '';
    switch (tc_vars["nav_countryCode"].toString().toLowerCase()) {
        case"ca":
            tc_internalvars_countries = "NA";
            break;
        case"do":
            tc_internalvars_countries = "NA";
            break;
        case"gt":
            tc_internalvars_countries = "NA";
            break;
        case"mx":
            tc_internalvars_countries = "NA";
            break;
        case"pa":
            tc_internalvars_countries = "NA";
            break;
        case"us":
            tc_internalvars_countries = "NA";
            break;
        case"am":
            tc_internalvars_countries = "APAC";
            break;
        case"au":
            tc_internalvars_countries = "APAC";
            break;
        case"az":
            tc_internalvars_countries = "APAC";
            break;
        case"bh":
            tc_internalvars_countries = "APAC";
            break;
        case"bn":
            tc_internalvars_countries = "APAC";
            break;
        case"cn":
            tc_internalvars_countries = "APAC";
            break;
        case"cy":
            tc_internalvars_countries = "APAC";
            break;
        case"ge":
            tc_internalvars_countries = "APAC";
            break;
        case"hk":
            tc_internalvars_countries = "APAC";
            break;
        case"in":
            tc_internalvars_countries = "APAC";
            break;
        case"id":
            tc_internalvars_countries = "APAC";
            break;
        case"il":
            tc_internalvars_countries = "APAC";
            break;
        case"jp":
            tc_internalvars_countries = "APAC";
            break;
        case"jo":
            tc_internalvars_countries = "APAC";
            break;
        case"kz":
            tc_internalvars_countries = "APAC";
            break;
        case"kw":
            tc_internalvars_countries = "APAC";
            break;
        case"kg":
            tc_internalvars_countries = "APAC";
            break;
        case"lb":
            tc_internalvars_countries = "APAC";
            break;
        case"mo":
            tc_internalvars_countries = "APAC";
            break;
        case"my":
            tc_internalvars_countries = "APAC";
            break;
        case"nz":
            tc_internalvars_countries = "APAC";
            break;
        case"om":
            tc_internalvars_countries = "APAC";
            break;
        case"ph":
            tc_internalvars_countries = "APAC";
            break;
        case"qa":
            tc_internalvars_countries = "APAC";
            break;
        case"sa":
            tc_internalvars_countries = "APAC";
            break;
        case"sg":
            tc_internalvars_countries = "APAC";
            break;
        case"kr":
            tc_internalvars_countries = "APAC";
            break;
        case"sy":
            tc_internalvars_countries = "APAC";
            break;
        case"tw":
            tc_internalvars_countries = "APAC";
            break;
        case"tj":
            tc_internalvars_countries = "APAC";
            break;
        case"th":
            tc_internalvars_countries = "APAC";
            break;
        case"tm":
            tc_internalvars_countries = "APAC";
            break;
        case"ae":
            tc_internalvars_countries = "APAC";
            break;
        case"uz":
            tc_internalvars_countries = "APAC";
            break;
        case"vn":
            tc_internalvars_countries = "APAC";
            break;
        case"al":
            tc_internalvars_countries = "EU";
            break;
        case"ad":
            tc_internalvars_countries = "EU";
            break;
        case"at":
            tc_internalvars_countries = "EU";
            break;
        case"by":
            tc_internalvars_countries = "EU";
            break;
        case"be":
            tc_internalvars_countries = "EU";
            break;
        case"ba":
            tc_internalvars_countries = "EU";
            break;
        case"bg":
            tc_internalvars_countries = "EU";
            break;
        case"hr":
            tc_internalvars_countries = "EU";
            break;
        case"cz":
            tc_internalvars_countries = "EU";
            break;
        case"dk":
            tc_internalvars_countries = "EU";
            break;
        case"ee":
            tc_internalvars_countries = "EU";
            break;
        case"fi":
            tc_internalvars_countries = "EU";
            break;
        case"fr":
            tc_internalvars_countries = "EU";
            break;
        case"de":
            tc_internalvars_countries = "EU";
            break;
        case"gr":
            tc_internalvars_countries = "EU";
            break;
        case"hu":
            tc_internalvars_countries = "EU";
            break;
        case"is":
            tc_internalvars_countries = "EU";
            break;
        case"ie":
            tc_internalvars_countries = "EU";
            break;
        case"it":
            tc_internalvars_countries = "EU";
            break;
        case"lv":
            tc_internalvars_countries = "EU";
            break;
        case"li":
            tc_internalvars_countries = "EU";
            break;
        case"lt":
            tc_internalvars_countries = "EU";
            break;
        case"lu":
            tc_internalvars_countries = "EU";
            break;
        case"mk":
            tc_internalvars_countries = "EU";
            break;
        case"mt":
            tc_internalvars_countries = "EU";
            break;
        case"md":
            tc_internalvars_countries = "EU";
            break;
        case"mc":
            tc_internalvars_countries = "EU";
            break;
        case"me":
            tc_internalvars_countries = "EU";
            break;
        case"nl":
            tc_internalvars_countries = "EU";
            break;
        case"no":
            tc_internalvars_countries = "EU";
            break;
        case"pl":
            tc_internalvars_countries = "EU";
            break;
        case"pt":
            tc_internalvars_countries = "EU";
            break;
        case"ro":
            tc_internalvars_countries = "EU";
            break;
        case"ru":
            tc_internalvars_countries = "EU";
            break;
        case"sm":
            tc_internalvars_countries = "EU";
            break;
        case"rs":
            tc_internalvars_countries = "EU";
            break;
        case"sk":
            tc_internalvars_countries = "EU";
            break;
        case"si":
            tc_internalvars_countries = "EU";
            break;
        case"se":
            tc_internalvars_countries = "EU";
            break;
        case"es":
            tc_internalvars_countries = "EU";
            break;
        case"ch":
            tc_internalvars_countries = "EU";
            break;
        case"tr":
            tc_internalvars_countries = "EU";
            break;
        case"ua":
            tc_internalvars_countries = "EU";
            break;
        case"gb":
            tc_internalvars_countries = "EU";
            break;
        case"va":
            tc_internalvars_countries = "EU";
            break;
        default:
            tc_internalvars_countries = "";
    }

    /*END_VARIABLES_BLOCK*/


    /*DYNAMIC JS BLOCK 2*/


    /*END DYNAMIC JS BLOCK 2*/

    /*CUSTOM_JS_BLOCK2*/

    /*END_CUSTOM_JS_BLOCK2*/
}

//----------------------------------------------------


//----

if (tC.getCookie('tc_mode_test') == 1) {
    (function () {
        var tc_testmodescriptexec = document.createElement('script');
        tc_testmodescriptexec.type = 'text/javascript';
        tc_testmodescriptexec.src = '//manager.tagcommander.com/utils/test_mode_include.php?id=5&site=2496&type=exec&rand=' + Math.random() + '&version=3.02';
        (document.getElementsByTagName('body')[0] || document.getElementsByTagName('head')[0] || document.getElementsByTagName('script')[0].parentNode).appendChild(tc_testmodescriptexec);
    })();
    (function () {
        setTimeout(function () {
            if (typeof top.tc_count !== 'undefined') {
                top.tc_count++;
            } else {
                top.tc_count = 1;
            }
            var tc_newscript = document.createElement('script');
            tc_newscript.type = 'text/javascript';
            tc_newscript.src = '//manager.tagcommander.com/utils/livetest/bookmarklet.php?r=' + Math.random() + '&nb=' + top.tc_count + '&container=2496!5&version=3.02';
            (document.getElementsByTagName('body')[0] || document.getElementsByTagName('head')[0] || document.getElementsByTagName('script')[0].parentNode).appendChild(tc_newscript);
        }, 1000);
    })();
} else {
    tC.launchTag('27', 'TagCommander - Data layer QA', '1375', '2496', '5');
    tC.log(tc_vars);
    if ((tc_vars["nav_pagetype"] == 'cart')) {
        tC.launchTag('31', 'Google remarketing ecommerce basket ', '627', '2496', '5');
        tC.template = {};
        tC.template.checkIfDefined = function (parameter) {
            if (typeof parameter == "undefined") {
                return "";
            } else {
                return parameter;
            }
        };
        tC.template.tabProduct = "";
        tC.template.tabPrice = "";
        tC.template.tabCategory = "";
        tC.template.tabQuantity = "";
        for (var i = 0; i < tc_vars["cart_items"].length; i++) {
            var product = tc_vars["cart_items"][i];
            var product_price = product['cart_item_unitprice_ati'];
            if (i === 0) {
                tC.template.tabPrice += tC.template.checkIfDefined(product_price);
                tC.template.tabProduct += tC.template.checkIfDefined(product['cart_item_cod8']);
                tC.template.tabCategory += tC.template.checkIfDefined(product['']);
                tC.template.tabQuantity += tC.template.checkIfDefined(product['cart_item_quantity']);
            } else {
                tC.template.tabPrice += "," + tC.template.checkIfDefined(product_price);
                tC.template.tabProduct += "," + tC.template.checkIfDefined(product['cart_item_cod8']);
                tC.template.tabCategory += "," + tC.template.checkIfDefined(product['']);
                tC.template.tabQuantity += "," + tC.template.checkIfDefined(product['cart_item_quantity']);
            }
        }
        tC.template.text = "ecomm_prodid=" + tC.template.tabProduct + ";ecomm_pagetype=cart;ecomm_category=" + tC.template.tabCategory + ";ecomm_pvalue=" + tC.template.tabPrice + ";ecomm_quantity=" + tC.template.tabQuantity + ";ecomm_nbcartitem=" + tc_vars["cart_itemsNo"] + ";ecomm_totalvalue=" + tc_vars["cart_total"];
        tC.template.img = document.createElement("img");
        tC.template.img.id = "tc_img__1";
        tC.template.img.type = "text/javascript";
        tC.template.img.src = "//googleads.g.doubleclick.net/pagead/viewthroughconversion/" + '977222704' + "/?script=0&url=" + encodeURIComponent(document.location.href) + "&data=" + encodeURIComponent(tC.template.text);
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0] || document.getElementsByTagName('script')[0].parentNode).insertBefore(tC.template.img, null);
    }
    if ((tc_vars["nav_pagetype"] == 'searchresult' || tc_vars["nav_pagetype"] == 'categories')) {
        tC.launchTag('33', 'Google remarketing ecommerce category', '631', '2496', '5');
        tC.template = {};
        tC.template.text = "ecomm_prodid=;ecomm_pagetype=category;ecomm_totalvalue=;ecomm_category=" + tc_vars["nav_section"];
        tC.template.img = document.createElement("img");
        tC.template.img.id = "tc_img__1";
        tC.template.img.type = "text/javascript";
        tC.template.img.src = "//googleads.g.doubleclick.net/pagead/viewthroughconversion/" + '977222704' + "/?script=0&url=" + encodeURIComponent(document.location.href) + "&data=" + encodeURIComponent(tC.template.text);
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0] || document.getElementsByTagName('script')[0].parentNode).insertBefore(tC.template.img, null);
    }
    if ((tc_vars["nav_pagetype"] == 'thankyou' && tc_vars["env_template"] == 'checkout')) {
        tC.launchTag('35', 'Google remarketing ecommerce conversion', '629', '2496', '5');
        tC.template = {};
        tC.template.checkIfDefined = function (parameter) {
            if (typeof parameter == "undefined") {
                return "";
            } else {
                return parameter;
            }
        };
        tC.template.tabProduct = "";
        tC.template.tabPrice = "";
        tC.template.tabCategory = "";
        tC.template.tabQuantity = "";
        for (var i = 0; i < tc_vars["order_items"].length; i++) {
            var product = tc_vars["order_items"][i];
            var product_price = product['order_item_unitprice_ati'].toString();
            product_price = product_price.replace(",", ".");
            if (i === 0) {
                tC.template.tabPrice += tC.template.checkIfDefined(product_price);
                tC.template.tabProduct += tC.template.checkIfDefined(product['order_item_cod8']);
                tC.template.tabCategory += tC.template.checkIfDefined(product['']);
                tC.template.tabQuantity += tC.template.checkIfDefined(product['order_item_quantity']);
            } else {
                tC.template.tabPrice += "," + tC.template.checkIfDefined(product_price);
                tC.template.tabProduct += "," + tC.template.checkIfDefined(product['order_item_cod8']);
                tC.template.tabCategory += "," + tC.template.checkIfDefined(product['']);
                tC.template.tabQuantity += "," + tC.template.checkIfDefined(product['order_item_quantity']);
            }
        }
        tC.template.text = "ecomm_prodid=" + tC.template.tabProduct + ";ecomm_pagetype=purchase;ecomm_category=" + tC.template.tabCategory + ";ecomm_pvalue=" + tC.template.tabPrice + ";ecomm_quantity=" + tC.template.tabQuantity + ";ecomm_quantity=" + tc_vars["order_itemsNo"] + ";ecomm_totalvalue=" + tc_vars["order_amount_ati_without_sf"];
        tC.template.img = document.createElement("img");
        tC.template.img.id = "tc_img__1";
        tC.template.img.type = "text/javascript";
        tC.template.img.src = "//googleads.g.doubleclick.net/pagead/viewthroughconversion/" + '1029068578' + "/?script=0&url=" + encodeURIComponent(document.location.href) + "&data=" + encodeURIComponent(tC.template.text);
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0] || document.getElementsByTagName('script')[0].parentNode).insertBefore(tC.template.img, null);
    }
    if ((tc_vars["nav_pagetype"] == 'home')) {
        tC.launchTag('37', 'Google remarketing ecommerce home', '633', '2496', '5');
        tC.template = {};
        tC.template.text = "ecomm_prodid=;ecomm_pagetype=homepage;ecomm_totalvalue=";
        tC.template.img = document.createElement("img");
        tC.template.img.id = "tc_img__1";
        tC.template.img.type = "text/javascript";
        tC.template.img.src = "//googleads.g.doubleclick.net/pagead/viewthroughconversion/" + '977222704' + "/?script=0&url=" + encodeURIComponent(document.location.href) + "&data=" + encodeURIComponent(tC.template.text);
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0] || document.getElementsByTagName('script')[0].parentNode).insertBefore(tC.template.img, null);
    }
    if ((tc_vars["nav_pagetype"] == 'item')) {
        tC.launchTag('39', 'Google remarketing ecommerce product', '635', '2496', '5');
        tC.template = {};
        tC.template.text = "ecomm_prodid=" + tc_vars["product_cod8"] + ";ecomm_pagetype=product" + ";ecomm_category=" + '' + ";ecomm_totalvalue=" + tc_vars["product_unitprice_ati"];
        tC.template.img = document.createElement("img");
        tC.template.img.id = "tc_img__1";
        tC.template.img.type = "text/javascript";
        tC.template.img.src = "//googleads.g.doubleclick.net/pagead/viewthroughconversion/" + '977222704' + "/?script=0&url=" + encodeURIComponent(document.location.href) + "&data=" + encodeURIComponent(tC.template.text);
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0] || document.getElementsByTagName('script')[0].parentNode).insertBefore(tC.template.img, null);
    }
    if ((tc_vars["nav_pagetype"] == 'item')) {
        tC.launchTag('45', 'Remarketing AdWords', '26', '2496', '5');
        var google_tag_params = {
            ecomm_prodid: tc_vars["product_cod10"],
            ecomm_pagetype: tc_vars["env_template"],
            ecomm_totalvalue: tc_vars["product_unitprice_ati"]
        };
        var google_conversion_id = 977222704;
        var google_custom_params = window.google_tag_params;
        var google_remarketing_only = true;
        tC.scriptElt1 = document.createElement("script");
        tC.scriptElt1.id = "tc_script_45_1";
        tC.scriptElt1.type = "text/javascript";
        tC.scriptElt1.src = "//www.googleadservices.com/pagead/conversion.js";
        tC.scriptElt1.async = true;
        tC.scriptElt1.defer = 'defer';
        (document.getElementsByTagName('head')[0] || document.getElementsByTagName('body')[0] || document.getElementsByTagName('script')[0].parentNode).insertBefore(tC.scriptElt1, null);
    }
}
function tc_events_5(tc_elt, tc_id_event, tc_array_events) {
    tc_array_events["id"] = tc_id_event;
    (function () {
        var l = 'id'.split('|');
        for (var k in l) {
            if (!tc_array_events.hasOwnProperty(l[k])) {
                tc_array_events[l[k]] = '';
            }
        }
    })();
}