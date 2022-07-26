//********************************************
//                                            
//  DEZINE CUSTOM JS SCRIPTS
//                                             
//********************************************

function fitDropdownMenu(submenu) {
	// alert('FIT');
	var window_w = jQuery(window).width();
	var submenu_r = jQuery(submenu).offset().left + jQuery(submenu).width();
	var submenu_w = jQuery(submenu).width();
	var parent_w = jQuery(submenu).closest('.menu-item-has-children').width();

	// alert('Parent: '+parent_w+' SUBW: '+submenu_w);

	if (submenu_r > window_w) {
		// reposition
		var adjustment = parent_w - submenu_w + 30;
		jQuery(submenu).css('left', adjustment);
	}
	return;
}

jQuery(document).ready(function () {

	//*******************************************
	//** SLICK NAV
	//*******************************************		

	var backButton = "<span class=\"slick-prev hero-nav-arrow slick-back-arrow\"><i class=\"fas fa-chevron-left\"></i></span>";
	var nextButton = "<span class=\"slick-next hero-nav-arrow slick-next-arrow\"><i class=\"fas fa-chevron-right\"></i></span>";

	//*******************************************
	//** SLICK HERO SLIDER
	//*******************************************

	jQuery("#home-hero-slider").slick({
		autoplay: true,
		autoplaySpeed: 5000,
		infinite: true,
		slidesToShow: 1,
		slidesToScroll: 1,
		arrows: true,
		prevArrow: backButton,
		nextArrow: nextButton,
		dots: true,
		fade: true,
		cssEase: 'linear',
		lazyLoad: 'ondemand'
		/*
		responsive: [
		{
		  breakpoint: 776,
		  settings: {
			slidesToShow: 3,
			slidesToScroll: 1
		  }
		},
		{
		  breakpoint: 480,
		  settings: {
			slidesToShow: 2,
			slidesToScroll: 1
		  }
		}
		
	  ]
		*/
	}); // SLICK HERO
	
	jQuery('#flyer-submit').on('click', function (event) {
		event.preventDefault();
		//alert('to-do: form processing'); 
	});

	jQuery('#search-submit').on('click', function (event) {
		event.preventDefault();
		if ($('#search-input').val() == '') {
			alert('Please type any search keywords!');
		}
		window.location.href = '/search?q=' + $('#search-input').val();
	});

	jQuery('#footer-submit').on('click', function (event) {
		event.preventDefault();
		window.location.href = '/contactus?name=' + $('#footer_contactus_name').val() + '&email=' + $('#footer_contactus_email').val() + '&desc=' + $('#footer_contactus_desc').val();
		//alert('Send to Constant Contact');
	});

	jQuery('.nav-login').on('click', function (event) {
		event.preventDefault();
		alert('Go to login');
	});


	/* Pricing section */
	/*
	jQuery('.decoration-select').on('click', function (event) {
		event.preventDefault();

		// what method
		var method = jQuery(this).attr('id');
		method = method.replace('decoration-', '');

		// clear selector button
		jQuery('.decoration-active').removeClass('decoration-active');

		// set new selector
		jQuery(this).addClass('decoration-active');

		// clear all active
		jQuery('.price-section-active').removeClass('price-section-active');


		// set new active
		jQuery('#price-' + method).addClass('price-section-active');
	});
	*/

	jQuery('#action-quote').on('click', function () {
		alert('function requestQuote()');
	});

	jQuery('#action-email').on('click', function () {
		alert('function emailShare()');
	});

	// PRODUCT THUMBNAIL CAROUSEL
	
	var backButton = '<span class="slick-prev hero-nav-arrow slick-back-arrow">&#8249;</span>';
	var nextButton = '<span class="slick-next hero-nav-arrow slick-next-arrow">&#8250;</span>';

	$('.product-thumbnails-carousel').slick({
		// centerMode: true,
		// centerPadding: '60px',
		slidesToShow: 4,
		arrows: true,
		dots: false,
		prevArrow: backButton,
		nextArrow: nextButton,
		responsive: [
			{
				breakpoint: 768,
				settings: {
					arrows: true,
					//centerMode: true,
					//centerPadding: '40px',
					slidesToShow: 3
				}
			},
			{
				breakpoint: 480,
				settings: {
					arrows: true,
					// centerMode: true,
					// centerPadding: '40px',
					slidesToShow: 2
				}
			}
		]
	});
	
	
	// HIDE PRESET SEARCH ON /SEARCH PAGE
	if( $(location).attr("href").indexOf("search") > -1 ){
		$('.search-preset-wrapper').hide();
	}
	
	// PRODUCTS MAIN IMAGE MODAL ON CLICK
	$('#product-image-zoom').on('click',function(){

		// get the image
		var modal_img = $(this).find('img').attr('src');
		
		console.log('OPEN PRODUCT MODAL FOR '+modal_img);
		
		var top_offset = $(window).scrollTop();
		
		var modal_content = '<div id="product-zoom-modal" class="product-modal" style="top:'+top_offset+'px"><div class="product-modal-inner"><div id="product-modal-close" class="product-modal-close"><div class="product-modal-close-button">&times;</div></div><img id="product-modal-img" class="product-modal-image" src="'+modal_img+'" alt=""></div></div><!-- /.product-modal -->';
		
		// append modal
		$('body').append(modal_content);
		
		//show modal
		$('#product-zoom-modal').css('display', 'flex').hide().fadeIn(300);
		
		/*
		// position the modal close
		var img_offset = $('#product-modal-img').offset();
		var img_width = $('#product-modal-img').width();
		var img_height = $('#product-modal-img').height(); 
		var img_nat_width = $('#product-modal-img').get(0).naturalWidth; 
		var img_nat_height = $('#product-modal-img').get(0).naturalHeight;
		// getObjectFitSize(true, img.width, img.height, img.naturalWidth, img.naturalHeight);
		
		var actual_img = getObjectFitSize(true, img_width, img_height, img_nat_width, img_nat_height);
		var img_right = actual_img.width + img_offset.left + 20 + 30;
		
	
		
		alert('TOP: '+img_offset.top+' LEFT: '+img_offset.left+' WIDTH: '+actual_img.width+' RIGHT: '+img_right);
		
		$('#product-modal-close').css('top', img_offset.top+'px');
		$('#product-modal-close').css('left', img_right+'px');
		*/
		
		//prevent body from scrolling
		$('body').addClass('modal-open');
	});
	
	// CLOSE MODAL
	$(document).on('click','.product-modal-close',function(){
		console.log('CLOSE MODAL CLICK');
		$('#product-zoom-modal').fadeOut(300, function(){
			// remove
			$('#product-zoom-modal').remove();
		});
		
		// reset body scrolling
		//prevent body from scrolling
		$('body').removeClass('modal-open');	
	});
	
	/* PRODUCT IMAGE SWAP */
	
	$('.product-thumbnail-wrapper').on('click',function(){
		//get the new image
		var thumb_image = $(this).find('img').attr('src');
		
		// trim the 300 resolution suffix
		if ( thumb_image.indexOf('_300') !== -1){
			thumb_image = thumb_image.replace('_300', '');
		}
				
		console.log('THUMBNAIL SWAP: '+thumb_image);

		// copy to the main image and animate
		$('#main-product-img').fadeOut(200, function(){
			
			$('#main-product-img').attr("src",thumb_image).fadeIn(200);
		});			
	});
	
	// Sort results
	
	$('#products-orderby').on('change',function(){
		// get current option
		var order = $(this).val();
		console.log('CHANGE SORT: '+order);
		
		// reload page with new sort query
		window.location.href = order;
	});

}); // DOCUMENT READY
