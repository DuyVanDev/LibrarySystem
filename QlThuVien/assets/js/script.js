$(document).ready(function () {
    $('#menu-main-menu').addClass('toggle-menu');
    $('.toggle-menu ul.sub-menu, .toggle-menu ul.children').addClass('toggle-submenu');
    $('.toggle-menu ul.sub-menu').parent().addClass('toggle-menu-item-parent');

    $('.toggle-menu .toggle-menu-item-parent').append('<span class="toggle-caret"><i class="fa fa-plus"></i></span>');

    $('.toggle-caret').click(function (e) {
        e.preventDefault();
        $(this).parent().toggleClass('active').children('.toggle-submenu').slideToggle('fast');
    });
    /* menu responsive*/
    $('.menu-open').click(function () {
        $('.menu-responsive').toggleClass('show-mn');
    });
    $('.menu-close').click(function () {
        $('.menu-responsive').toggleClass('show-mn');
    });

    /* back to top*/
    $(window).scroll(function () {
        /* Show hide scrolltop button */
        if ($(window).scrollTop() == 0) {
            $('.topcontrol').stop(false, true).fadeOut(600);
        } else {
            $('.topcontrol').stop(false, true).fadeIn(600);
        }
        /* Main menu on top */
        var h = $(window).scrollTop();
    });

    $('.topcontrol').click(function () {
        $('html, body').animate({ scrollTop: '0px' }, 800);
        return false;
    });

    $('.btn-xoa').click(function () {
        $(this).parents('tr').remove();
    });
    
});

function OnPopup(sPageName, iHeight, iWidth) {
    tb_show('', sPageName + "&TB_iframe=true;height=" + iHeight + ";width=" + iWidth + ";", '');
    return false;
}
