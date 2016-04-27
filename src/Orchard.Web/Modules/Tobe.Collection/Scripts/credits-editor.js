(function ($) {

    var Agent = function (agent) {
        var self = this;
        this.id = ko.observable(agent ? agent.id : 0);
        this.name = ko.observable(agent ? agent.name : null);

        this.editUrl = function(element) {
            return $(element).data("edit-url").replace("(id)", self.id());
        };
    };

    var Role = function (role) {
        var self = this;
        this.id = ko.observable(role ? role.id : 0);
        this.name = ko.observable(role ? role.name : null);
        
        this.editUrl = function (element) {
            return $(element).data("edit-url").replace("(id)", self.id());
        };
    };

    var Credit = function (credit, index) {
        this.id = credit ? credit.Id : 0;
        this.agent = new Agent(credit ? { id: credit.AgentId, name: credit.AgentName } : null);
        this.role = new Role(credit ? { id: credit.RoleId, name: credit.RoleName } : null);
        this.index = ko.observable(index ? index : 0);

        this.creditIdInputName = ko.computed(function () {
            return "CreditsContainerPart.Credits[" + this.index() + "].Id";
        }, this);
        
        this.agentIdInputName = ko.computed(function() {
            return "CreditsContainerPart.Credits[" + this.index() + "].AgentId";
        }, this);
        
        this.agentNameInputName = ko.computed(function () {
            return "CreditsContainerPart.Credits[" + this.index() + "].AgentName";
        }, this);
        
        this.roleIdInputName = ko.computed(function () {
            return "CreditsContainerPart.Credits[" + this.index() + "].RoleId";
        }, this);
        
        this.roleNameInputName = ko.computed(function () {
            return "CreditsContainerPart.Credits[" + this.index() + "].RoleName";
        }, this);
    };

    var CreditsViewModel = function (scope, credits) {
        var self = this;
        this.credits = ko.observableArray();

        for (var i = 0; i < credits.length; i++) {
            this.credits.push(new Credit(credits[i], i));
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
        
        this.selectedRole = function () {
            var selectedOption = scope.find(".all-roles option:selected");

            if (selectedOption.val() == "") {
                return null;
            }

            return new Role({
                id: parseInt(selectedOption.val()),
                name: selectedOption.text()
            });

        };

        this.addCreditEnabled = ko.observable(false);

        this.hasCredits = ko.computed(function() {
            return this.credits().length > 0;
        }, this);

        this.addCredit = function () {
            var agent = this.selectedAgent();
            var role = this.selectedRole();
            var credit = new Credit();
            credit.agent = agent;
            credit.role = role;
            credit.index(this.credits().length);
            this.credits.push(credit);

            scope.find(".all-agents").val("").data("agent-id", "");
            scope.find(".all-roles").val("");
            this.syncAddCreditEnabled();
        };

        this.removeCredit = function(data, event) {
            var credit = self.getCreditByAgent(data.agent.id());
            
            if (credit != null) {
                self.credits.remove(credit);
            }

            self.updateIndices();
            event.preventDefault();
        };

        this.getCreditByAgent = function(agentId) {
            var credits = this.credits();
            for (var i = 0; i < credits.length; i++) {
                var credit = credits[i];
                if (credit.agent.id() == agentId) {
                    return credit;
                }
            }
            return null;
        };

        this.syncAddCreditEnabled = function() {
            self.addCreditEnabled(self.selectedAgent() != null && self.selectedRole() != null);
        };

        this.updateIndices = function() {
            var credits = this.credits();
            
            for (var i = 0; i < credits.length; i++) {
                credits[i].index(i);
            }
        };

        this.onAgentSelected = function(e, ui) {
            var agentId = ui.item.value;
            var agentName = ui.item.label;
            $(this).data("agent-id", agentId);
            $(this).data("agent-name", agentName);
            $(this).val(agentName);
            self.syncAddCreditEnabled();
            e.preventDefault();
        };
    };

    $(function () {
        var creditsEditor = $("#creditsEditor");
        var viewModel = new CreditsViewModel(creditsEditor, creditsEditor.data("credits"));

        creditsEditor.find(".all-roles").on("change", viewModel.syncAddCreditEnabled);
        creditsEditor.find(".all-agents").autocomplete({
            select: viewModel.onAgentSelected,
            source: function(request, response) {
                var url = creditsEditor.find(".all-agents").data("source-url") + "?term=" + request.term;
                $.ajax({
                    url: url,
                    cache: false
                }).then(response);
            }
        });
        ko.applyBindings(viewModel, creditsEditor[0]);
    });
})(jQuery);