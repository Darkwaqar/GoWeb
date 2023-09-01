$(document).ready(function(){

    /* ------------------------------------- */
    /* 2. MultiScroll ...................... */
    /* ------------------------------------- */

    var onMobile = false;
            
    if( /Android|webOS|iPhone|iPad|iPod|BlackBerry/i.test(navigator.userAgent) ) { onMobile = true; }

    if( ( onMobile === false ) ) {

        $('#multi-div').multiscroll({
            // anchors: ['Home', 'Contact', 'Services', 'Contact'],
            loopTop: true,
            loopBottom: true,
            navigation: true,
            navigationTooltips: ['Home', 'Contact', 'Services', 'Contact'],
        });

    } else {

        $('#multi-div').multiscroll({
            // anchors: ['Home', 'Contact', 'Services', 'Contact'],
            loopTop: true,
            loopBottom: true,
            navigation: true,
            navigationTooltips: ['Home', 'Contact', 'Services', 'Contact'],
        });

        $('#multi-div').multiscroll.destroy();
    }

});