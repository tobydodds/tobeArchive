(function ($) {
    $(function () {
        $("[data-filter-input]").each(function () {
            var scope = $(this);
            var filterInput = $("#" + scope.data("filter-input"));
            var filterUrl = scope.data("filter-url");

            filterInput
                .data("timeout", null)
                .keyup(function () {
                    clearTimeout($(this).data("timeout"));
                    $(this).data("timeout", setTimeout(filter, 500));
                });

            scope.on("click", "ul.pager a", function () {
                filter(this);
                return false;
            });

            var filter = function(pageLink) {
                var filterValue = $.trim(filterInput.val());
                var params = { filter: filterValue };
                if (pageLink) {
                    params.page = getPageNumber(pageLink);
                    var pageSize = getPageSize(pageLink);

                    if (pageSize) {
                        params.pageSize = pageSize;
                    }
                }
                refreshTable(filterUrl, params);
            };

            var refreshTable = function (url, params) {
                $.get(url, params, function (result) {
                    scope.html(result);
                    initializePager();
                });
            };

            var getPageNumber = function(pageLink) {
                var href = $(pageLink).attr("href");
                return queryString("page", 1, href);
            };

            var getPageSize = function(pageLink) {
                var href = $(pageLink).attr("href");
                return queryString("pageSize", null, href);
            };

            var initializePager = function () {
                $('ul.selector').each(function () {
                    var self = $(this),
                        options = $.map(self.find("li"), function (li) {
                            var self = $(li);
                            return $("<option/>", {
                                value: self.children("a").attr("href"),
                                text: self.text(),
                                selected: self.hasClass("selected")
                            })[0];
                        }),
                        select = $("<select/>", {
                            id: self.attr("id") + "Selector",
                            "class": self.attr("class"),
                            name: self.attr("name") + "Selector"
                        }).change(onPageSizeChanged).append(options);
                    self.replaceWith(select);
                });
            };

            var onPageSizeChanged = function () {
                var filterUrl = $(this).attr("disabled", true).val();
                refreshTable(filterUrl, null);
            };

            var queryString = function(key, default_, url) {
                if (default_ == null) {
                    default_ = "";
                }
                if (url == null) {
                    url = window.location.href;
                }
                key = key.replace(/[\[]/, "\\\[").replace(/[\]]/, "\\\]");
                var regex = new RegExp("[\\?&]" + key + "=([^&#]*)");
                var qs = regex.exec(url);
                if (qs == null)
                    return default_;
                else
                    return qs[1];
            };
        });
    });

})(jQuery);