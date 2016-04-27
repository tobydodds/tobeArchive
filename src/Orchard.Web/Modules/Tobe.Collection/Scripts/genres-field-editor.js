(function ($) {

    var Genre = function (genre, index, prefix) {
        var self = this;
        this.id = ko.observable(genre ? genre.Id : 0);
        this.name = ko.observable(genre ? genre.Name : null);
        this.index = ko.observable(index ? index : 0);
        this.prefix = prefix;
        
        this.editUrl = function (element) {
            return $(element).data("edit-url").replace("(id)", self.id());
        };

        this.idInputName = ko.computed(function () {
            return prefix + ".Genres[" + this.index() + "].Id";
        }, this);

        this.nameInputName = ko.computed(function () {
            return prefix + ".Genres[" + this.index() + "].Name";
        }, this);
    };

    var GenresViewModel = function (scope, genres, prefix) {
        var self = this;
        this.genres = ko.observableArray();
        this.prefix = prefix;
        this.scope = scope;

        for (var i = 0; i < genres.length; i++) {
            this.genres.push(new Genre(genres[i], i, this.prefix));
        }

        this.selectedGenre = function () {
            var selectedOption = scope.find(".all-genres option:selected");

            if (selectedOption.val() == "") {
                return null;
            }

            return new Genre({
                Id: parseInt(selectedOption.val()),
                Name: selectedOption.text()
            }, 0, self.prefix);

        };

        this.addEnabled = ko.observable(false);

        this.hasGenres = ko.computed(function() {
            return this.genres().length > 0;
        }, this);

        this.add = function () {
            var genre = this.selectedGenre();
            genre.index(this.genres().length);
            this.genres.push(genre);

            scope.find(".all-genres").val("");
            this.syncAddEnabled();
        };

        this.remove = function(data, event) {
            var genre = data;
            
            if (genre != null) {
                self.genres.remove(genre);
            }

            self.updateIndices();
            event.preventDefault();
        };

        this.get = function(id) {
            var genres = this.genres();
            for (var i = 0; i < genres.length; i++) {
                var genre = genres[i];
                if (genre.id() == id) {
                    return genre;
                }
            }
            return null;
        };

        this.syncAddEnabled = function() {
            self.addEnabled(self.selectedGenre() != null);
        };

        this.updateIndices = function() {
            var genres = this.genres();
            
            for (var i = 0; i < genres.length; i++) {
                genres[i].index(i);
            }
        };

        this.onGenreSelected = function() {
            self.syncAddEnabled();
        };
    };

    $(function () {
        var genresEditors = $("[data-genres]");

        genresEditors.each(function () {
            var genresEditor = $(this);
            var genresViewModel = new GenresViewModel(genresEditor, genresEditor.data("genres"), genresEditor.data("prefix"));

            genresEditor.find(".all-genres").on("change", genresViewModel.syncAddEnabled);
            ko.applyBindings(genresViewModel, genresEditor[0]);
        });
    });

})(jQuery);