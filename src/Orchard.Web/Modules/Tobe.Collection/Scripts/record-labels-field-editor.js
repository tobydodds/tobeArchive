(function ($) {

    var RecordLabel = function (recordLabel, index, prefix) {
        var self = this;
        this.id = ko.observable(recordLabel ? recordLabel.Id : 0);
        this.name = ko.observable(recordLabel ? recordLabel.Name : null);
        this.index = ko.observable(index ? index : 0);
        this.prefix = prefix;
        
        this.editUrl = function (element) {
            return $(element).data("edit-url").replace("(id)", self.id());
        };

        this.idInputName = ko.computed(function () {
            return prefix + ".RecordLabels[" + this.index() + "].Id";
        }, this);

        this.nameInputName = ko.computed(function () {
            return prefix + ".RecordLabels[" + this.index() + "].Name";
        }, this);
    };

    var RecordLabelsViewModel = function (scope, recordLabels, prefix) {
        var self = this;
        this.recordLabels = ko.observableArray();
        this.prefix = prefix;
        this.scope = scope;

        for (var i = 0; i < recordLabels.length; i++) {
            this.recordLabels.push(new RecordLabel(recordLabels[i], i, this.prefix));
        }

        this.selectedRecordLabel = function () {
            var selectedOption = scope.find(".all-recordLabels option:selected");

            if (selectedOption.val() === "") {
                return null;
            }

            return new RecordLabel({
                Id: parseInt(selectedOption.val()),
                Name: selectedOption.text()
            }, 0, self.prefix);

        };

        this.addEnabled = ko.observable(false);

        this.hasRecordLabels = ko.computed(function() {
            return this.recordLabels().length > 0;
        }, this);

        this.add = function () {
            var recordLabel = this.selectedRecordLabel();
            recordLabel.index(this.recordLabels().length);
            this.recordLabels.push(recordLabel);

            scope.find(".all-recordLabels").val("");
            this.syncAddEnabled();
        };

        this.remove = function(data, event) {
            var recordLabel = data;
            
            if (recordLabel !== null) {
                self.recordLabels.remove(recordLabel);
            }

            self.updateIndices();
            event.preventDefault();
        };

        this.get = function(id) {
            var recordLabels = this.recordLabels();
            for (var i = 0; i < recordLabels.length; i++) {
                var recordLabel = recordLabels[i];
                if (recordLabel.id() === id) {
                    return recordLabel;
                }
            }
            return null;
        };

        this.syncAddEnabled = function() {
            self.addEnabled(self.selectedRecordLabel() !== null);
        };

        this.updateIndices = function() {
            var recordLabels = this.recordLabels();
            
            for (var i = 0; i < recordLabels.length; i++) {
                recordLabels[i].index(i);
            }
        };

        this.onRecordLabelSelected = function() {
            self.syncAddEnabled();
        };
    };

    $(function () {
        var recordLabelsEditors = $("[data-record-labels]");

        recordLabelsEditors.each(function () {
            var recordLabelsEditor = $(this);
            var recordLabelsViewModel = new RecordLabelsViewModel(recordLabelsEditor, recordLabelsEditor.data("record-labels"), recordLabelsEditor.data("prefix"));

            recordLabelsEditor.find(".all-recordLabels").on("change", recordLabelsViewModel.syncAddEnabled);
            ko.applyBindings(recordLabelsViewModel, recordLabelsEditor[0]);
        });
    });

})(jQuery);