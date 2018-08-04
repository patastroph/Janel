// Write your JavaScript code.
$(document).ready(function () {
  jQuery('[data-confirm]').click(function (e) {
    if (!confirm(jQuery(this).attr("data-confirm"))) {
      e.preventDefault();
    }
  });

  jQuery('[data-selected]').each(function () {    
    var val = jQuery(this).attr("data-selected");    
    jQuery(this).val(val);
  });
  
})