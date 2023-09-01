$(document).ready(function(){
  $(".shownav").click(function(){
    $(".inner").slideToggle();
  });

  


$(window).scroll(function(){
  var vHieght = $(".video-holder").height();
    if ($(window).scrollTop() >= vHieght) {
        $('nav').addClass('fixed-header');
        $('nav div').addClass('visible-title');
    }
    else {
        $('nav').removeClass('fixed-header');
        $('nav div').removeClass('visible-title');
    }
});

$(window).scroll(function(){
  var vHieght = $(".video-holder").height();
    if ($(window).scrollTop() >= vHieght) {
        $('.mobile-menu ').addClass('fixed-header');
        $('.mobile-menu ').addClass('visible-title');
    }
    else {
        $('.mobile-menu ').removeClass('fixed-header');
        $('.mobile-menu ').removeClass('visible-title');
    }
});

$('.pro-nav-section li').click(function() {
    $(this).addClass('current').siblings().removeClass('current');
    
});


  $('.btnclose').click(function(e) {
    $(this).closest('.current').removeClass('current');
      e.stopPropagation();
  });


});

function openNav() {
  document.getElementById("mySidenav").style.height = "100%";
}

function closeNav() {
  document.getElementById("mySidenav").style.height = "0";
}


var viewer       = document.querySelector('.viewer'),
    frame_count  = 24,
    offset_value = 100;

// init controller
var controller = new ScrollMagic.Controller({
  globalSceneOptions: {
    triggerHook: 0,
    reverse: true
  }
});

// build pinned scene
new ScrollMagic.Scene({
  triggerElement: '#sticky',
  duration: (frame_count * offset_value) + 'px',
  reverse: true
})
.setPin('#sticky')
//.addIndicators()
.addTo(controller);

// build step frame scene
for (var i = 1, l = frame_count; i <= l; i++) {
  new ScrollMagic.Scene({
      triggerElement: '#sticky',
      offset: i * offset_value
    })
    .setClassToggle(viewer, 'frame' + i)
    //.addIndicators()
    .addTo(controller);
}


/*This Code Is For Mobile View Animation*/

var viewer       = document.querySelector('.mobileviewer'),
    frame_count  = 24,
    offset_value = 100;

// init controller
var controller = new ScrollMagic.Controller({
  globalSceneOptions: {
    triggerHook: 0,
    reverse: true
  }
});

// build pinned scene
new ScrollMagic.Scene({
  triggerElement: '#stickymobile',
  duration: (frame_count * offset_value) + 'px',
  reverse: true
})
.setPin('#stickymobile')
//.addIndicators()
.addTo(controller);

// build step frame scene
for (var i = 1, l = frame_count; i <= l; i++) {
  new ScrollMagic.Scene({
      triggerElement: '#stickymobile',
      offset: i * offset_value
    })
    .setClassToggle(viewer, 'frame' + i)
    //.addIndicators()
    .addTo(controller);
}
