// ** JavaScript Namespace pattern

var Ich = {
    namespace: function (name) {
        var parts = name.split(".");
        var ns = this;

        for (var i = 0, len = parts.length; i < len; i++) {
            ns[parts[i]] = ns[parts[i]] || {};
            ns = ns[parts[i]];
        }

        return ns;
    }
}

// Namespace template -- use this as a template on local pages

Ich.namespace("Local").Page = (function () {
    var start = function () {
    };

    return { start: start };
})

// Displays failure and success alert boxes

Ich.namespace("Utils").Alert = (function () {

    var start = function () {


        $("#alert-success").fadeIn(1000).delay(3000).fadeOut(1000, function () {
            $(this).remove();
        });

        $("#alert-failure").fadeIn(1000).delay(4000).fadeOut(1000, function () {
            $(this).remove();
        });

        $("#alert-info").fadeIn(500).delay(6500).fadeOut(1000, function () {
            $(this).remove();
        });
    };

    return { start: start };

})();

// Confirm delete requests

Ich.namespace("Utils").Delete = (function () {

    var start = function () {

        // delete button. open modal delete confirmation box.

        $('.js-delete').on('click', function (e) {

            if ($(this).attr('href') != "javascript:void(0);") {
                var id = $(this).data('id');
                var returnUrl = $(this).data('return-url');

                // opens and populates modal delete box

                $('#delete-id').val(id);
                $('#delete-return-url').val(returnUrl);
                $('#delete-form').attr('action', $(this).attr("href"));
                $('#delete-modal').modal('show');
            }

            e.preventDefault();
            return false;
        });

        $('.js-submit-delete').on('click', function (e) {

            var url = $('#delete-form').attr('action');
            var id = $('#delete-id').val();
            var token = $('[name="__RequestVerificationToken"]').val();
            var data = { 'id': id, '__RequestVerificationToken': token };

            $.ajax({
                url: url,
                type: 'POST',
                data: data,
                error: function (e) {
                    alert('Sorry, an error occured');
                    location = location;
                },
                success: function (data) {
                    // redirect page or refresh same page

                    var returnUrl = $('#delete-return-url').val();

                    if (returnUrl) {
                        location = returnUrl;
                    } else {
                        location = location;
                    }
                }
            });
        });

        // set the proper referer url before editing

        $('.js-edit').on('click', function (e) {

            var url = window.location.href.split('?')[0];
            history.pushState({}, '', url + "?tab=details");

            return true;
        });
    };

    return { start: start };

})();

// Page tabs, filters, and sorters 

Ich.namespace("Utils").Misc = (function () {

    var start = function () {

        // related tab in detail page is clicked -> display different tab area

        $('.tabs a').on('click', function (e) {

            var tab = $(this).attr("href").substr(1);
            var url = window.location.href.split('?')[0];
            history.pushState({}, '', url + "?tab=" + tab);

            $(this).tab('show');
            e.preventDefault();
            return false;
        });

        // list page: standard filter dropdown changed -> submit

        $('#Filter').on('change', function () {
            $('#Page').val(1);
            $(this).closest('form').submit();
        });

        // advanced filter button is clicked

        $('.js-filter').on('click', function () {
            $('#Page').val(1);
        });

        // sort header is clicked -> submit

        $('[data-sort]').on('click', function () {
            var sort = $(this).data('sort');
            $("#Sort").val(sort);
            $("#Page").val(1);

            $(this).closest('form').submit();
        });

        // page button is clicked -> submit

        $('[data-page]').on('click', function () {
            var page = $(this).data('page');
            $("#Page").val(page);

            $(this).closest('form').submit();
        });

        // Filter toggles are clicked -> animate to different filter area

        $('.standard-toggle').on('click', function () {
            $('#standard-filter').slideDown();
            $('#advanced-filter').slideUp();
            $('#AdvancedFilter').val('False');

            $('.advanced-toggle').removeClass('active');
            $('.standard-toggle').addClass('active');
        });

        $('.advanced-toggle').on('click', function () {
            $('#standard-filter').slideUp();
            $('#advanced-filter').slideDown();
            $('#AdvancedFilter').val('True');

            $('.standard-toggle').removeClass('active');
            $('.advanced-toggle').addClass('active');
        });

        // Initialize popovers

        $('[data-toggle="popover"]').popover({ placement: "top", trigger: "hover", html: true });

        // Initialize color picker

        $('#Color').colorpicker({ format: 'hex' });

        // Search dropdown helper

        $(".js-dropdown-item").click(function () {

            var type = $(this).data('type');
            var text = $(this).text();

            $(".btn-search:first-child").val(text);
            $('.btn-search:first-child').html(text + '&nbsp; <span class="caret"></span>'); //.substr(0, 3)
            $('#search-type').val(type);
        });
    };

    // get parameter value from query string

    var getUrlParameter = function (name) {

        var url = window.location.href;
        name = name.replace(/[\[\]]/g, "\\$&");

        var regex = new RegExp("[?&]" + name + "(=([^&#]*)|&|#|$)");
        var results = regex.exec(url);

        if (!results) return null;
        if (!results[2]) return '';

        return decodeURIComponent(results[2].replace(/\+/g, " "));
    };

    return { start: start, getUrlParameter: getUrlParameter };

})();

// Date pickers

Ich.namespace("Utils").Globalize = (function () {

    var start = function () {

        // Note: culture values are extracted from hidden fields 
        //       in the footer partial view

        // initialize date picker. optionally set language

        var culture2letter = $('#culture-2lettercode').val();

        if (culture2letter && culture2letter != 'en') {
            $('.js-date-picker').datepicker({ language: culture2letter, format: 'd-m-yyyy', autoclose: true });  // Dutch only. When other languages are added adjust format.
        } else {
            $('.js-date-picker').datepicker({ format: 'm/d/yyyy', autoclose: true });
        }
    };

    return { start: start };

})();

$(function () {

    Ich.Utils.Alert.start();
    Ich.Utils.Misc.start();
    Ich.Utils.Delete.start();
    Ich.Utils.Globalize.start();

});