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
	//** SLICK TESTIMONIAL SLIDER
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
		cssEase: 'linear'
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




	// sticky nav

	jQuery(window).scroll(function (event) {
		var scroll = jQuery(window).scrollTop();
		// Do something
		if (scroll > 130) {
			// add sticky
			jQuery('#new-header, #section-featured-slider').addClass('sticky');
		}
		else {
			jQuery('#new-header, #section-featured-slider').removeClass('sticky');
		}
	});




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

	jQuery('.search-preset, .product-category-link').on('click', function (event) {
		//event.preventDefault();
		//alert('need category links'); 
	});

	jQuery('.home-hero-slide-cta').on('click', function (event) {
		//event.preventDefault();
		//alert('Hero CTA link needed');
	});

	//jQuery('.home-talk-cta').on('click', function (event) {
	//	event.preventDefault();
	//	alert('Link needed');
	//});

	//jQuery('.more-products-cta').on('click', function (event) {
	//	event.preventDefault();
	//	alert('Link needed');
	//});

	jQuery('.nav-login').on('click', function (event) {
		event.preventDefault();
		alert('Go to login');
	});


	/* Pricing section */
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

	/* CTA actions */
	//jQuery('#action-print').on('click',function(){
	//	window.print(); 	
	//});

	jQuery('#action-quote').on('click', function () {
		alert('function requestQuote()');
	});

	jQuery('#action-email').on('click', function () {
		alert('function emailShare()');
	});


	// handle mobile menu
	jQuery('#mobile-menu-toggle').on('click', function () {
		console.log('HAMBURGER CLICK');
		jQuery('.mobile-nav').toggle();
		jQuery('#mobile-nav-roll').slideToggle(500, function () {
			// on slide complete

			// check for current state
			if (jQuery('#icon-hamburger-collapse').css('display') == 'none') {

				// remove hamburger expand
				jQuery('#icon-hamburger-expand').fadeToggle(200, function () {
					// show hamburger collapse
					jQuery('#icon-hamburger-collapse').fadeToggle(200);
				});
			}
			else {
				// remove hamburger collapse
				jQuery('#icon-hamburger-collapse').fadeToggle(200, function () {
					// show hamburger collapse
					jQuery('#icon-hamburger-expand').fadeToggle(200);
				});
			}
		});
	});

	jQuery('.desktop-nav .menu-item-has-children').on('mouseenter mouseleave', function () {

		// add hover class to hovered element
		jQuery(this).toggleClass('hover');
		jQuery('a', this).toggleClass('hover');

		//get the child menu
		// var submenu = jQuery(this).find('.sub-menu');

		// current state
		var isHovered = jQuery(this).hasClass("hover");

		if (isHovered) {
			jQuery(this).children(".sub-menu").stop().slideDown(100);
			fitDropdownMenu(jQuery(this).children(".sub-menu"));
		}
		else {
			jQuery(this).children(".sub-menu").stop().slideUp();
		}
	}); // hover


	jQuery('.nav-has-subnav').on('mouseenter mouseleave', function () {

		// add hover class to hovered element
		jQuery(this).toggleClass('active');
		jQuery('a', this).toggleClass('active');

		//get the child menu
		// var submenu = jQuery(this).find('.sub-menu');

		// current state
		var isHovered = jQuery(this).hasClass("active");

		if (isHovered) {
			jQuery(this).children(".subnav").stop().slideDown(100);
		}
		else {
			jQuery(this).children(".subnav").stop().slideUp();
		}
	}); // hover

	jQuery('.nav-has-sub-subnav').on('mouseenter mouseleave', function () {

		// add hover class to hovered element
		jQuery(this).toggleClass('active');
		jQuery('a', this).toggleClass('active');

		//get the child menu
		// var submenu = jQuery(this).find('.sub-menu');

		// current state
		var isHovered = jQuery(this).hasClass("active");

		if (isHovered) {
			jQuery(this).children(".sub-subnav").stop().slideDown(100);
		}
		else {
			jQuery(this).children(".sub-subnav").stop().slideUp();
		}
	}); // hover




	jQuery('.mobile-nav .nav-has-subnav').on('click', function (event) {

		// event.preventDefault();

		// add hover class to hovered element
		jQuery(this).toggleClass('hover');

		//get the child menu
		// var submenu = jQuery(this).find('.sub-menu');

		// current state
		var isHovered = jQuery(this).hasClass("hover");

		if (isHovered) {
			jQuery(this).children(".sub-menu").stop().slideDown(300);
			fitDropdownMenu(jQuery(this).children(".sub-menu"));
		}
		else {
			jQuery(this).children(".sub-menu").stop().slideUp(300);
		}
	}); // hover

	
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
					// arrows: false,
					//centerMode: true,
					//centerPadding: '40px',
					slidesToShow: 3
				}
			},
			{
				breakpoint: 480,
				settings: {
					// arrows: false,
					// centerMode: true,
					// centerPadding: '40px',
					slidesToShow: 1
				}
			}
		]
	});


}); // DOCUMENT READY
