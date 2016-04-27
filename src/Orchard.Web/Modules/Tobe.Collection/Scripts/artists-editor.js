(function ($) {

    var Agent = function (agent) {
        var self = this;
        this.id = ko.observable(agent ? agent.id : 0);
        this.name = ko.observable(agent ? agent.name : null);

        this.editUrl = function(element) {
            return $(element).data("edit-url").replace("(id)", self.id());
        };
    };

    var Delimiter = function (delimiter) {
        var self = this;
        this.id = ko.observable(delimiter ? delimiter.id : null);
        this.name = ko.observable(delimiter ? delimiter.name : null);
        
        this.editUrl = function (element) {
            return $(element).data("edit-url").replace("(id)", self.id());
        };
    };

    Delimiter.NullInstance = { id: null, name: "", editUrl: function() { return "#"; } };
    
    var Artist = function (artist, index) {
        this.id = artist ? artist.Id : 0;
        this.agent = new Agent(artist ? { id: artist.AgentId, name: artist.AgentName } : null);
        this.delimiter = new Delimiter(artist != null ? { id: artist.DelimiterId, name: artist.DelimiterName } : Delimiter.NullInstance);
        this.index = ko.observable(index ? index : 0);
        this.removed = ko.observable(artist != null && artist.removed === true);

        this.artistIdInputName = ko.computed(function () {
            return "ArtistsContainerPart.Artists[" + this.index() + "].Id";
        }, this);
        
        this.agentIdInputName = ko.computed(function() {
            return "ArtistsContainerPart.Artists[" + this.index() + "].AgentId";
        }, this);
        
        this.agentNameInputName = ko.computed(function () {
            return "ArtistsContainerPart.Artists[" + this.index() + "].AgentName";
        }, this);
        
        this.delimiterIdInputName = ko.computed(function () {
            return "ArtistsContainerPart.Artists[" + this.index() + "].DelimiterId";
        }, this);
        
        this.delimiterNameInputName = ko.computed(function () {
            return "ArtistsContainerPart.Artists[" + this.index() + "].DelimiterName";
        }, this);

        this.removedInputName = ko.computed(function () {
            return "ArtistsContainerPart.Artists[" + this.index() + "].Removed";
        }, this);
    };

    var ArtistsViewModel = function (scope, artists) {
        var self = this;
        this.artists = ko.observableArray();

        for (var i = 0; i < artists.length; i++) {
            this.artists.push(new Artist(artists[i], i));
        }

        this.selectedAgent = function () {
            var allAgentsElement = scope.find(".all-agents");
            var agentId = parseInt(allAgentsElement.data("agent-id"));
            var agentName = allAgentsElement.data("agent-name");
            
            if (isNaN(agentId)) {
                return null;
            }

            return new Agent({
                id: agentId,
                name: agentName
            });
            
        };
        
        this.selectedDelimiter = function () {
            var selectedOption = scope.find(".all-delimiters option:selected");

            if (selectedOption.val() == "") {
                return Delimiter.NullInstance;
            }

            return new Delimiter({
                id: parseInt(selectedOption.val()),
                name: selectedOption.text()
            });

        };

        this.addArtistEnabled = ko.observable(false);

        this.hasArtists = ko.computed(function() {
            return this.artists().length > 0;
        }, this);

        this.addArtist = function () {
            var agent = this.selectedAgent();
            var delimiter = this.selectedDelimiter();
            var artist = new Artist();
            artist.agent = agent;
            artist.delimiter = delimiter;
            artist.index(this.artists().length);
            this.artists.push(artist);

            scope.find(".all-agents").val("").data("agent-id", "");
            scope.find(".all-delimiters").val("");
            this.syncAddArtistEnabled();
        };

        this.removeArtist = function(data, event) {
            var artist = data;
            artist.removed(true);

            self.updateIndices();
            event.preventDefault();
        };

        this.getArtistByAgent = function(agentId) {
            var artists = this.artists();
            for (var i = 0; i < artists.length; i++) {
                var artist = artists[i];
                if (artist.agent.id() == agentId) {
                    return artist;
                }
            }
            return null;
        };

        this.syncAddArtistEnabled = function() {
            self.addArtistEnabled(self.selectedAgent() != null);
        };

        this.updateIndices = function() {
            var artists = this.artists();
            
            for (var i = 0; i < artists.length; i++) {
                artists[i].index(i);
            }
        };

        this.onAgentSelected = function(e, ui) {
            var agentId = ui.item.value;
            var agentName = ui.item.label;
            $(this).data("agent-id", agentId);
            $(this).data("agent-name", agentName);
            $(this).val(agentName);
            self.syncAddArtistEnabled();
            e.preventDefault();
        };
    };

    $(function () {
        var artistsEditor = $("#artistsEditor");
        var viewModel = new ArtistsViewModel(artistsEditor, artistsEditor.data("artists"));

        artistsEditor.find(".all-delimiters").on("change", viewModel.syncAddArtistEnabled);
        artistsEditor.find(".all-agents").autocomplete({
            select: viewModel.onAgentSelected,
            source: function(request, response) {
                var url = artistsEditor.find(".all-agents").data("source-url") + "?term=" + request.term;
                $.ajax({
                    url: url,
                    cache: false
                }).then(response);
            }
        });
        ko.applyBindings(viewModel, artistsEditor[0]);
    });
})(jQuery);