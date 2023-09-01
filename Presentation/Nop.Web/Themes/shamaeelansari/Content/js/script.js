$(document).ready(function () {
    //var owl = $('.owl-carousel');
    //owl.owlCarousel({
    //  nav: true,
    //  loop: true,
    //  autoplay: false,
    //  autoplayTimeout: 5000,
    //  autoplaySpeed: 1000,
    //  autoplayHoverPause: true,
    //  thumbs: false,
    //  responsive: {
    //    0: {
    //      items: 1
    //    },
    //    600: {
    //      items: 1
    //    },
    //    1000: {
    //      items: 1
    //    }
    //  }
    //})


    var box = window.localStorage.getItem('box');

    if (box) {

        if (box === "x3") {
            $('.html-category-page .item-box').css("width", "33.33333%");

        }

        if (box === "x4") {
            $('.html-category-page .item-box').css("width", "25%");

        }

    } else {

        window.localStorage.setItem('box', 'x4');
        $('.html-category-page .item-box').css("width", "25%");

    }
   
   
    

    $('span#filter-close').on('click', function () {
     
        $('.filter-caret').click();

    }); 

    $('.sortarea .product-sorting select option:first-child').text('Sort By');
    $('.sortarea .product-sorting select option:last-child').text('Latest');
    $('#product-price').insertBefore('.add-to-wishlist');


    $('.search-div').addClass('hider');


    if ($(".html-home-page")[0]) {
        $('#myHeader').css('top', '0px');
        $('.customnav-right').css('padding', '90px 0px 0px 0px');
        $('nav .navbar-brand-centered .logo').css({ 'margin-top': '20px' });
        $('nav .navbar-brand-centered .logo').css({ 'width': '85%' });
        $('.navbar-brand-centered').css('overflow', 'visible');
        $('ul.mega-menu').css('padding', '90px 0px 0px 0px');
        //$('.navbar-brand-centered').css('width', '160px');
        $('nav').css('height', '147px');
        $('.customnav-right .dropdown-menu').css('top', '100%');
        $('.master-wrapper-content').css('padding-top', '149px');
        $('.logo-txt').hide();
        $('.logo').show();
        $('nav').css('background', '#fff');


    } else {


        $('.customnav-right').css('padding', '1% 0%');
        $('nav .navbar-brand-centered .logo').css({ 'margin-top': '-100px' });
        $('nav .navbar-brand-centered .logo').css({ 'width': '100%' });
        $('.navbar-brand-centered').css('overflow', 'hidden');
        $('ul.mega-menu').css('padding', '20px 0px');
        //$('.navbar-brand-centered').css('width', '200px');
        $('nav').css('height', '60px');
        $('#myHeader').css('top', '0px');
        $('.customnav-right .dropdown-menu').css('top', '100%')
        $('.master-wrapper-content').css('padding-top', '60px');
        $('.logo-txt').show();
        $('.logo').hide();
        $('nav').css('background', '#fff');


    }







    $('#newsletter-email').focusin(function () {

        $(this).attr('placeholder', '');

    });

    $('#newsletter-email').focusout(function () {


        if ($(this).value != "") {

            $(this).attr('placeholder', 'Enter your email here... ');


        }


        
    });




// var top = $('#myHeader').offset().top;

//$('.sidenav').css('top', top);



    $(".q-add").click(function () {
        $(this).hide();
        $(".sizemain").addClass("sizelist");

    });
    $(".product-holder").mouseleave(function () {
        $(".sizemain").removeClass("sizelist");
        $(".q-add").show();
    });

    //    var owl = $('.main-slider');
    //    owl.owlCarousel({
    //    autoplay: true,
    //    autoplayTimeout: 4000,
    //    loop: true,
    //    items: 1,
    //    center: true,
    //    nav: false,
    //    thumbs: true,
    //    thumbImage: false,
    //    thumbsPrerendered: true,
    //    thumbContainerClass: 'owl-thumbs',
    //    thumbItemClass: 'owl-thumb-item',

    //});

    // var imgheight = $(this).find('.swarovski-img img').innerHeight();
    // $('body').find('.new-collection, .leftimg-holder img').css('height' , imgheight);



    $('.dropdown-menu a[data-toggle="tab"]').click(function (e) {
        e.stopPropagation()
        e.preventDefault();
        $(this).tab('show')
    })

    // var imgwidth = $(this).find('.owl-item img').innerWidth();
    // $('body').find('.banner-text').css('right', imgwidth);
    // console.log('Anil Working');

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

    //var count = 1;
    //var countEl = $(".qty-input").val();
    //function plus() {
    //    count++;
    //    countEl.value = count;
    //}
    //function minus() {
    //    if (count > 1) {
    //        count--;
    //        countEl.value = count;
    //    }
    //}

    $('.plus').click(function () {
        var value = parseInt($(this).parent().find('.qty-input').val());
        $(this).parent().find('.qty-input').val(value + 1);
    })

    $('.minus').click(function () {
        var value = parseInt($(this).parent().find('.qty-input').val());
        $(this).parent().find('.qty-input').val(value - 1);
    })






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

   

//$(document).ready(function () {
  
//        $('.product-holder').each(function () {
//            var product_info_height = $(this).find('.product-info').outerHeight();
//            $('.ajax-cart-button-wrapper').find("input type["button"]").css('bottom', product_info_height);
//            console.log("working");
      
//        })
//    });

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


$(window).scroll(function () {

    



    if ($(window).scrollTop() >= 47 ) {
       // $('#myHeader').addClass('fixed-header');

        if ($(".html-home-page")[0]) {


            $('.customnav-right').css('padding', '1% 0%');
            $('nav .navbar-brand-centered .logo').css({ 'margin-top': '-100px' });
            $('nav .navbar-brand-centered .logo').css({ 'width': '100%' });
            $('.navbar-brand-centered').css('overflow', 'hidden');
            $('ul.mega-menu').css('padding', '20px 0px');
            //$('.navbar-brand-centered').css('width', '200px');
            $('nav').css('height', '60px');
            $('#myHeader').css('top', '0px');
            $('.customnav-right .dropdown-menu').css('top', '100%')
            $('.master-wrapper-content').css('padding-top', '60px');
            $('.logo-txt').show();
            $('.logo').hide();
            $('nav').css('background', '#fff');
        }
   
    }
    else {


        if ($(".html-home-page")[0]) {
            //$('#myHeader').removeClass('fixed-header');
            $('#myHeader').css('top', '0px');
            $('.customnav-right').css('padding', '90px 0px 0px 0px');
            $('nav .navbar-brand-centered .logo').css({ 'margin-top': '20px' });
            $('nav .navbar-brand-centered .logo').css({ 'width': '85%' });
            $('.navbar-brand-centered').css('overflow', 'visible');
            $('ul.mega-menu').css('padding', '90px 0px 0px 0px');
            //$('.navbar-brand-centered').css('width', '160px');
            $('nav').css('height', '147px');
            $('.customnav-right .dropdown-menu').css('top', '100%');
            $('.master-wrapper-content').css('padding-top', '149px');
            $('.logo-txt').hide();
            $('.logo').show();
            $('nav').css('background', '#fff');
        }
    }


    //if ($(window).scrollTop() >= 130) {

    //    $('.fixed').css('position', 'fixed');
    //    $('.fixed').css('z-index', '3');
    //    $('.fixed').css('top', '60px');
    //    $('.filter-area').css('border-bottom', '1px solid #f4c37e');
        
   


        
        
    //} else {

    //    $('.fixed').css('position', 'relative');
    //    $('.fixed').css('z-index', '');
    //    $('.fixed').css('top', '');
    //    $('.filter-area').css('border-bottom', '0px solid #f4c37e');
    //}


    if ($(window).scrollTop() >= 150) {

        $('nav').addClass('h-46');


        $('.sidenav').css('margin-top', '-8px');

        

    
        $('.search-div').removeClass('hider');


    } else {


        $('nav').removeClass('h-46');
        $('.search-div').addClass('hider');
        $('.sidenav').css('margin-top', '1px ');

    }




});



//(function () {



//    $('.picture-thumbs-prev-arrow').






//}());

