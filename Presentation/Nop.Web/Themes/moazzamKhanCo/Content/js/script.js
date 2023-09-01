function TextHeightResize() {
    var windowWidth = $(window).width();
    if (windowWidth < 975) {
        $(".txt-box").css("height", "auto");
    }
    else {
        $(".txt-box").css("height", $(".textHeight").height());
    }
}

$(window).load(function () {
    var windowWidth = $(window).width();
    if (windowWidth < 975) {
        $(".txt-box").css("height", "auto");
    }
    else {
        //$(".txt-box").css("height", $(".textHeight").height());
        setTimeout(function () {
            $(".txt-box").css("height", $(".textHeight").height());
        }, 3000);
    }
});
$(document).ready(function () {
    TextHeightResize();
});
$(window).resize(function () {
    TextHeightResize();
});

$(document).ready(function () {

    $(".pager li").click(function () { $(window).scrollTop(0) });

    var box = window.localStorage.getItem('box');

    if (box) {

        if (box === "x3") {
            $('.html-category-page .item-box').css("width", "33.33333%");
        }

        if (box === "x4") {
            $('.html-category-page .item-box').css("width", "25%");
        }
    } else {

        window.localStorage.setItem('box', 'x3');
        $('.html-category-page .item-box').css("width", "33.33333%");
    }

    $('span#filter-close').on('click', function () {

        $('.filter-caret').click();
    });

    $('.sortarea .product-sorting select option:first-child').text('Sort By');
    $('.sortarea .product-sorting select option:last-child').text('Latest');
    $('#product-price').insertBefore('.add-to-wishlist');


    $('.search-div').addClass('hider');

    if ($(window).scrollTop() >= 150) {

    }

    if ($(".html-home-page")[0]) {

        $('#myHeader').css('top', '0px');
        $('.customnav-right .dropdown-menu').css('top', '100%');

    } else {

        $('#myHeader').css('top', '0px');
        $('.customnav-right .dropdown-menu').css('top', '100%');

        $('.logo-txt').show();
        $('.logo').hide();
    }

    $('#newsletter-email').focusin(function () {
        $(this).attr('placeholder', '');
    });

    $('#newsletter-email').focusout(function () {
        if ($(this).value !== "") {
            //$(this).attr('placeholder', 'Enter your email here... ');
            $(this).attr('placeholder', 'Email');
        }
    });


    $(".q-add").click(function () {
        $(this).hide();
        $(".sizemain").addClass("sizelist");

    });
    $(".product-holder").mouseleave(function () {
        $(".sizemain").removeClass("sizelist");
        $(".q-add").show();
    });

    $('.dropdown-menu a[data-toggle="tab"]').click(function (e) {
        e.stopPropagation();
        e.preventDefault();
        $(this).tab('show');
    });

    getImageSize($('.swarovski-img img'), function (height) {
        // alert(height);
        $('body').find('.new-collection, .leftimg-holder img').css('height', height);
        console.log(height);
    });

    function getImageSize(img, callback) {
        img = $('.swarovski-img img');

        var wait = setInterval(function () {
            var h = img.height();

            if (h) {
                done(h);
            }
        }, 0);

        var onLoad;
        img.on('load', onLoad = function () {
            done(img.width(), img.height());
        });

        var isDone = false;
        function done() {
            if (isDone) {
                return;
            }
            isDone = true;

            clearInterval(wait);
            img.off('load', onLoad);

            callback.apply(this, arguments);
        }
    }

    /*user-show*/
    $(".user-show").click(function () {
        if ($("#d_m").hasClass("show-user")) {
            $("#d_m").removeClass("show-user");
        }
        else {
            $("#d_m").addClass("show-user");
        }

    });

    $(document).click(function (e) {
        if (!$("#d_m").is(e.target) && $("#d_m").has(e.target).length === 0) {
            $("#d_m").removeClass("show-user");
        }
    });

    $('.x3').click(function () {
        $('.html-category-page .item-box').css("width", "33.33333%");
        window.localStorage.setItem('box', 'x3');

    });
    $('.x4').click(function () {
        $('.html-category-page .item-box').css("width", "25%");
        window.localStorage.setItem('box', 'x4');
    });

   

    $('.nav-icon').toggle(function () {
        openNav1();
    }, function () {
        closeNav1();
    });

    $(document).mouseup(function (e) {
        var container = $("#myNav");
        // If the target of the click isn't the container
        if (!container.is(e.target) && container.has(e.target).length === 0) {
            closeNav1();
        }
    });

    $(".dropdown").width($(window).width());

    $('.play-pause-btn').on('click', function () {

        if ($(this).attr('data-click') === "1") {
            $(this).attr('data-click', 0);
            $(this).html('<i class="fa fa-play-circle-o" aria-hidden="true"></i>');
            $('#proDetailVideo')[0].pause();
        } else {
            $(this).attr('data-click', 1);
            $(this).html('<i class="fa fa-pause-circle-o" aria-hidden="true"></i>');
            $('#proDetailVideo')[0].play();
        }

    });

    /*Product Page Script*/
    $(".RequestAppointment").click(function () {
        $("#reservationDate").slideToggle();
    });
    /*Product Page Script End*/

    $("a.custom-price-whats").attr("href", "https://api.whatsapp.com/send?phone=+923218265077&text=I'm interested in ".concat(window.location.href));

    var owl = $('.owl-carousel');
    owl.owlCarousel({
        margin: 10,
        nav: true,
        center: true,
        loop: false,
        responsive: {
            0: {
                items: 1.3
            },
            600: {
                items: 1.3
            },
            1000: {
                items: 1.3
            }
        }
    });
});


