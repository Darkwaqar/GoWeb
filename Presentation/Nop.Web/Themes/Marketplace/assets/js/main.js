/*
 Table OF Contents
 ==========================
 1-Parallax
 2-Twitter feeds
 3-Slick carousel
 4-Pretty Photo
 5-Blog styles
 6-Packery
 8-Accordian
 9-Masonry
 10-Quantity control and cart
 11-header scroll
 12-Dropdowns
 13-Tabs
 14-Insta Feeds
 15-Alert
 16-Tooltip
 17-Nav
 18-Search lookbook
 19-Google Maps
 20-Archieve style
 21-Portfolio
 22-Filter

 */
jQuery(function($) {
    "use strict";

    var xv_ww = $(window).width();

    $("body.getting-ready").css("padding-top", ($(window).height() / 2) - 50 + "px");

    $('.bodyWrap').imagesLoaded({
        background: true
    }, function() {
        var loading_delay = setTimeout(function() {
            clearTimeout(loading_delay);
            $(".pre-loading").addClass("loadingDone");
            $("body.getting-ready").css("padding-top", "0");
            $("body").removeClass("getting-ready");
            afterLoading();
            $(window).resize();
        }, 100);
    });

    $(".imgAsBG > img").each(function() {
        var $this = $(this),
            bg = $this.attr("src");
        $this.parent().css("background-image", "url(" + bg + ")");
    });

    /*code to be executed after preloading*/
    function afterLoading() {
        $(".xvModal").each(function() {
            var $this = $(this),
                delayTime = $this.data("delay") * 1000,
                modal_delay = setTimeout(function() {
                    clearTimeout(modal_delay);
                    $this.fadeIn();
                }, delayTime);
        });
    }

    $("body").on("click", ".scroller[href^='#']", function(e) {
        e.preventDefault();
        var $this = $(this),
            target = $this.attr("href");
        if (target.length) {
            target = $(target).offset().top;
        }
        $("body,html").animate({
            scrollTop: target
        });
    });


    $("body").on("click", ".navTriger", function(e) {
        e.preventDefault();
        $(".mainNav").slideToggle();
    });


    $(".mainNav > li").each(function() {
        var $this = $(this);
        if ($this.children("ul,.mega-xv").length) {
            $this.addClass("parent");
        }
    });
    $("body").on("click", ".mainNav > li.parent > a", function(e) {
        var $this = $(this);
        if ($(window).width() < 768) {
            e.preventDefault();
            e.stopImmediatePropagation();
            $this.siblings().slideToggle();
            $this.parent().toggleClass("expanded");
        }
    });

    /*=================================
     Body Click
     ================================*/
    $('body').on("click", function(e) {
        if (!$(e.target).closest('.topLinks').length) {
            $(".topLinks > li > ul,.topLinks > li > div").fadeOut();
        }
    });

    /*=======================================
     1-Parallax
     =======================================*/
    if (xv_ww >= 768) {
        $.stellar({
            horizontalScrolling: false,
            verticalOffset: 0,
            responsive: true,
        });
    }

    /*===========*/
    $(".dial").knob();

    /*================
     2-Twitter feeds
     =================*/

    if ($('.xvtweet').length !== 0) {
        $('.xvtweet').twittie({
            username: 'envato',
            dateFormat: '%Y-%m-%d',
            template: '{{tweet}} <time class="timeago" datetime="{{date}}">{{date}}</time>',
            count: 3,
            apiPath: "assets/php/tweet_api/tweet.php",
        }, function() {
            $("time.timeago").timeago();
            $(".xvtweet ul").addClass("slick-tweet");
            $(".slick-tweet").slick({
                infinite: true,
                slidesToShow: 1,
                arrows: false,
                autoplay: true,
                adaptiveHeight: true
            });
        });
    }

    /*=================
     3-Slick carousel
     =================*/
    var slick = $(".slick-carousel");
    slick.each(function() {
        var $this = $(this),
            viewDots = $this.data("dots"),
            isLoop = $this.data("loop"),
            isNav = $this.data("nav"),
            viewSlides = +$this.data("slides"),
            slidesScroll = +$this.data("slides-scroll"),
            viewSlides_lg = +$this.data("slides-lg"),
            viewSlides_md = +$this.data("slides-md"),
            viewSlides_sm = +$this.data("slides-sm"),
            nextIcon = $this.data("next"),
            prevIcon = $this.data("prev"),
            slideDrag = $this.data("drag"),
            slideFade = $this.data("fade"),
            slideAuto = $this.data("auto"),
            centerMode = $this.data("center"),
            centerPadding = $this.data("center-padding"),
            heightAuto = $this.data("height");
        $this.slick({
            autoplaySpeed: 5000,
            speed: 700,
            infinite: isLoop,
            dots: viewDots,
            arrows: isNav,
            slidesToShow: viewSlides,
            centerPadding: centerPadding,
            draggable: slideDrag,
            fade: slideFade,
            autoplay: slideAuto,
            adaptiveHeight: heightAuto,
            slidesToScroll: slidesScroll,
            centerMode: centerMode,
            prevArrow: '<div class="owl-prev" style=""><i class="btn_prev slick-prev ' + prevIcon + ' "></i></div>',
            nextArrow: '<div class="owl-next" style=""><i class="btn_next slick-next ' + nextIcon + ' "></i></div>',
            responsive: [{
                breakpoint: 1024,
                settings: {
                    slidesToShow: viewSlides_lg
                }
            }, {
                breakpoint: 767,
                settings: {
                    slidesToShow: viewSlides_md
                }
            }, {
                breakpoint: 520,
                settings: {
                    slidesToShow: viewSlides_sm,
                    slidesToScroll: 1,
                    centerPadding: 0
                }
            }],


        }); /*Slick end*/


        $this.on('setPosition', function(event, slick, currentSlide, nextSlide) {
            var slickTimer = setTimeout(function() {
                $(window).resize();
                clearTimeout(slickTimer);
            }, 500);
        });

    }); /*==//each==*/


    $('.slider-for').slick({
        slidesToShow: 1,
        slidesToScroll: 1,
        arrows: false,
        fade: true,
        asNavFor: '.slider-nav'
    });

    $('.slider-nav').slick({
        slidesToShow: 5,
        slidesToScroll: 1,
        asNavFor: '.slider-for',
        dots: false,
        arrows: true,
        centerMode: true,
        centerPadding: 0,
        prevArrow: '<div class="slick-prev"><i class="xv-arrows_square_left"></i></div>',
        nextArrow: '<div class="slick-next"><i class="xv-arrows_square_right"></i></div>',
        focusOnSelect: true
    });

    $("body").on("mouseenter", ".products.style-demo1 .slideHover.right", function(e) {
        var $this = $(this);
        $this.parents(".product").find('.slick-carousel.bags').slick('slickNext');
    });

    $("body").on("mouseenter", ".products.style-demo1 .slideHover.left", function(e) {
        var $this = $(this);
        $this.parents(".product").find('.slick-carousel.bags').slick('slickPrev');
    });

    /*==================
     4-Pretty Photo
     ===================*/
    $("a[data-rel^='prettyPhoto']").prettyPhoto();

    /*===================
     5-Blog styles
     =====================*/

    $("body").on("click", ".changeBlogstyle .blogstyleLg", function(e) {
        var $this = $(this);
        e.preventDefault();
        var r = $this.parents(".xvBlogWrap");
        r.removeClass("postSm");
        r.removeClass("postMd");
        r.addClass("postLg");
        $(".changeBlogstyle li").removeClass("active");
        $this.parent("li").addClass("active");
    });
    $("body").on("click", ".changeBlogstyle .blogstyleMd", function(e) {
        var $this = $(this);
        e.preventDefault();
        var r = $this.parents(".xvBlogWrap");
        r.removeClass("postSm");
        r.removeClass("postLg");
        r.addClass("postMd");
        $(".changeBlogstyle li").removeClass("active");
        $this.parent("li").addClass("active");
    });
    $("body").on("click", ".changeBlogstyle .blogstyleSm", function(e) {
        var $this = $(this);
        e.preventDefault();
        var r = $this.parents(".xvBlogWrap");
        r.removeClass("postMd");
        r.removeClass("postLg");
        r.addClass("postSm");
        $(".changeBlogstyle li").removeClass("active");
        $this.parent("li").addClass("active");
    });


    /*=================
     6-Packery
     =================*/
    var $packeryContainer = $('.packeryWrap');
    $packeryContainer.packery({
        itemSelector: '.item',
        gutter: 0
    });


    /*=================
     7-
     =================*/

    $('.xvStat').bind('inview', function(event, visible) {
        if (visible === true) {
            $(this).parent('.record');
            $(this).html($(this).attr('data-total'));
        }
    });

    /*=================
     8-Accordian
     =================*/
    $("body").on("click", ".xvaccorTriger", function(e) {
        var $this = $(this);
        e.preventDefault();
        var p = $this.parents(".xvAccordianWrap");
        p.find(".accorPane").slideToggle();
        p.toggleClass("active");

    });

    /*=================
     9-Masonry
     =================*/
    $('.masonry').masonry({
        // options
        itemSelector: '.masonry-item'
    });


    /*=======================================
     10-Quantity control and cart
     =======================================*/
    var qtimer;

    function qtyMsg(alertMsg, p, defaultVal) {
        p.find(".alert-msg").remove();
        p.append('<span class="alert-msg">' + alertMsg + '</span>');
        if (typeof defaultVal != "undefined") {
            p.find("input").val(defaultVal);
        }
        qtimer = setInterval(function(e) {
            if (qtimer !== null) {
                clearTimeout(qtimer);
                qtimer = null;
            }
            p.find(".alert-msg").remove();
        }, 4000);
    }

    $('body').on("click", ".quantity-control .btn-plus", function(e) {
        var p = $(this).parent(),
            tInput = p.find("input"),
            tValue = +tInput.val(),
            maxAllowed = +tInput.attr("data-max"),
            alertMsg = tInput.attr("data-maxalert");
        if (tValue < maxAllowed) {
            tInput.val(tValue + 1);
        } else if (!p.find(".alert-msg").length) {
            qtyMsg(alertMsg, p);
        }
    });

    $('body').on("click", ".quantity-control .btn-minus", function(e) {
        var p = $(this).parent(),
            tInput = p.find("input"),
            tValue = +tInput.val(),
            minAllowed = +tInput.attr("data-min"),
            alertMsg = tInput.attr("data-minalert");
        if (tValue > minAllowed) {
            tInput.val(tValue - 1);
        } else if (!p.find(".alert-msg").length) {
            qtyMsg(alertMsg, p);
        }
    });

    $(".quantity-control input").keyup(function() {
        var $this = $(this),
            p = $this.parent(),
            val = $this.val(),
            minAllowed = +$this.attr("data-min"),
            maxAllowed = +$this.attr("data-max"),
            msgMax = $this.attr("data-maxalert"),
            msgMin = $this.attr("data-minalert"),
            msginvalid = $this.attr("data-invalid");
        if ($.isNumeric(val)) {
            if (val > maxAllowed) {
                qtyMsg(msgMax, p, maxAllowed);
            } else if (val < minAllowed) {
                qtyMsg(msgMin, p, minAllowed);
            }
        } else {

            $this.val(minAllowed);
            qtyMsg(msginvalid, p);
        }
    });


    /*======================
     11-header scroll
     ======================*/

    $(window).scroll(function() {
        var scroll = $(window).scrollTop();

        if (scroll >= 100) {
            $(".stickyHeader").addClass("lightBg scrolling");
        } else {
            $(".stickyHeader").removeClass("lightBg scrolling");
        }
    });

    /*=================
     12-Dropdowns
     =================*/

    $("body").on("click", ".navTriger", function(e) {
        e.preventDefault();
        $(".mobile-menu").slideToggle();
    });

    /*=================
     Modals
     ===================*/
    $("body").on("click", ".modalRemove", function(e) {
        e.preventDefault();
        var $this = $(this);
        $this.parents(".xvModal").fadeOut();
    });



    /*============================
     User Quick Actions
     ============================*/
    $("body").on("click", ".topLinks > li > a", function(e) {
        e.preventDefault();
        var $this = $(this);
        $(".topLinks > li > ul,.topLinks > li > div").not($this.siblings("ul,div")).fadeOut();
        $this.siblings("ul,div").fadeToggle();
    });

    $("body").on("click", ".languageDropdown > li > ul > li > a", function(e) {
        var $this = $(this);
        e.preventDefault();
        $this.parent("li").siblings("li").removeClass("active");
        $this.parent("li").addClass("active");
    });


    /*================
     13-Tabs
     ================*/
    $("body").on("click", ".tabs a", function(e) {
        e.preventDefault();
        $(".tabs li").removeClass("active");
        $(this).parent().addClass("active");
        $(".tab-pane").removeClass("active");
        $($(this).attr("href") + ".tab-pane").addClass("active");
        $($(this).attr("href") + ".tab-pane").addClass("opened");

    });

    $("body").on("click", ".accorTrigger", function(e) {
        e.preventDefault();
        $(this).parent().toggleClass("opened");
    });
    /*=========*/
    $("body").on("click", ".sideNavTriger", function(e) {
        e.preventDefault();
        var $this = $(this);
        $this.parents(".innerWrap").find(".sideInnerNav").slideToggle();
    });


    /*==================
     14-Insta Feeds
     ==================*/
    if ($(".instagramFeeds").length) {
        var ins_id = $(".instagramFeeds").data('insta-id'),
            ins_count = $(".instagramFeeds").data('insta-count'),
            ins_token = $(".instagramFeeds").data('insta-token');

        bcr_mig(ins_id, ins_count, ins_token);
    }
    /*==================
     15-Alert
     =================*/

    $("body").on("click", ".xvAlert .remove", function(e) {
        var $this = $(this);
        e.preventDefault();
        $this.parent(".xvAlert").fadeOut();
    });

    /*=================
     16-Tooltip
     =================*/
    $("body").on("click", ".hotspotTriger", function(e) {
        e.preventDefault();
        var $this = $(this),
            p = $this.parents(".hotspotWrap");
        $(".hotspotWrap").not(p).removeClass("active").find(".products").fadeOut();
        p.find(".products").slideToggle();
        p.toggleClass("active");
    });

    /*==================
     17-Nav
     ==================*/
    $("body").on("click", ".slidenavTriger", function(e) {
        var $this = $(this);
        e.preventDefault();
        $this.parents(".coffee-nav").addClass("active");
    });

    $("body").on("click", ".closeNav", function(e) {
        var $this = $(this);
        e.preventDefault();
        $this.parents(".coffee-nav").removeClass("active");
    });
    /*====================
     18-Search lookbook
     ====================*/
    $("body").on("click", ".manageTriger", function(e) {
        var $this = $(this);
        e.preventDefault();
        $this.parent(".manageResults").find(".xvmanage").slideToggle();
    });


    /*================================
     19-Google Maps
     ================================*/

    if ($('.xv-gmap').length !== 0) {
        $('.xv-gmap').each(function() {
            var $this = $(this);

            var selector_map = $this.attr('id'),
                mapAddress = $this.data('address'),
                mapType = $this.data('maptype'),
                zoomLvl = $this.data('zoomlvl'),
                map_theme_control = $this.data('theme'),
                map_theme;

            switch (map_theme_control) {
                case 'pink':
                    map_theme = [{
                        stylers: [{
                            hue: "#505050"
                        }, {
                            visibility: "on"
                        }, {
                            invert_lightness: true
                        }, {
                            saturation: 40
                        }, {
                            lightness: 10
                        }]
                    }];

                    break;
                case 'red':
                    map_theme = [{
                        featureType: "water",
                        elementType: "geometry",
                        stylers: [{
                            color: "#ffdfa6"
                        }]
                    }, {
                        featureType: "landscape",
                        elementType: "geometry",
                        stylers: [{
                            color: "#b52127"
                        }]
                    }, {
                        featureType: "poi",
                        elementType: "geometry",
                        stylers: [{
                            color: "#c5531b"
                        }]
                    }, {
                        featureType: "road.highway",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "#74001b"
                        }, {
                            lightness: -10
                        }]
                    }, {
                        featureType: "road.highway",
                        elementType: "geometry.stroke",
                        stylers: [{
                            color: "#da3c3c"
                        }]
                    }, {
                        featureType: "road.arterial",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "#74001b"
                        }]
                    }, {
                        featureType: "road.arterial",
                        elementType: "geometry.stroke",
                        stylers: [{
                            color: "#da3c3c"
                        }]
                    }, {
                        featureType: "road.local",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "#990c19"
                        }]
                    }, {
                        elementType: "labels.text.fill",
                        stylers: [{
                            color: "#ffffff"
                        }]
                    }, {
                        elementType: "labels.text.stroke",
                        stylers: [{
                            color: "#74001b"
                        }, {
                            lightness: -8
                        }]
                    }, {
                        featureType: "transit",
                        elementType: "geometry",
                        stylers: [{
                            color: "#6a0d10"
                        }, {
                            visibility: "on"
                        }]
                    }, {
                        featureType: "administrative",
                        elementType: "geometry",
                        stylers: [{
                            color: "#ffdfa6"
                        }, {
                            weight: 0.4
                        }]
                    }, {
                        featureType: "road.local",
                        elementType: "geometry.stroke",
                        stylers: [{
                            visibility: "off"
                        }]
                    }];

                    break;
                case 'blue':
                    map_theme = [{
                        stylers: [{
                            hue: "#007fff"
                        }, {
                            saturation: 89
                        }]
                    }, {
                        featureType: "water",
                        stylers: [{
                            color: "#ffffff"
                        }]
                    }, {
                        featureType: "administrative.country",
                        elementType: "labels",
                        stylers: [{
                            visibility: "off"
                        }]
                    }];

                    break;
                case 'yellow':
                    map_theme = [{
                        featureType: "water",
                        elementType: "geometry",
                        stylers: [{
                            color: "#a2daf2"
                        }]
                    }, {
                        featureType: "landscape.man_made",
                        elementType: "geometry",
                        stylers: [{
                            color: "#f7f1df"
                        }]
                    }, {
                        featureType: "landscape.natural",
                        elementType: "geometry",
                        stylers: [{
                            color: "#d0e3b4"
                        }]
                    }, {
                        featureType: "landscape.natural.terrain",
                        elementType: "geometry",
                        stylers: [{
                            visibility: "off"
                        }]
                    }, {
                        featureType: "poi.park",
                        elementType: "geometry",
                        stylers: [{
                            color: "#bde6ab"
                        }]
                    }, {
                        featureType: "poi",
                        elementType: "labels",
                        stylers: [{
                            visibility: "off"
                        }]
                    }, {
                        featureType: "poi.medical",
                        elementType: "geometry",
                        stylers: [{
                            color: "#fbd3da"
                        }]
                    }, {
                        featureType: "poi.business",
                        stylers: [{
                            visibility: "off"
                        }]
                    }, {
                        featureType: "road",
                        elementType: "geometry.stroke",
                        stylers: [{
                            visibility: "off"
                        }]
                    }, {
                        featureType: "road",
                        elementType: "labels",
                        stylers: [{
                            visibility: "off"
                        }]
                    }, {
                        featureType: "road.highway",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "#ffe15f"
                        }]
                    }, {
                        featureType: "road.highway",
                        elementType: "geometry.stroke",
                        stylers: [{
                            color: "#efd151"
                        }]
                    }, {
                        featureType: "road.arterial",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "#ffffff"
                        }]
                    }, {
                        featureType: "road.local",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "black"
                        }]
                    }, {
                        featureType: "transit.station.airport",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "#cfb2db"
                        }]
                    }];


                    break;
                case 'green':
                    map_theme = [{
                        featureType: "water",
                        elementType: "geometry",
                        stylers: [{
                            visibility: "on"
                        }, {
                            color: "#aee2e0"
                        }]
                    }, {
                        featureType: "landscape",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "#abce83"
                        }]
                    }, {
                        featureType: "poi",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "#769E72"
                        }]
                    }, {
                        featureType: "poi",
                        elementType: "labels.text.fill",
                        stylers: [{
                            color: "#7B8758"
                        }]
                    }, {
                        featureType: "poi",
                        elementType: "labels.text.stroke",
                        stylers: [{
                            color: "#EBF4A4"
                        }]
                    }, {
                        featureType: "poi.park",
                        elementType: "geometry",
                        stylers: [{
                            visibility: "simplified"
                        }, {
                            color: "#8dab68"
                        }]
                    }, {
                        featureType: "road",
                        elementType: "geometry.fill",
                        stylers: [{
                            visibility: "simplified"
                        }]
                    }, {
                        featureType: "road",
                        elementType: "labels.text.fill",
                        stylers: [{
                            color: "#5B5B3F"
                        }]
                    }, {
                        featureType: "road",
                        elementType: "labels.text.stroke",
                        stylers: [{
                            color: "#ABCE83"
                        }]
                    }, {
                        featureType: "road",
                        elementType: "labels.icon",
                        stylers: [{
                            visibility: "off"
                        }]
                    }, {
                        featureType: "road.local",
                        elementType: "geometry",
                        stylers: [{
                            color: "#A4C67D"
                        }]
                    }, {
                        featureType: "road.arterial",
                        elementType: "geometry",
                        stylers: [{
                            color: "#9BBF72"
                        }]
                    }, {
                        featureType: "road.highway",
                        elementType: "geometry",
                        stylers: [{
                            color: "#EBF4A4"
                        }]
                    }, {
                        featureType: "transit",
                        stylers: [{
                            visibility: "off"
                        }]
                    }, {
                        featureType: "administrative",
                        elementType: "geometry.stroke",
                        stylers: [{
                            visibility: "on"
                        }, {
                            color: "#87ae79"
                        }]
                    }, {
                        featureType: "administrative",
                        elementType: "geometry.fill",
                        stylers: [{
                            color: "#7f2200"
                        }, {
                            visibility: "off"
                        }]
                    }, {
                        featureType: "administrative",
                        elementType: "labels.text.stroke",
                        stylers: [{
                            color: "#ffffff"
                        }, {
                            visibility: "on"
                        }, {
                            weight: 4.1
                        }]
                    }, {
                        featureType: "administrative",
                        elementType: "labels.text.fill",
                        stylers: [{
                            color: "#495421"
                        }]
                    }, {
                        featureType: "administrative.neighborhood",
                        elementType: "labels",
                        stylers: [{
                            visibility: "off"
                        }]
                    }];

                    break;
                default:
                    map_theme = [];
            }
            contactemaps(selector_map, mapAddress, mapType, zoomLvl, map_theme);

        });
    }

    function contactemaps(selector_map, address, type, zoom_lvl, map_theme) {
        var map = new google.maps.Map(document.getElementById(selector_map), {
            mapTypeId: google.maps.MapTypeId.type,
            scrollwheel: false,
            draggable: false,
            zoom: zoom_lvl,
            styles: map_theme,
        });
        var geocoder = new google.maps.Geocoder();
        geocoder.geocode({
                'address': address
            },
            function(results, status) {
                if (status === google.maps.GeocoderStatus.OK) {
                    new google.maps.Marker({
                        position: results[0].geometry.location,
                        map: map,
                        /* icon: map_pin*/
                    });
                    map.setCenter(results[0].geometry.location);
                }
            });
    }

    /*==========================
     20-Archieve style
     ===========================*/
    $("body").on("click", ".prod-style-2", function(e) {
        e.preventDefault();
        var $this = $(this);
        var q = $this.parents(".xvArchieve").find(".products");
        q.removeClass("col-grid-5");
        q.removeClass("col-grid-4");
        q.removeClass("col-grid-3");
        q.removeClass("style1");
        q.removeClass("style3");
        q.addClass("col-grid-2");
        $this.parents(".change-prod-style").find("li").removeClass("active");
        $this.parent("li").addClass("active");
    });
    $("body").on("click", ".prod-style-3", function(e) {
        e.preventDefault();
        var $this = $(this);
        var q = $this.parents(".xvArchieve").find(".products");
        q.removeClass("col-grid-5");
        q.removeClass("col-grid-4");
        q.removeClass("col-grid-2");
        q.removeClass("style1");
        q.addClass("style3");
        q.addClass("col-grid-3");
        $this.parents(".change-prod-style").find("li").removeClass("active");
        $this.parent("li").addClass("active");
    });
    $("body").on("click", ".prod-style-4", function(e) {
        e.preventDefault();
        var $this = $(this);
        var q = $this.parents(".xvArchieve").find(".products");
        q.removeClass("col-grid-5");
        q.removeClass("col-grid-3");
        q.removeClass("col-grid-2");
        q.removeClass("style1");
        q.addClass("style3");
        q.addClass("col-grid-4");
        $this.parents(".change-prod-style").find("li").removeClass("active");
        $this.parent("li").addClass("active");
    });
    $("body").on("click", ".prod-style-5", function(e) {
        e.preventDefault();
        var $this = $(this);
        var q = $this.parents(".xvArchieve").find(".products");
        q.removeClass("col-grid-2");
        q.removeClass("col-grid-3");
        q.removeClass("col-grid-4");
        q.removeClass("style3");
        q.addClass("style1");
        q.addClass("col-grid-5");
        $this.parents(".change-prod-style").find("li").removeClass("active");
        $this.parent("li").addClass("active");
    });

    /*=============
     21-Portfolio
     =============*/
    $("body").on("click", ".changePortfoliostyle .foliostyleLg", function(e) {
        var $this = $(this);
        e.preventDefault();
        var r = $this.parents(".xvPortfolio");
        r.removeClass("styleMd");
        r.removeClass("styleSm");
        r.addClass("styleLg");
        $(".changePortfoliostyle li").removeClass("active");
        $this.parent("li").addClass("active");
        $(".custom-filter-elements .filter-item").removeClass("folio-last");
        $(".custom-filter-elements .filter-item").each(function() {
            var $this = $(this),
                num;
            num = +$(this).attr("data-sequence");
            if ((num % 2) === 0)
                $(this).addClass("folio-last");
        });
    });
    $("body").on("click", ".changePortfoliostyle .foliostyleMd", function(e) {
        var $this = $(this);
        e.preventDefault();
        var r = $this.parents(".xvPortfolio");
        r.removeClass("styleLg");
        r.removeClass("styleSm");
        r.addClass("styleMd");
        $(".changePortfoliostyle li").removeClass("active");
        $this.parent("li").addClass("active");
        $(".custom-filter-elements .filter-item").removeClass("folio-last");
        $(".custom-filter-elements .filter-item").each(function() {
            var $this = $(this),
                num;
            num = +$(this).attr("data-sequence");
            if ((num % 3) === 0)
                $(this).addClass("folio-last");
        });
    });
    $("body").on("click", ".changePortfoliostyle .foliostyleSm", function(e) {
        var $this = $(this);
        e.preventDefault();
        var r = $this.parents(".xvPortfolio");
        r.removeClass("styleLg");
        r.removeClass("styleMd");
        r.addClass("styleSm");
        $(".changePortfoliostyle li").removeClass("active");
        $this.parent("li").addClass("active");
        $(".custom-filter-elements .filter-item").removeClass("folio-last");
        $(".custom-filter-elements .filter-item").each(function() {
            var $this = $(this),
                num;
            num = +$(this).attr("data-sequence");
            if ((num % 4) === 0)
                $(this).addClass("folio-last");
        });
    });

    $(".custom-filter-elements .filter-item").each(function(indx, ele) {
        var $this = $(this);
        $(this).attr("data-sequence", indx + 1);
    });

    function portfolioStyles() {
        if ($(".xvPortfolio").hasClass("styleLg")) {
            $(".custom-filter-elements .filter-item").each(function() {
                var $this = $(this),
                    num;
                num = +$(this).attr("data-sequence");
                if ((num % 2) === 0)
                    $(this).addClass("folio-last");
            });
        }
        if ($(".xvPortfolio").hasClass("styleMd")) {
            $(".custom-filter-elements .filter-item").each(function() {
                var $this = $(this),
                    num;
                num = +$(this).attr("data-sequence");
                if ((num % 3) === 0){
                    $(this).addClass("folio-last");
                }
            });
        }
        if ($(".xvPortfolio").hasClass("styleSm")) {
            $(".custom-filter-elements .filter-item").each(function() {
                var $this = $(this),
                    num;
                num = +$(this).attr("data-sequence");
                if ((num % 4) === 0){
                    $(this).addClass("folio-last");
                }
            });
        }
    }
    portfolioStyles();

    /*=================
     22-Filter
     =================*/

    $("body").on("click", ".custom-filter li a", function(e) {
        e.preventDefault();
        var $this = $(this),
            q = $(".custom-filter-elements .filter-item"),
            p = $this.attr("data-filter");
        $(".custom-filter li").removeClass("active");
        $(this).parent("li").addClass("active");
        if (p == "*") {
            q.addClass("filter-active").removeClass("filter-disabled");
            q.attr("data-sequence", "");
            q.each(function(indx, ele) {
                var $this = $(this);
                $(this).attr("data-sequence", indx + 1);
            });
            q.removeClass("folio-last");
            portfolioStyles();
        } else {
            q.not(p).addClass("filter-disabled").removeClass("filter-active");
            q.attr("data-sequence", "");
            $(p).addClass("filter-active").removeClass("filter-disabled");
            $(p).each(function(indx, ele) {
                var $this = $(this);
                $(this).attr("data-sequence", indx + 1);
            });
            q.removeClass("folio-last");
            portfolioStyles();
        }

    });

});