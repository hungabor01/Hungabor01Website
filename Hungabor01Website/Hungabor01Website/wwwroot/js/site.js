jQuery(function ($) {

  $(".sidebar-dropdown > a").click(function () {
    $(".sidebar-submenu").slideUp(200);
    if (
      $(this)
        .parent()
        .hasClass("active")
    ) {
      $(".sidebar-dropdown").removeClass("active");
      $(this)
        .parent()
        .removeClass("active");
    } else {
      $(".sidebar-dropdown").removeClass("active");
      $(this)
        .next(".sidebar-submenu")
        .slideDown(200);
      $(this)
        .parent()
        .addClass("active");
    }
  });

  $("#close-sidebar").click(function () {
    $(".page-wrapper").removeClass("toggled");
    $("main").removeClass("toggled");
    $(".footer").removeClass("toggled");
  });
  $("#show-sidebar").click(function () {
    $(".page-wrapper").addClass("toggled");
    $("main").addClass("toggled");
    $(".footer").addClass("toggled");
  });
});

$(window).resize(function (e) {
  if ($(window).width() <= 768) {
    $(".page-wrapper").removeClass("toggled");
    $("main").removeClass("toggled");
    $(".footer").removeClass("toggled");
  } else {
    $(".page-wrapper").addClass("toggled");
    $("main").addClass("toggled");
    $(".footer").addClass("toggled");
  }
});