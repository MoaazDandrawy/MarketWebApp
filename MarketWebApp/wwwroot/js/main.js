
'use strict';

(function ($) {

    /*------------------
        Preloader
    --------------------*/
    $(window).on('load', function () {
        $(".loader").fadeOut();
        $("#preloder").delay(200).fadeOut("slow");

        /*------------------
            Gallery filter
        --------------------*/
        $('.featured__controls li').on('click', function () {
            $('.featured__controls li').removeClass('active');
            $(this).addClass('active');
        });
        if ($('.featured__filter').length > 0) {
            var containerEl = document.querySelector('.featured__filter');
            var mixer = mixitup(containerEl);
        }
    });

    /*------------------
        Background Set
    --------------------*/
    $('.set-bg').each(function () {
        var bg = $(this).data('setbg');
        $(this).css('background-image', 'url(' + bg + ')');
    });

    //Humberger Menu
    $(".humberger__open").on('click', function () {
        $(".humberger__menu__wrapper").addClass("show__humberger__menu__wrapper");
        $(".humberger__menu__overlay").addClass("active");
        $("body").addClass("over_hid");
    });

    $(".humberger__menu__overlay").on('click', function () {
        $(".humberger__menu__wrapper").removeClass("show__humberger__menu__wrapper");
        $(".humberger__menu__overlay").removeClass("active");
        $("body").removeClass("over_hid");
    });

    /*------------------
        Navigation
    --------------------*/
    $(".mobile-menu").slicknav({
        prependTo: '#mobile-menu-wrap',
        allowParentLinks: true
    });

    /*-----------------------
        Categories Slider
    ------------------------*/
    $(".categories__slider").owlCarousel({
        loop: true,
        margin: 0,
        items: 4,
        dots: false,
        nav: true,
        navText: ["<span class='fa fa-angle-left'><span/>", "<span class='fa fa-angle-right'><span/>"],
        animateOut: 'fadeOut',
        animateIn: 'fadeIn',
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: true,
        responsive: {

            0: {
                items: 1,
            },

            480: {
                items: 2,
            },

            768: {
                items: 3,
            },

            992: {
                items: 4,
            }
        }
    });


    $('.hero__categories__all').on('click', function () {
        $('.hero__categories ul').slideToggle(400);
    });

    /*--------------------------
        Latest Product Slider
    ----------------------------*/
    $(".latest-product__slider").owlCarousel({
        loop: true,
        margin: 0,
        items: 1,
        dots: false,
        nav: true,
        navText: ["<span class='fa fa-angle-left'><span/>", "<span class='fa fa-angle-right'><span/>"],
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: true
    });

    /*-----------------------------
        Product Discount Slider
    -------------------------------*/
    $(".product__discount__slider").owlCarousel({
        loop: true,
        margin: 0,
        items: 3,
        dots: true,
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: true,
        responsive: {

            320: {
                items: 1,
            },

            480: {
                items: 2,
            },

            768: {
                items: 2,
            },

            992: {
                items: 3,
            }
        }
    });

    /*---------------------------------
        Product Details Pic Slider
    ----------------------------------*/
    $(".product__details__pic__slider").owlCarousel({
        loop: true,
        margin: 20,
        items: 4,
        dots: true,
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: true
    });

    /*-----------------------
        Price Range Slider
    ------------------------ */
    var rangeSlider = $(".price-range"),
        minamount = $("#minamount"),
        maxamount = $("#maxamount"),
        minPrice = rangeSlider.data('min'),
        maxPrice = rangeSlider.data('max');
    rangeSlider.slider({
        range: true,
        min: minPrice,
        max: maxPrice,
        values: [minPrice, maxPrice],
        slide: function (event, ui) {
            minamount.val('$' + ui.values[0]);
            maxamount.val('$' + ui.values[1]);
        }
    });
    minamount.val('$' + rangeSlider.slider("values", 0));
    maxamount.val('$' + rangeSlider.slider("values", 1));

    /*--------------------------
        Select
    ----------------------------*/
    $("select").niceSelect();

    /*------------------
        Single Product
    --------------------*/
    $('.product__details__pic__slider img').on('click', function () {

        var imgurl = $(this).data('imgbigurl');
        var bigImg = $('.product__details__pic__item--large').attr('src');
        if (imgurl != bigImg) {
            $('.product__details__pic__item--large').attr({
                src: imgurl
            });
        }
    });

    /*-------------------
        Quantity change
    --------------------- */
    var sum = 0;
    document.addEventListener('DOMContentLoaded', function () {
        var proQtyElements = document.querySelectorAll('.pro-qty');

        proQtyElements.forEach(function (proQtyElement) {
            var inputElement = proQtyElement.querySelector('input');
            var price = parseFloat(proQtyElement.closest('tr').querySelector('.shoping__cart__price').textContent.replace('L.E', ''));
            var stock = parseInt(proQtyElement.closest('tr').querySelector('.stock').textContent.trim()); // Get the stock value
            proQtyElement.insertAdjacentHTML('afterbegin', '<span class="dec qtybtn">-</span>');
            proQtyElement.insertAdjacentHTML('beforeend', '<span class="inc qtybtn">+</span>');

            proQtyElement.addEventListener('click', function (event) {
                var button = event.target;
                var oldValue = parseFloat(inputElement.value);
                var newVal;

                if (button.classList.contains('inc')) {
                    newVal = oldValue + 1;
                //    newVal = newVal > stock ? stock : newVal;
                } else if (button.classList.contains('dec') && oldValue > 1) {
                    newVal = oldValue - 1;
                } else {
                    newVal = 1;
                }
                if (newVal > stock) {
                    newVal = stock;
                    Swal.fire({
                        icon: "warning",
                        title: "Oops...",
                        text: "Quantity equals available stock!",
                    });
                }
                else {
                    inputElement.value = parseFloat(newVal) ;
                sum = totalPrice;
                var totalPriceElement = proQtyElement.closest('tr').querySelector('.total-price');
                var totalPrice = newVal * price;
                totalPriceElement.textContent = totalPrice.toFixed(2) + 'L.E';
                calculateSum();
                var id = inputElement.id;
                var quantity = newVal;
                var url = "/shoppingcart/UpdateQuantity?id=" + id + "&quantity=" + quantity;
                    window.location.href = url;
                }

            });
        });
    });

    function calculateSum() {
        sum = 0;
        var totalPrices = document.querySelectorAll('.total-price');

        totalPrices.forEach(function (priceElement) {
            sum += parseFloat(priceElement.textContent.replace('L.E', ''));
        });

        document.getElementById('Total').innerHTML = "<li>Total <span>" + parseInt(sum) + "L.E </span></li>";

    }
    
    calculateSum();
})(jQuery);

