jQuery(function($) {
    "use strict";
    var count=0,
        finalEnd ,
        start= $(".productPopulate").attr("data-current-index");
        function getProducts(selector,start,step,end){
//      selector.parent().addClass("loading");
        $.getJSON( "products.json", function(data) {
             selector.html("");
             $.each(data, function(i, products) {
                 $.each(products.slice(start,end), function(j, product) {
                     count=count+1;
                    selector.append(
                        '<div id="prod-'+product.id+'-'+count+'" class="product text-center">' +
                            '<div class="productImages">' +
                                '<div class="image-default imgAsBG"><img class="hidden" src="'+product.img+'"/></div>' +
                                '<div class="image-hover imgAsBG"><img class="hidden" src="'+product.imgback+'"/></div>' +
                            '</div>' +
                            '<span class="tag">'+product.tags+'</span>' +
                            '<ul class="links">' +
                                '<li><a href="#"><i class="xv-basic_heart"></i></a></li>' +
                                '<li><a class="add_to_cart_button" href="#"><i class="xv-ecommerce_cart_content"></i></a></li>' +
                                '<li><a href="#"><i class="xv-basic_eye"></i></a></li>' +
                            '</ul>' +
                            '<div class="productInfo">' +
                                '<h3><a href="#">'+product.name+'</a></h3>' +
                                '<div class="star-rating '+product.rating+'"></div>' +
                                '<span class="price">'+product.price+'</span>' +
                            '</div>' +
                        '</div>');
                 });
             });
            finalEnd = data.products.length;
            selector.attr("data-current-index",end);
            $(".imgAsBG > img").each(function () {
                var $this = $(this),
                    bg = $this.attr("src");
                $this.parent().css("background-image","url("+bg+")");
            });
        });
    }
    function xvprodView(selector,stepType){
        var step = $(".productPopulate").attr(stepType),
            end = start+step;
        getProducts(selector,start,step,end);
    }
    
    $(".products.ajaxify").each(function(){
        var $this = $(this),
        populateArea = $(this).find(".productPopulate");
        if($(this).hasClass("style1")){
            xvprodView(populateArea,"data-step-sm");
        }
        else{
            xvprodView(populateArea,"data-step-lg");
        }
    });
    
    
    $("body").on("click",".prodViewBig",function(e){
        var $this = $(this);
        e.preventDefault();
        $this.parents(".productsWrap").find(".products").removeClass("style1");
        $this.parents(".productsWrap").find(".products").addClass("prod-style-lg");
        $this.parents(".changeView").find("li").removeClass("active");
        $this.parent("li").addClass("active");
        xvprodView($(this).parents(".products.ajaxify").find(".productPopulate"),"data-step-lg");
        $this.parents(".controls").find(".product-next").removeClass("disabled");
        $this.parents(".controls").find(".product-prev").addClass("disabled");
    });
    
    $("body").on("click",".prodViewSmall",function(e){
        var $this = $(this);
        e.preventDefault();
        $this.parents(".productsWrap").find(".products").addClass("style1");
        $this.parents(".productsWrap").find(".products").removeClass("prod-style-lg");
        $this.parents(".changeView").find("li").removeClass("active");
        $this.parent("li").addClass("active");
        xvprodView($(this).parents(".products.ajaxify").find(".productPopulate"),"data-step-sm");
        $this.parents(".controls").find(".product-next").removeClass("disabled");
        $this.parents(".controls").find(".product-prev").addClass("disabled");
    });
    
    $("body").on("click",".product-next",function(e){
        e.preventDefault();
        var $this = $(this),
            step_new,
            selector = $this.parents(".products.ajaxify").find(".productPopulate"),
            start = +selector.attr("data-current-index");
            if($(this).parents(".products.ajaxify").hasClass("style1"))
                step_new = +selector.attr("data-step-sm")
            else
                step_new = +selector.attr("data-step-lg")
            var step = step_new,
                end = start+step_new;
            $(this).parents(".customslider-controls").find(".product-prev").removeClass("disabled");
        if(end > finalEnd){
            end = finalEnd;
            start = finalEnd-step_new;
            $(this).addClass("disabled");
        }else{
            $(this).removeClass("disabled");
        }
        getProducts(selector,start,step,end);
    });
    
    $("body").on("click",".product-prev",function(e){
        e.preventDefault();
        var $this = $(this),
            step_new,
            selector = $this.parents(".productsWrap").find(".productPopulate");
            if($(this).parents(".products.ajaxify").hasClass("style1"))
                step_new = +selector.attr("data-step-sm")
            else
                step_new = +selector.attr("data-step-lg")
                
            var step = step_new,
            end = +selector.attr("data-current-index")-step,
            start = end-step;
            $(this).parents(".customslider-controls").find(".product-next").removeClass("disabled");
        if(start < 0){
            start = 0;
            end = start+step;
            $(this).addClass("disabled");
        }else{
            $(this).removeClass("disabled");
        }
        getProducts(selector,start,step,end);
    });
    
});