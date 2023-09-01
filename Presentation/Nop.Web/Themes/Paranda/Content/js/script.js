(function ($) {

    // Hide the div
    // $(".arrow-animation").hide();

    // Show the div in 5s
    /*$(".arrow-animation").delay(2500).css('opacity', 0)
    		.slideDown('slow')
    		.animate(
	    		{ opacity: 1 },
				{ queue: false, duration: 'slow' }
			);
	*/
    //scroll automatico verso l'ancora
    $('.scrollToAncora').on('click', function (e) {
        e.preventDefault();
        var anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: $(anchor.attr('href')).offset().top - 100
        }, 1500, 'swing');
    });
    $('.scrollToAncoraSTORE').on('click', function (e) {
        e.preventDefault();
        var assesta = 100;
        if ($(window).width() < 551) assesta = 200;
        var anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: $(anchor.attr('href')).offset().top - assesta
        }, 1500, 'swing');
    });
    $('.scrollToAncoraLegal').on('click', function (e) {
        e.preventDefault();
        $('.scrollToAncoraLegal').removeClass('active');
        $(this).addClass('active');
        var assesta = 80;
        //if($(window).width()<551) assesta =200;
        var anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: $(anchor.attr('href')).offset().top - assesta
        }, 1500, 'swing');
    });

    /* Share pictures */
    $('.sharethis').hover(
        function () {

            $(this).append('<div class="velina"><div class="share-picture-btn"><a id="open-share-popup" href="#"><img src="http://www.stefanoricci.com/wp-content/themes/sricci/img/shareicon.png" alt=""><br>SHARE THIS PICTURE</a></div></div>')
        },
        function () {
            $('.velina').remove();
        }
    );
    $('body').on('click', '#open-share-popup', function (e) {
        e.preventDefault();
        var immag = $(this).parents('.sharethis').find('img')
        var src = immag.attr('src');

        var margintop = ($(window).height() - immag.height()) / 2;
        var margi = margintop - 35;
        var thisUrl = window.location.href.split('?')[0];
        var thisUrlTwit = thisUrl + '?img=' + src;
        var pageTitle = document.title;
        var pageDescription = document.getElementsByName('description')[0].getAttribute('content');
        var popup = '<div class="velina-big">' +
            '<div style="width:' + immag.width() + 'px;" id="modal-share-picture">' +
            '<img style="max-width:80%;margin-top:' + margintop + 'px;" src="' + src + '" alt="">' +
            '<div id="ics" style="top:' + margi + 'px;"><i class="fa fa-times" aria-hidden="true"></i></div>' +
            '<div  id="share-btn-container" style="top:' + margintop + 'px;">' +
            '<a href="' + thisUrl + '" data-image="' + src + '" data-title="' + pageTitle + '" data-desc="' + pageDescription + '" class="btnShare" title="Share on Facebook" target="_blank"><i class="fa fa-facebook"></i></a><br>' +
            '<a href="' + thisUrl + '" data-image="' + src + '" data-desc="' + pageTitle + '" class="btnPinit" title="Share on Pinterest" target="_blank"><i class="fa fa-pinterest" aria-hidden="true"></i></a><br>' +
            '<a class="twbtn" href="https://twitter.com/intent/tweet?url=' + encodeURI(thisUrlTwit) + '" title="Share on Twitter" target="_blank"><i class="fa fa-twitter"></i></a>' +
            '</div>' +
            '</div>' +
            '</div>';
        $('body').append(popup);

    });
    $('body').on('click', '#ics,.velina-big', function (e) {
        $('.velina-big').remove();
    });

    /*fb buttom share */

    window.fbAsyncInit = function () {
        FB.init({
            appId: '885360428245593',
            xfbml: true,
            version: 'v2.5'
        });
    };

    (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) { return; }
        js = d.createElement(s);
        js.id = id;
        js.src = "//connect.facebook.net/en_US/sdk.js";
        fjs.parentNode.insertBefore(js, fjs);
    }(document, 'script', 'facebook-jssdk'));


    function postToFeed(title, desc, url, image) {
        var obj = { method: 'feed', link: url, picture: image, name: title, description: desc };

        function callback(response) { }
        FB.ui(obj, callback);
    }
    $('body').on('click', '.btnShare', function () {
        elem = $(this);
        postToFeed(elem.data('title'), elem.data('desc'), elem.prop('href'), elem.data('image'));

        return false;
    });
    /*twitter buttom share */
    $('body').on('click', '.twbtn', function () {

        var url = $(this).attr('href');
        var top = ($(window).height() - 300) / 2;
        var left = ($(window).width() - 400) / 2;
        window.open(url, 'Share on Twitter', 'width=400,height=300,top=' + top + ',left=' + left + '');
        return false;
    });
    /*pin buttom share */
    $('body').on('click', '.btnPinit', function () {
        var url = $(this).attr('href');
        var top = ($(window).height() - 500) / 2;
        var left = ($(window).width() - 760) / 2;
        var media = $(this).attr('data-image');
        var desc = $(this).attr('data-desc');
        window.open("//www.pinterest.com/pin/create/button/" +
            "?url=" + url +
            "&media=" + media +
            "&description=" + desc, 'Share on Pinterest', 'width=760,height=500,top=' + top + ',left=' + left + '');
        return false;
    });

    /* Carousel Touch */

    $("#myCarousel").swiperight(function () {
        $(this).carousel('prev');
    });

    $("#myCarousel").swipeleft(function () {
        $(this).carousel('next');
    });

    $("#myCarousel").carousel({
        interval: 6000
    });


    $('.tooltip').tooltipster();

    /* Tooltip in lookbook */

    $('.tooltip-left').tooltipster({
        contentAsHTML: true,
        position: 'left',
        offsetX: 20,

    });

    $('.tooltip-right').tooltipster({
        contentAsHTML: true,
        position: 'right',
        offsetX: 20,

    });

    /* Panel Lookbook */

    $(".button-panel").click(function () {
        $(".panel-lookbook").toggleClass("show-panel");

    });

    var toggleVaribale = false;

    $('.button-panel').bind('click', function () {

        video_url = $(this).data('video-url');

        focus_text = $(this).data('focus-text');

        iframe = $('#iframe-panel');

        if (!toggleVaribale) {

            iframe_source = video_url + "?loop=1&autoplay=1"
            iframe.attr('src', iframe_source);

            $('#focus-text').html(focus_text);

            $('#myCarousel').carousel('pause');

            toggleVaribale = true;

        } else {

            url = iframe.attr('src');
            if (url.indexOf("autoplay") > 0) {
                new_url = url.substring(0, url.indexOf("?"));
                iframe.attr('src', new_url + '');
            }

            $('#myCarousel').carousel('cycle');

            //function
            toggleVaribale = false;

        }

    });



    /* Home Scroll */

    $('#scroll-down h2, #scroll-down h3, #scroll-down i, .text-cont-video i, .scroll-down, .scroll-down-arrow .fa').click(function (event) {
        event.preventDefault();
        var n = $(document).height();
        $('html, body').animate({ scrollTop: $('.mos-container, .cont-container, .cont-video-small, .scroll-cont').position().top - /*offset */ 55 }, 1000);
    });

    /* Store doble slide  2 slide */
    $('.scroll-down-arrow-2 .fa').click(function (event) {
        event.preventDefault();
        var n = $(document).height();
        $("html, body").animate({ scrollTop: $(document).height() }, 1000);
    });


    /* Store Locatore Scroll */
    $('#scroll-store-locator').click(function (event) {
        event.preventDefault();
        var scroll = $('.container-store').height();
        $('html, body').animate({ scrollTop: scroll }, 1000);
    });
    /* Print Store */


    $("#print_store").click(function () {
        $('#schedashop').printElement({ printMode: 'popup' });
    })




    /* Animate link to the top */

    $("a[href='#top']").click(function () {
        $("html, body").animate({ scrollTop: 0 }, "slow");
        return false;
    });

    /* Activate gallery */

    $('.gallery').magnificPopup({
        gallery: { enabled: true },
        delegate: 'a', // child items selector, by clicking on it popup will open
        type: 'image',
        // other options
    });

    /* Activate popup video */

    $('.popup-vimeo').magnificPopup({
        disableOn: 0,
        type: 'iframe',
        mainClass: 'mfp-fade',
        removalDelay: 160,
        preloader: false,
        fixedContentPos: false
    });

    $('#close_popup').on("click", function () {
        $.magnificPopup.close();
    });

    /* Activate popup video for china user */

    $('.popup-modal').magnificPopup({
        type: 'inline',
        preloader: false,
        focus: '#username',
        modal: true,
        callbacks: {
            open: function () {

                var magnificPopup = $.magnificPopup.instance,
                    cur = magnificPopup.st.el;
                MyVideoOpen = cur.attr("data-video");

                focus_text = cur.attr('data-focus-text');

                $('#focus-text-china').html(focus_text);

                jwplayer("mediaplayer").setup({
                    "flashplayer": "http://www.jellytest.com/sricci/wp-content/themes/sricci/js/player.swf",
                    "file": MyVideoOpen,
                    "autostart": "true",
                    "height": "100%",
                    "width": "100%",
                    "stretching": "bestfit",

                });



            },
            close: function () {

            }
        }
    });
    $(document).on('click', '.popup-modal-dismiss', function (e) {
        e.preventDefault();
        $.magnificPopup.close();
        //location.reload();

    });



    /* Isotope Mosaic */

    var $container = $('.mos-container'),
        colWidth = function () {

            var w = $container.width(),
                columnNum = 1,
                columnWidth = 0;

            /* Break Point */
            if (w > 767) {
                columnNum = 3;
            } else if (w > 0) {
                columnNum = 2;
            }

            columnWidth = Math.floor(w / columnNum);
            $container.find('.mos-item').each(function () {
                var $item = $(this),
                    multiplier_w = $item.attr('class').match(/item-w(\d)/),
                    multiplier_h = $item.attr('class').match(/item-h(\d)/),

                    multiplier_sm_w = $item.attr('class').match(/item-sm-w(\d)/),
                    multiplier_sm_h = $item.attr('class').match(/item-sm-h(\d)/),


                    /* width form desktop */
                    width = multiplier_w ? columnWidth * multiplier_w[1] : columnWidth,

                    /* height */
                    height = multiplier_h ? columnWidth * multiplier_h[1] * 0.9 : columnWidth * 0.9;

                /* width form mobile */
                widthMobile = multiplier_sm_w ? columnWidth * multiplier_sm_w[1] : columnWidth,

                    /* height for mobile */
                    heightMobile = multiplier_sm_h ? columnWidth * multiplier_sm_h[1] * 0.9 : columnWidth * 0.9;


                /* Break Point */
                if (w > 767) {

                    /* desktop */
                    $item.css({
                        width: width,
                        height: height
                    });

                } else if (w > 0) {

                    /* mobile */
                    $item.css({
                        width: widthMobile,
                        height: heightMobile
                    });
                }


            });
            return columnWidth;



        },
        isotope = function () {
            $container.isotope({
                resizable: false,
                itemSelector: '.mos-item',
                masonry: {
                    columnWidth: colWidth(),
                    gutterWidth: 4
                }
            });
        };

    isotope();

    $(window).resize(isotope);
    $('#discovermorenews').click(function (e) {
        e.preventDefault();

        $('.mos-item.more').toggleClass('open');

        isotope();
        $(this).hide();
    });

    //$(".cop-video-full").on("loadedmetadata", scaleVideo);
    //$(window).on('load', scaleVideo);
    //$(window).on('resize', scaleVideo);


    //function scaleVideo() {

    //    var windowHeight = $(window).height();
    //    var windowWidth = $(window).width();


    //    var videoNativeWidth = $(".cop-video-full")[0].videoWidth;
    //    var videoNativeHeight = $(".cop-video-full")[0].videoHeight;



    //    var hightScaleFactor = windowHeight / videoNativeHeight;
    //    var widthScaleFactor = windowWidth / videoNativeWidth;


    //    if (widthScaleFactor > hightScaleFactor) {

    //        var scale = widthScaleFactor;

    //    } else {

    //        var scale = hightScaleFactor;

    //    }

    //    var scaledVideoHeight = videoNativeHeight * scale;

    //    var scaledVideoWidth = videoNativeWidth * scale;

    //    //var videoMargin = (videoNativeWidth-windowWidth) / 3;

    //    $(".cop-video-full").height(scaledVideoHeight);
    //    $(".cop-video-full").width(scaledVideoWidth);

    //    //$(".cop-video-full").css('margin-left', -videoMargin);
    //    //$('.cop-video-full').css('marginLeft', windowWidth - videoNativeWidth);

    //    /* HO AGGIUNTO QUESTO PER CENTRARE IL VIDEO */

    //    var centertop = (scaledVideoHeight - windowHeight) / 2;
    //    var centerleft = (scaledVideoWidth - windowWidth) / 2;
    //    $(".cop-video-full").css({ 'margin-top': '-' + centertop + 'px', 'margin-left': '-' + centerleft + 'px' });

    //    /* FINE AGGIUNTA  */


    //}

    // $("body").on("contextmenu", "img", function(e) {
    // return false;
    // });
    function isMobile() {
        try { document.createEvent("TouchEvent"); return true; } catch (e) { return false; }
    }
    /* Tap Hover Pagine Singole */
    if (isMobile()) {
        var taps = $('.taphover');

        $.each(taps,
            function (k, v) {
                if ($(v).find('.mos-panel').hasClass('animate')) {
                    $(v).append('<i style="position:absolute; top:15px; right:15px; color:#fff; font-size:26px;" class="fa fa-chevron-down tapicon"></i>');

                }
            }
        );
    }
    $('.taphover').click('touchstart touchend', function () {
        var thistap = $(this);
        $(this).toggleClass('hover-show');
        if (isMobile() && (thistap.find('.mos-panel').hasClass('animate'))) {
            if (thistap.hasClass('hover-show')) {
                thistap.children('.tapicon').remove();
                thistap.append('<i style="position:absolute;z-index:10; top:15px; right:15px; color:#fff; font-size:26px;" class="fa fa-chevron-up tapicon"></i>');
            } else {
                thistap.children('.tapicon').remove();
                thistap.append('<i style="position:absolute; top:15px; right:15px; color:#fff; font-size:26px;" class="fa fa-chevron-down tapicon"></i>');
            };
        }

    });

    /* Seleziona una option della tendina Request Area della pagina contatti a seconda della url di provenienza */

    if ($('body').hasClass("page-id-789")) { //verifico se sono nella pagina contatti

        //catturo la url di provenienza
        var referral = document.referrer;
        if (referral == 'http://www.stefanoricci.com/made-to-measure/') { //se arrivo da qui eseguo
            //aggiungo selected alla option
            $('.wpcf7-select').children('option')
                .each(function () {

                    if ($(this).val() == 'MADE-TO-MEASURE') {
                        $(this).attr('selected', true);
                    }

                });
        }
        if (referral == 'http://www.stefanoricci.com/bespoke-interiors/') { //se arrivo da qui eseguo
            //aggiungo selected alla option BESPOKE INTERIORS
            $('.wpcf7-select').children('option')
                .each(function () {

                    if ($(this).val() == 'BESPOKE INTERIORS') {
                        $(this).attr('selected', true);
                    }

                });
        }
    }


}(jQuery));