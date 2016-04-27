(function ($) {

    var Agent = function (agent, index, prefix) {
        var self = this;
        this.id = ko.observable(agent ? agent.Id : 0);
        this.name = ko.observable(agent ? agent.Name : null);
        this.index = ko.observable(index ? index : 0);
        this.prefix = prefix;
        
        this.editUrl = function (element) {
            return $(element).data("edit-url").replace("(id)", self.id());
        };

        this.idInputName = ko.computed(function () {
            return prefix + ".Agents[" + this.index() + "].Id";
        }, this);

        this.nameInputName = ko.computed(function () {
            return prefix + ".Agents[" + this.index() + "].Name";
        }, this);
    };

    var AgentsViewModel = function (scope, agents, prefix) {
        var self = this;
        this.agents = ko.observableArray();
        this.prefix = prefix;
        this.scope = scope;

        for (var i = 0; i < agents.length; i++) {
            this.agents.push(new Agent(agents[i], i, this.prefix));
        }

        this.selectedAgent = function () {
            var allAgentsElement = scope.find(".all-agents");
            var agentId = parseInt(allAgentsElement.data("agent-id"));
            var agentName = allAgentsElement.data("agent-name");
            
            if (isNaN(agentId)) {
                return null;
            }

            return new Agent({
                Id: agentId,
                Name: agentName,
            }, null, prefix);
        };

        this.addEnabled = ko.observable(false);

        this.hasAgents = ko.computed(function () {
            return this.agents().length > 0;
        }, this);

        this.add = function () {
            var agent = this.selectedAgent();
            agent.index(this.agents().length);
            this.agents.push(agent);

            scope.find(".all-agents").val("");
            this.syncAddEnabled();
        };

        this.remove = function(data, event) {
            var agent = data;
            
            if (agent != null) {
                self.agents.remove(agent);
            }

            self.updateIndices();
            event.preventDefault();
        };

        this.get = function(id) {
            var agents = this.agents();
            for (var i = 0; i < agents.length; i++) {
                var agent = agents[i];
                if (agent.id() === id) {
                    return agent;
                }
            }
            return null;
        };

        this.syncAddEnabled = function() {
            self.addEnabled(self.selectedAgent() != null);
        };

        this.updateIndices = function() {
            var agents = this.agents();
            
            for (var i = 0; i < agents.length; i++) {
                agents[i].index(i);
            }
        };

        this.onAgentSelected = function(e, ui) {
            var agentId = ui.item.value;
            var agentName = ui.item.label;
            $(this).data("agent-id", agentId);
            $(this).data("agent-name", agentName);
            $(this).val(agentName);
            self.syncAddEnabled();
            e.preventDefault();
        };
    };

    $(function () {
        var agentsEditors = $(".agents-field");

        agentsEditors.each(function () {
            var agentEditor = $(this);
            var agentsViewModel = new AgentsViewModel(agentEditor, agentEditor.data("agents"), agentEditor.data("prefix"));

            agentEditor.find(".all-agents").on("change", agentsViewModel.syncAddEnabled);
            ko.applyBindings(agentsViewModel, agentEditor[0]);
            
            agentEditor.find(".all-agents").autocomplete({
                select: agentsViewModel.onAgentSelected,
                source: function(request, response) {
                    var url = agentEditor.find(".all-agents").data("source-url") + "?term=" + request.term;
                    $.ajax({
                        url: url,
                        cache: false
                    }).then(response);
                }
            });
        });
    });

})(jQuery);