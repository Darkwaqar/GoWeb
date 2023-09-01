function toggleAttr(i) {
    
    if ($(".p", $(i)).length) {
        $(".left-custom-panel-icon", $(i)).addClass("m").removeClass("p");
        $(".left-custom-panel-icon", $(i)).html("-");
        $(".panel-body", $(i).parent()).css("display", "block");
    } else {
        $(".left-custom-panel-icon", $(i)).addClass("p").removeClass("m");
        $(".left-custom-panel-icon", $(i)).html("+");
        $(".panel-body", $(i).parent()).css("display", "none");
    }
}

function toggleSpecFilter(t, url) {
  
    var chkId = $(t).attr("id");
    var spcfltID = $(t).attr("id").replace("spflt_", "");
    if ($("#" + chkId + ':checked').length) {       
        window.location = url;
    } else {

        var domain = window.location.origin;
        var params = window.location.pathname;
        var flt = getUrlVars();
        if (flt.specs.indexOf(spcfltID) >= 0) {
            if (flt.specs.indexOf(",") >= 0) {
                var str = flt.specs.split(",");
                var qstr = "?specs=";
                for (var i = 0; i < str.length; i++) {
                    if (str[i] != spcfltID) {
                        qstr = qstr + str[i];
                        if(i != str.length -1)
                        {
                            qstr = qstr + ",";
                        }
                    }
                }
                window.location = domain + params + qstr;
            } else {
                var qstr = window.location.search.replace(spcfltID, "");
              

                window.location = domain + params + qstr;
            }
        }
    }
}

function getUrlVars() {
    var vars = [], hash;
    var hashes = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }
    return vars;
}

function flt()
{
    $("input:checkbox:checked").closest('.left-panel-body').css("display", "block");
    $(".left-custom-panel-icon", $("input:checkbox:checked").closest('.left-panel-body').parent().children()).addClass("m").removeClass("p").html("-");
}