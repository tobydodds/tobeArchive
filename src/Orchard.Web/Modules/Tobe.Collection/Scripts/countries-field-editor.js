(function ($) {

    var Country = function (country, index, prefix) {
        var self = this;
        this.id = ko.observable(country ? country.Id : 0);
        this.name = ko.observable(country ? country.Name : null);
        this.index = ko.observable(index ? index : 0);
        this.prefix = prefix;
        
        this.editUrl = function (element) {
            return $(element).data("edit-url").replace("(id)", self.id());
        };

        this.idInputName = ko.computed(function () {
            return prefix + ".Countries[" + this.index() + "].Id";
        }, this);

        this.nameInputName = ko.computed(function () {
            return prefix + ".Countries[" + this.index() + "].Name";
        }, this);
    };

    var CountriesViewModel = function (scope, countries, prefix) {
        var self = this;
        this.countries = ko.observableArray();
        this.prefix = prefix;
        this.scope = scope;

        for (var i = 0; i < countries.length; i++) {
            this.countries.push(new Country(countries[i], i, this.prefix));
        }

        this.selectedCountry = function () {
            var selectedOption = scope.find(".all-countries option:selected");

            if (selectedOption.val() === "") {
                return null;
            }

            return new Country({
                Id: parseInt(selectedOption.val()),
                Name: selectedOption.text()
            }, 0, self.prefix);

        };

        this.addEnabled = ko.observable(false);

        this.hasCountries = ko.computed(function () {
            return this.countries().length > 0;
        }, this);

        this.add = function () {
            var country = this.selectedCountry();
            country.index(this.countries().length);
            this.countries.push(country);

            scope.find(".all-countries").val("");
            this.syncAddEnabled();
        };

        this.remove = function(data, event) {
            var country = data;
            
            if (country != null) {
                self.countries.remove(country);
            }

            self.updateIndices();
            event.preventDefault();
        };

        this.get = function(id) {
            var countries = this.countries();
            for (var i = 0; i < countries.length; i++) {
                var country = countries[i];
                if (country.id() === id) {
                    return country;
                }
            }
            return null;
        };

        this.syncAddEnabled = function() {
            self.addEnabled(self.selectedCountry() != null);
        };

        this.updateIndices = function() {
            var countries = this.countries();
            
            for (var i = 0; i < countries.length; i++) {
                countries[i].index(i);
            }
        };

        this.onCountrySelected = function() {
            self.syncAddEnabled();
        };
    };

    $(function () {
        var countriesEditors = $("[data-countries]");

        countriesEditors.each(function () {
            var countryEditor = $(this);
            var countriesViewModel = new CountriesViewModel(countryEditor, countryEditor.data("countries"), countryEditor.data("prefix"));

            countryEditor.find(".all-countries").on("change", countriesViewModel.syncAddEnabled);
            ko.applyBindings(countriesViewModel, countryEditor[0]);
        });
        
    });

})(jQuery);