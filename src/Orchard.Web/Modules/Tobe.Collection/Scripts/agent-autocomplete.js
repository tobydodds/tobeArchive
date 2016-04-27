(function ($) {
    $(function () {
        $("[data-agents-source]").autocomplete({
            source: $("[data-agents-source]").data("agents-source")
        });
    });
})(jQuery);