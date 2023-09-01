$(document).ready(function() {
        //if ($(window).width() <= 1199) {
        
        //    $('#myContainer-inner').multiscroll.destroy();
        //} 
        //else {
        //console.log("Working");
        //}

 

 

 

    

   

    $(window).on('scroll', function () {
        var $elem = $('.related-products-grid');
        var $window = $(window);

        var docViewTop = $window.scrollTop();
        var docViewBottom = docViewTop + $window.height();
        var elemTop = $elem.offset().top;
        var elemBottom = elemTop + $elem.height();
        if (elemBottom < docViewBottom) {

            $('#right1').addClass('red');
        } else {
            $('#right1').removeClass('red');

        }
    });


});