function openNav1() {
    document.getElementById("myNav").style.width = "260px";

    $('.nav-icon').addClass('nav-active');
    $('.nav-icon div').css('transform', 'scale(0)');
}

function closeNav1() {
    document.getElementById("myNav").style.width = "0";
    $('.nav-icon').removeClass('nav-active');
    $('.nav-icon div').css('transform', 'scale(1)');
}

function openNav() {
    document.getElementById("mySidenav").style.height = "100%";
}

function closeNav() {
    document.getElementById("mySidenav").style.height = "0";
}

function openNav3() {
    document.getElementById("contactForm").style.height = "100%";
}

function closeNav3() {
    document.getElementById("contactForm").style.height = "0";
}

var scroll;
$(document).ready(function () {
    scroll = $(".VideoHome video").height() + $('nav').height() + 47;
});
$(window).resize(function () {
    scroll = $(".VideoHome video").height() + $('nav').height() + 47;

    /*mega menu dropdown width*/
    $(".dropdown").width($(window).width());
});

$(window).scroll(function () {
    if ($(window).scrollTop() >= scroll) {

        // if (!$("html").hasClass("html-home-page")) { }

        if ($(".html-home-page")[0]) {
            $('.customnav-right .dropdown-menu').css('top', '100%');
        }
    } else {
        if ($(".html-home-page")[0]) {
            $('.customnav-right .dropdown-menu').css('top', '100%');
            $('.logo-txt').hide();
            $('.logo').show();
        }
    }

    //150
    if ($(window).scrollTop() >= 150) {

        $(".sidenav").css('height', '100%');
        //$('.search-div').removeClass('hider');
    } else {
        //$('.search-div').addClass('hider');
        $('.sidenav').css('margin-top', '1px ');
        $(".sidenav").css('height', '100%');
    }

    $(window).scroll(function () {

        if ($("div").hasClass("VideoHome")) {

           // var nevheight = $(".VideoHome").height();
           var nevheight = $(".AfterVideo").offset().top;

            if ($(window).scrollTop() > nevheight) {

                $("#myHeader").addClass('fixed-header');


                $("#myHeader").css("top", "0px");

                var marginTop = $(".navbar.fixed-header").height() + 25;
                $(".master-wrapper-content").css("margin-top", marginTop + "px");

            } else if ($(window).scrollTop() < nevheight) {

                $("#myHeader").removeClass('fixed-header');
                $("#myHeader").removeAttr("style");
                $(".master-wrapper-content").removeAttr("style");
            }
        } else {
            var nevheight = $(".navbar").height();
            if ($(window).scrollTop() > 71) {

                $("#myHeader").addClass('fixed-header');

                $("#myHeader").css("top", "0px");

                var marginTop = $(".navbar.fixed-header").height() + 25;
                $(".master-wrapper-content").css("margin-top", marginTop + "px");
                $(".master-wrapper-page").css("margin-top", "0");

            } else if ($(window).scrollTop() < nevheight) {

                $("#myHeader").removeClass('fixed-header');
                $("#myHeader").removeAttr("style");
                $(".master-wrapper-content").removeAttr("style");
                $(".master-wrapper-page").removeAttr("style");
            }
        }

    });
});
