
document.addEventListener('gesturestart', function (e) {
    e.preventDefault();
});

function responsiveCheck() {


    if ($(window).width() > 1022) {


        if ($(".ms-left")[0]) {



            $('#wrapperMain').multiscroll({
                sectionsColor: ['#', '#', '#', '#'],
                anchors: ['first', 'second', 'third','fourth'],

                navigation: false,
                navigationTooltips: ['One', 'Two', 'Three', 'Fourth'],
                loopBottom: true,
                loopTop: false,
                afterResize: function () {



                },
                afterLoad: function (anchorLink, index) {

                },
                onLeave: function (index, nextIndex, direction) {
                    //after leaving section 2
                    //if (index == '1') {

                    //    var myTransition = ($.browser.webkit) ? '-webkit-transition' :
                    //        ($.browser.mozilla) ? '-moz-transition' :
                    //            ($.browser.msie) ? '-ms-transition' :
                    //                ($.browser.opera) ? '-o-transition' : 'transition',
                    //        myRight = { width: '25%' },
                    //        myLeft = { width: '75%' };

                    //    myRight[myTransition] = 'width 1s ease-in-out';
                    //    $('.ms-right').css(myRight);


                    //    myLeft[myTransition] = 'width 1s ease-in-out';
                    //    $('.ms-left').css(myLeft);


                     
                    //    //$('.ms-left').css('width', '75%');


                    //}

                    //else if (index == '2') {
                    //    $('.ms-right').css('width', '75%');
                    //    $('.ms-left').css('width', '25%');
                    //}
                    //else if (index == '3') {

                    //    $('.ms-right').css('width', '25%');
                    //    $('.ms-left').css('width', '75%');
                    //}
                }
            });
        }



        $('.ms-right').css('width', '35%');
        $('.ms-left').css('width', '65%');



    } else if ($(window).width() < 1023) {

        $('.productDetail__gallery__sliderWrap').flickity({
            // options
            // cellAlign: 'left',
            // contain: true
            accessibility: !0,
            cellAlign: "center",
            freeScrollFriction: .075,
            friction: .28,
            namespaceJQueryEvents: !0,
            percentPosition: !0,
            resize: !0,
            selectedAttraction: .025,
            setGallerySize: !0,
            contain: true,
            pageDots: true,
            prevNextButtons: false,
        });
     
        $('.ms-right').css('width', '100%');
        $('.ms-left').css('width', '100%');
    }

}


function openNav(id) {
    document.getElementById(id).style.width = "300px";

   

}

function closeNav() {

    $('.sidenav').css('width', '0');
}



function close_cart() {
    $('.sidebarBase').css('display', 'none');
    $('.sidebarBase').css('opacity', '0');
}


function open_cart() {

    $('.sidebarBase').css('display', 'block');
    $('.sidebarBase').css('opacity', '1');


}




$(window).on('resize', function () {


    if ($(window).width() > 1022) {

        location.reload();
    }



    responsiveCheck();

});

$(document).ready(function () {


    $('.sidebarBase__bg').on('click', function () {
        close_cart();
        closeNav('cart');

    });



    responsiveCheck();

    //$('.ms-right .homeBox__image').css('display', 'none');
    //$('.ms-right .homeBox__image:first-child').css('display', 'block');

    $('.swap').click(function () {

        //$('.ms-right .homeBox__image').animate({

        //    opacity: '0',
           
        //});


        if ($(this).text().trim() === "Romaan") {
            $('.ms-right .Romaan').css('display', 'block');
            $('.Romaan').animate({
                opacity: '1', 
               
            });

        } else if ($(this).text().trim() === "Titli") {
            $('.ms-right .Titli').css('display', 'block');
            $('.Titli').animate({
                opacity: '1',
                
            });

        } else if ($(this).text().trim() === "Uroosa") {
            $('.ms-right .Uroosa').css('display', 'block');
            $('.Uroosa').animate({
                opacity: '1',
               
            });

        } else if ($(this).text().trim() === "Henna") {
            $('.ms-right .Henna').css('display', 'block');
            $('.Henna').animate({
                opacity: '1',
               
            });
        }

    });


    //$(window).resize(function () {
    //   window.location.reload();
      
    //});






   
    $('#right1 .homeBox__titleBox').removeClass('tr').addClass('bl');
    $('#right1 .homeBox__titleBox').removeClass('serif').addClass('sans');
    $('#right1 .homeBox__brief').removeClass('bl').addClass('tl');
    $('#right2 .collections__collection__title').removeClass('bl').addClass('br');

    $('#small-search-box-form').on('keyup keypress', function (e) {
        var keyCode = e.keyCode || e.which;
        if (keyCode === 13) {
            e.preventDefault();
            return false;
        }
    });
    
});


//(function () {
//var videoPlayButton,
//    videoWrapper = document.getElementsByClassName('video-wrapper')[0],
//    video = document.getElementsByTagName('video')[0],
//    videoMethods = {
//        renderVideoPlayButton: function () {
//            if (videoWrapper.contains(video)) {
//                this.formatVideoPlayButton()
//                video.classList.add('has-media-controls-hidden')
//                videoPlayButton = document.getElementsByClassName('video-overlay-play-button')[0]
//                videoPlayButton.addEventListener('click', this.hideVideoPlayButton)
//            }
//        },

//        formatVideoPlayButton: function () {
//            videoWrapper.insertAdjacentHTML('beforeend', '\
//                <svg class="video-overlay-play-button" viewBox="0 0 200 200" alt="Play video">\
//                    <circle cx="100" cy="100" r="90" fill="none" stroke-width="15" stroke="#fff"/>\
//                    <polygon points="70, 55 70, 145 145, 100" fill="#fff"/>\
//                </svg>\
//            ')
//        },

//        hideVideoPlayButton: function () {
//            video.play()
//            videoPlayButton.classList.add('is-hidden')
//            video.classList.remove('has-media-controls-hidden')
//            video.setAttribute('controls', 'controls')
//        }
//    }

//videoMethods.renderVideoPlayButton()





//    }());