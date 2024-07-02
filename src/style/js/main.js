console.log('Ready');

var breakpoint = {};
breakpoint.refreshValue = function () {
  this.value = window.getComputedStyle(document.querySelector('body'), ':before').getPropertyValue('content').replace(/\"/g, '');
};

// if (breakpoint.value == 'tablet') {
//   console.log('Tablet breakpoint');
// } else {
//   console.log('Some other breakpoint');
// }

$(document).ready(function() {

    var scrollTop = 0;

    $("img").unveil(200, function() {
        $(this).load(function() {
            this.style.opacity = 1;
        });
    });

    // TEAM TOGGLE
    $('.js-tab-team-toggle').on('click','.btn',function(e){
        e.preventDefault();

        var clicked = $(this),
            target = clicked.attr('href');
        
        $('#team-tabs').find('.csb-tab--active').removeClass('csb-tab--active');
        $('.js-tab-team-toggle').find('.btn--active').removeClass('btn--active');


        clicked.addClass('btn--active');
        $(target).addClass('csb-tab--active');
    });

    // TEACHING TOGGLE
    $('.js-tab-teaching-toggle').on('click','.btn',function(e){
        e.preventDefault();
        e.stopPropagation();
        
        var clicked = $(this),
        target = clicked.attr('href');

        $('#teaching-tabs').find('.csb-tab--active').removeClass('csb-tab--active');
        $('.js-tab-teaching-toggle').find('.btn--active').removeClass('btn--active');

        clicked.addClass('btn--active');
        $(target).addClass('csb-tab--active');
    });

    //$('.publication:gt(1)').hide();

    $('.js-toggle-publication').on('click',function(e){
        e.preventDefault();

        if ( $('.publication-container').hasClass('active') ) {
            $(this).html('Show more <i class="fa fa-chevron-down"></i>');
            $('.publication--nfeatured').toggleClass('publication--featured publication--nfeatured');
        }else {
            $(this).html('Show less <i class="fa fa-chevron-up"></i>');
            $('.publication--featured').toggleClass('publication--featured publication--nfeatured');
        }
        $('.publication-container').toggleClass('active');
    });


    $('.publication__title').on('mouseover',function(e){
        e.preventDefault();
        $(this).toggleClass('clicked');
    }).on('mouseout',function(e){
        e.preventDefault();
        $(this).toggleClass('clicked');
    });

    $('a[href^="#"]').on('click', function(e) {
        var target = $(this).attr("href");  
        var position = $(target).offset().top;
        if ( position !== 0) {
            e.preventDefault();
            
            if ( $('.header').hasClass('header--fixed') ){
                position = position - $('.header').outerHeight();
            }

            $('html, body').animate({
                scrollTop: position
            }, 500, function() {
                location.hash = target;
            });
            return false;
        }
    });

    // // Set navbar `header--fixed` on scroll. Making it fixed snap to top 
    // function onScroll(){
    //     scrollTop = $(window).scrollTop();

    //     if ( !$('.header').hasClass('header--fixed') && scrollTop > $('.header').height() ){
    //         $('.header').addClass('header--fixed');
    //     }else if (scrollTop <= $('.header').height()){
    //         $('.header--fixed').removeClass('header--fixed');
    //     }
    // }

    // $(window).scroll(onScroll).scroll();

    // $(window).resize(function () {
    //     breakpoint.refreshValue();
    // }).resize();
});

/// Add toggle to all navbar-burgers
document.addEventListener('DOMContentLoaded', () => {
    // Get all "navbar-burger" elements
    const $navbarBurgers = Array.prototype.slice.call(document.querySelectorAll('.navbar-burger'), 0);
  
    // Add a click event on each of them
    $navbarBurgers.forEach( el => {
      el.addEventListener('click', () => {
  
        // Get the target from the "data-target" attribute
        const target = el.dataset.target;
        const $target = document.getElementById(target);
  
        // Toggle the "is-active" class on both the "navbar-burger" and the "navbar-menu"
        el.classList.toggle('is-active');
        $target.classList.toggle('is-active');
      });
    });
  
});

/// START: change csb-navbar color on scroll to top
let scrollpos = window.scrollY
const rptu = document.getElementById("rptu-navbar")
const csb = document.getElementById("csb-navbar")
const logo = document.getElementById("csb-logo")
let scrollChange = rptu.offsetHeight
const add_class_on_scroll = () => {
    csb.classList.add("is-link", "has-text-white")
    csb.classList.remove("is-light")
    logo.src = "./content/images/logo_small.png";
}

const remove_class_on_scroll = () => {
    logo.src = "./content/images/logo_small_dark.png";
    csb.classList.remove("is-link")
    csb.classList.remove("has-text-white")
    csb.classList.add("is-light")
}

document.addEventListener('DOMContentLoaded', function() {
    scrollpos = window.scrollY;
    if (scrollpos >= scrollChange) { add_class_on_scroll() }
    else { remove_class_on_scroll() }
}, {once: true});

window.addEventListener('scroll', function() { 
  scrollpos = window.scrollY;
  if (scrollpos >= scrollChange) { add_class_on_scroll() }
  else { remove_class_on_scroll() }
})
/// End: change csb-navbar color on scroll to top

document.addEventListener('DOMContentLoaded', () => {
  // Functions to open and close a modal
  function openModal($el) {
    $el.classList.add('is-active');
  }

  function closeModal($el) {
    $el.classList.remove('is-active');
  }

  function closeAllModals() {
    (document.querySelectorAll('.modal') || []).forEach(($modal) => {
      closeModal($modal);
    });
  }

  // Add a click event on buttons to open a specific modal
  (document.querySelectorAll('.js-modal-trigger') || []).forEach(($trigger) => {
    
    $trigger.addEventListener('click', () => {
      const modal = $trigger.dataset.target;
      console.log("modal", modal)
      const $target = document.getElementById(modal);
      console.log("clicked", $target)
      openModal($target);
    });
  });

  // Add a click event on various child elements to close the parent modal
  (document.querySelectorAll('.modal-background, .modal-close, .modal-card-head .delete, .modal-card-foot .button') || []).forEach(($close) => {
    const $target = $close.closest('.modal');

    $close.addEventListener('click', () => {
      closeModal($target);
    });
  });

  // Add a keyboard event to close all modals
  document.addEventListener('keydown', (event) => {
    if(event.key === "Escape") {
      closeAllModals();
    }
  });
});