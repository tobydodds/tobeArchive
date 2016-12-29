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
            var allGenresElement = scope.find(".all-genres");
            var genreId = parseInt(allGenresElement.data("genre-id"));
            var genreName = allGenresElement.data("genre-name");
            
            if (isNaN(genreId)) {
                return null;
            }

            return new Genre({
                Id: genreId,
                Name: genreName,
            }, null, prefix);
        };

        this.addEnabled = ko.observable(false);

        this.hasGenres = ko.computed(function () {
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
                if (genre.id() === id) {
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

        this.onGenreSelected = function(e, ui) {
            var genreId = ui.item.value;
            var genreName = ui.item.label;
            $(this).data("genre-id", genreId);
            $(this).data("genre-name", genreName);
            $(this).val(genreName);
            self.syncAddEnabled();
            e.preventDefault();
        };
    };

    $(function () {
        var genresEditors = $(".genres-field");

        genresEditors.each(function () {
            var genreEditor = $(this);
            var genresViewModel = new GenresViewModel(genreEditor, genreEditor.data("genres"), genreEditor.data("prefix"));

            genreEditor.find(".all-genres").on("change", genresViewModel.syncAddEnabled);
            ko.applyBindings(genresViewModel, genreEditor[0]);
            
            genreEditor.find(".all-genres").autocomplete({
                select: genresViewModel.onGenreSelected,
                source: function(request, response) {
                    var url = genreEditor.find(".all-genres").data("source-url") + "?term=" + request.term;
                    $.ajax({
                        url: url,
                        cache: false
                    }).then(response);
                }
            });
        });
    });

})(jQuery);