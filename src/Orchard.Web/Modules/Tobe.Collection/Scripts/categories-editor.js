(function ($) {

    var Genre = function (genre) {
        var self = this;
        this.id = ko.observable(genre ? genre.id : 0);
        this.name = ko.observable(genre ? genre.name : null);

        this.editUrl = function(element) {
            return $(element).data("edit-url").replace("(id)", self.id());
        };
    };
    
    var Category = function (category, index) {
        this.id = category ? category.Id : 0;
        this.genre = new Genre(category ? { id: category.GenreId, name: category.GenreName } : null);
        this.index = ko.observable(index ? index : 0);
        this.removed = ko.observable(category != null && category.removed === true);

        this.categoryIdInputName = ko.computed(function () {
            return "CategoriesContainerPart.Categories[" + this.index() + "].Id";
        }, this);
        
        this.genreIdInputName = ko.computed(function() {
            return "CategoriesContainerPart.Categories[" + this.index() + "].GenreId";
        }, this);
        
        this.genreNameInputName = ko.computed(function () {
            return "CategoriesContainerPart.Categories[" + this.index() + "].GenreName";
        }, this);

        this.removedInputName = ko.computed(function () {
            return "CategoriesContainerPart.Categories[" + this.index() + "].Removed";
        }, this);
    };

    var CategoriesViewModel = function (scope, categories) {
        var self = this;
        this.categories = ko.observableArray();

        for (var i = 0; i < categories.length; i++) {
            this.categories.push(new Category(categories[i], i));
        }

        this.selectedGenre = function () {
            var selectedOption = scope.find(".all-genres option:selected");

            if (selectedOption.val() == "") {
                return Genre.NullInstance;
            }

            return new Genre({
                id: parseInt(selectedOption.val()),
                name: selectedOption.text()
            });

        };

        this.addCategoryEnabled = ko.observable(false);

        this.hasCategories = ko.computed(function() {
            return this.categories().length > 0;
        }, this);

        this.addCategory = function () {
            var genre = this.selectedGenre();
            var category = new Category();
            category.genre = genre;
            category.index(this.categories().length);
            this.categories.push(category);

            scope.find(".all-genres").val("");
            this.syncAddCategoryEnabled();
        };

        this.removeCategory = function (data, event) {
            var category = data;

            if (category != null) {
                category.removed(true);
            }

            self.updateIndices();
            event.preventDefault();
        };

        this.getCategoryByGenre = function(genreId) {
            var categories = this.categories();
            for (var i = 0; i < categories.length; i++) {
                var category = categories[i];
                if (category.genre.id() == genreId) {
                    return category;
                }
            }
            return null;
        };

        this.syncAddCategoryEnabled = function() {
            self.addCategoryEnabled(self.selectedGenre() != null);
        };

        this.updateIndices = function() {
            var categories = this.categories();
            
            for (var i = 0; i < categories.length; i++) {
                categories[i].index(i);
            }
        };

        this.onGenreSelected = function(e, ui) {
            var genreId = ui.item.value;
            var genreName = ui.item.label;
            $(this).data("genre-id", genreId);
            $(this).data("genre-name", genreName);
            $(this).val(genreName);
            self.syncAddCategoryEnabled();
            e.preventDefault();
        };
    };

    $(function () {
        var categoriesEditor = $("#categoriesEditor");
        var viewModel = new CategoriesViewModel(categoriesEditor, categoriesEditor.data("categories"));

        categoriesEditor.find(".all-genres").on("change", viewModel.syncAddCategoryEnabled);

        ko.applyBindings(viewModel, categoriesEditor[0]);
    });
})(jQuery);