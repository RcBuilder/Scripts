
/******* Modules *******
************************/

function iModule(elem) {
    var that = this; // reference

    // properties
    this._elem = $(elem);
    this._queryName = null;
    this._isDisabled = false;

    // public methods
    this.equal = equal;
    this.hide = hide;
    this.isPopulated = isPopulated;

    // initialization
    (function () {
        that._queryName = that._elem.attr('data-query-name') || null;
        
        var isDisabled = that._elem.attr('data-disabled');
        if (isDisabled)
            that._isDisabled = isDisabled == '1';
    })();

    // methods
    function equal(other) {
        // isEqualNode
        return that._elem[0] === other;
    }

    function hide() {
        that._elem.hide(1000);
    }

    // abstract methods
    function setSelected(data) { throw new Error("Not implemented."); }
    function getSelected() { throw new Error("Not implemented."); }
    function isPopulated() { throw new Error("Not implemented."); }
}

iRangeModule.prototype = Object.create(iModule.prototype);
iListModule.prototype = Object.create(iModule.prototype);
freeSearchModule.prototype = Object.create(iModule.prototype);

function iRangeModule(elem) {
    var that = this; // reference

    iModule.call(that, elem); // base

    // properties
    this._leftBox = null;
    this._rightBox = null;

    // public methods
    this.getRange = getRange;

    // initialization
    (function () { })();

    // methods
    function getRange() {
        var invalidClass = 'invalid';

        if (!Validate()) {
            this._leftBox.addClass(invalidClass);
            this._rightBox.addClass(invalidClass);
            return null;
        }

        this._leftBox.removeClass(invalidClass);
        this._rightBox.removeClass(invalidClass);

        return {
            min: that._leftBox.val(),
            max: that._rightBox.val()
        };
    };

    function Validate() {
        var min = that._leftBox.val();
        var max = that._rightBox.val();

        if (min != '' && max != '')
            return parseInt(max) >= parseInt(min);
        return true;
    }

    // protected
    return {
        bindAction: function (eventName, selector, action) {
            that._elem.on(eventName, selector, function (x) { eval(action)(that) });
        },
        bindBoxesOnlyNumbersAction: function () {

            that._elem.on('keydown', 'input[type = "number"]', function (e) {
                var keyCode = (e.keyCode > 0) ? e.keyCode : e.charCode;

                // between 0 - 9  or left/right arrows
                var isAllowed = ((keyCode >= 48 && keyCode <= 57) || (e.keyCode >= 96 && e.keyCode <= 105) || e.keyCode >= 110 || e.keyCode <= 190 || keyCode == 37 || keyCode == 39 || keyCode == 8);
                /// console.log(keyCode + ': ' + isAllowed);

                if (!isAllowed) {
                    e.preventDefault();
                    return false;
                }
            });
        },
    };
}

numericRangeModule.prototype = Object.create(iRangeModule.prototype);

function iListModule(elem) {
    var that = this; // reference

    iModule.call(that, elem); // base

    // properties
    this._list = null;
    this._EmptyIfNoPhrase = true;

    // public methods
    this.clearItems = clearItems;
    this.hideItems = hideItems;
    this.showItems = showItems;
    this.disableItems = disableItems;
    this.getItems = getItems;
    this.clearSelected = clearSelected;
    this.findInput = findInput;
    this.initScrollbar = initScrollbar;
    this.bindItems = bindItems;
    this.getSelectedIds = getSelectedIds;
    this.countSelected = countSelected;

    // initialization
    (function () {

        // ul (_list) is not exists - add it
        var ulExists = that._elem.find('> ul').length > 0;
        that._list = ulExists ? that._elem.find('> ul:first') : $('<ul />');
        that._elem.append(that._list);

        that.initScrollbar();

        var listHeight = that._elem.attr('data-list-height');
        if (listHeight) that._list.css('max-height', listHeight);

        var emptyIfNoPhrase = that._elem.attr('data-empty-list-if-no-phrase');
        if (emptyIfNoPhrase)
            that._EmptyIfNoPhrase = emptyIfNoPhrase == '1';
    })();

    // methods
    function clearItems(theList) {
        theList = theList || that._list;
        theList.empty();
    }

    function hideItems(theList) {
        theList = theList || that._list;
        theList.find('li').hide();
    }

    function showItems(theList) {
        theList = theList || that._list;
        theList.find('li').show();
    }

    function disableItems(theList) {
        theList = theList || that._list;
        theList.find('li input').prop('disabled', 'disabled');
    }

    function clearSelected(theList) {
        theList = theList || that._list;
        theList.find('input').removeAttr('checked');
    }

    function clearItems(theList) {
        theList = theList || that._list;
        theList.empty();
    }

    function findInput(value, theList) {
        theList = theList || that._list;
        return theList.find('li input[value="' + value + '"]:first');
    }

    function initScrollbar(theList) {
        theList = theList || that._list;
        theList.addClass('module-scrollbar');
    }

    // abstract methods
    function getItems() { throw new Error("Not implemented."); }
    function bindItems(data) { throw new Error("Not implemented."); }
    function getSelectedIds() { throw new Error("Not implemented."); };
    function countSelected() { throw new Error("Not implemented."); }
    function searchByValue() { throw new Error("Not implemented."); }

    // protected
    return {
        bindAction: function (eventName, selector, action, callback, preinit) {
            that._elem.on(eventName, selector, function (x) {
                if (preinit) preinit.call(null, x);
                eval(action)(that, x.target);
                if (callback) callback.call(null, x);
                x.stopPropagation();
            });
        },
        bindDataProvider: function (eventName, selector, provider) {
            that._elem.on(eventName, selector, function (x) {
                that._list.show();
                eval(provider)(that);
                x.stopPropagation();
            });
        },

        // TODO ->> [Temporary] move out to an intermediate base class for autocomplete
        bindDataProviderUsingAutocomplete: function (eventName, selector, provider, theList, emptyValueCallback) {
            theList = theList || that._list;

            that._elem.on(eventName, selector, function (x) {
                var phrase = that.getSearchPhrase().trim();
                var phraseCount = phrase.length;

                if (phraseCount > 0 && phraseCount < 3) return;

                // empty search value - clear items and don't trigger the event!
                if (that._EmptyIfNoPhrase && phraseCount == 0) {
                    that.clearItems(theList);

                    if (emptyValueCallback) emptyValueCallback.call(null, x);
                    return;
                }

                theList.show();
                eval(provider)(that);
                x.stopPropagation();
            });
        },

        initCompleted: function () {
            var initProvider = that._elem.attr('data-init-provider');
            if (initProvider)
                eval(initProvider)(that);

            if (that._isDisabled)
                that.disableItems();
        }
    }
}

// [ISSUE] DO NOT use new iListModule -> issue with instanceof clause
autoCompleteModule.prototype = Object.create(iListModule.prototype);
checklistModule.prototype = Object.create(iListModule.prototype);
checklistAutoCompleteModule.prototype = Object.create(iListModule.prototype);
radiolistModule.prototype = Object.create(iListModule.prototype);
checklistAddToListModule.prototype = Object.create(iListModule.prototype);

// [module] freeSearchModule
function freeSearchModule(elem) {
    var that = this; // reference

    var protected = iListModule.call(that, elem); // base

    // properties
    this._phraseInput = null;

    // override methods    
    this.setSelected = setSelected;
    this.getSelected = getSelected;
    this.isPopulated = isPopulated;

    // public methods
    this.getSearchPhrase = getSearchPhrase;

    // initialization
    (function (protected) {
        that._elem.addClass('freeSearch-module');

        that._phraseInput = that._elem.find('input[type="text"]:eq(0)');

        var action = that._elem.attr('data-action');
        if (action) protected.bindAction('change', that._phraseInput, action);

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
    function setSelected(data) {
        that._phraseInput.val(data[0] || '');
    }

    function getSelected() {
        return [that.getSearchPhrase()];
    }

    function isPopulated() {
        return that.getSearchPhrase() != '';
    }

    function getSearchPhrase() {
        return that._phraseInput.val().trim();
    }
}

// [module] numericRangeModule
function numericRangeModule(elem) {
    var that = this; // reference

    var protected = iRangeModule.call(that, elem); // base

    // override methods
    this.setSelected = setSelected;
    this.getSelected = getSelected;
    this.isPopulated = isPopulated;

    // initialization
    (function (protected) {
        that._elem.addClass('numeric-range-module');

        that._leftBox = that._elem.find('input[type="number"]:eq(0)');
        that._rightBox = that._elem.find('input[type="number"]:eq(1)');

        protected.bindBoxesOnlyNumbersAction();

        var action = that._elem.attr('data-action');
        if (action) protected.bindAction('change', 'input[type="number"]', action);
    })(protected);

    // methods
    function setSelected(data) {
        if (!(data instanceof Array))
            return;

        var values = data[0].split('-');
        that._leftBox.val(values[0] || '');
        that._rightBox.val(values[1] || '');
    }

    function getSelected() {
        var range = that.getRange();
        return [''.concat(range.min, '-', range.max)];
    }

    function isPopulated() {
        var range = that.getRange();
        return range.min != '' || range.max != '';
    }
}

// [module] autoCompleteModule
function autoCompleteModule(elem) {
    var that = this; // reference

    var protected = iListModule.call(that, elem); // base

    // properties
    this._phraseInput = null;

    // override methods
    this.getItems = getItems;
    this.setSelected = setSelected;
    this.getSelected = getSelected;
    this.isPopulated = isPopulated;
    this.bindItems = bindItems;

    // public methods
    this.getSearchPhrase = getSearchPhrase;
    this.clearSearchPhrase = clearSearchPhrase;

    // initialization
    (function (protected) {
        that._elem.addClass('autocomplete-module');

        that._phraseInput = that._elem.find('input[type="text"]:eq(0)');

        var action = that._elem.attr('data-action');

        var provider = that._elem.attr('data-provider');
        if (provider) protected.bindDataProviderUsingAutocomplete('keyup', that._phraseInput, provider, null, function (x) {
            if (action) eval(action)(that);
        });

        that._list.on('click', 'li', function () {
            that._phraseInput.val($(this).text());
            that._list.hide();

            if (action) eval(action)(that);
        });

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
    function getItems() {
        // [ string ]
        var items = that._list.find('li');
        return items.toArray().map(function (x) { return $(x).text().trim(); });
    }

    function setSelected(data) {
        that._phraseInput.val(data[0] || '');
        that._phraseInput.trigger('keyup');
    }

    function getSelected() {
        return [that.getSearchPhrase()];
    }

    function isPopulated() {
        return that.getSearchPhrase() != '';
    }

    function bindItems(data) {
        // data [ string ]

        that.clearItems();

        if (!(data instanceof Array))
            return;

        for (i in data)
            that._list.append('<li>' + data[i] + '</li>');
    }

    function getSearchPhrase() {
        return that._phraseInput.val().trim();
    }

    function clearSearchPhrase() {
        that._phraseInput.val('');
        that._phraseInput.trigger('keyup');
    }
}

// [module] checklistModule
function checklistModule(elem) {
    var that = this; // reference

    var protected = iListModule.call(that, elem); // base

    // override methods
    this.getItems = getItems;
    this.setSelected = setSelected;
    this.setSelected = setSelected;
    this.getSelected = getSelected;
    this.isPopulated = isPopulated;
    this.bindItems = bindItems;
    this.getSelectedIds = getSelectedIds;
    this.countSelected = countSelected;

    // initialization
    (function (protected) {
        that._elem.addClass('checklist-module');

        var action = that._elem.attr('data-action');
        if (action) protected.bindAction('click', '> ul > li input', action, null, function (x) {

            // TODO [Temporary]
            let selectAllValue = '0A8E7073D5AD19A4';
            if (x.target.value == selectAllValue) {
                // select All
                that.clearSelected();
                x.target.checked = true;
            }
            else {
                // select Specific 
                var item = that.findInput(selectAllValue).get(0);
                if (item) item.checked = false;
            }
        });

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
    function getItems() {
        // [{ value, text, checked } ... ]
        var items = that._list.find('li');
        return items.toArray().map(function (x) {
            var input = $(x).find('input:checkbox');
            var label = $(x).find('label');
            return {
                value: input.val(),
                text: label.text().trim(),
                checked: input.attr('checked') ? true : false
            }
        });
    }

    function setSelected(data) {
        if (!(data instanceof Array))
            return;

        that.clearSelected();

        // [ISSUE] doesn't work!!
        // that._list.find('input:checkbox[value="' + data[i] + '"]').attr('checked', 'checked')

        for (i in data) {
            var input = that._list.find('input:checkbox[value="' + data[i] + '"]')[0];
            if (input) input.checked = true
        }
    }

    function getSelected() {
        return that.getSelectedIds();
    }

    function isPopulated() {
        return that.countSelected() > 0;
    }

    function bindItems(data) {
        // data [{ value, text, checked } ... ]

        that.clearItems();

        if (!(data instanceof Array))
            return;

        var template =
            '<li>' +
            '<label class="checkbox-container" title="{3}">{1}' +
            '<input value="{0}" type="checkbox" {2} />' +
            '<span class="checkmark"></span>' +
            '</label>' +
            '</li>';

        for (i in data) {
            var item = data[i];
            that._list.append(template
                .replace('{0}', item.value)
                .replace('{1}', item.text)
                .replace('{2}', item.checked ? 'checked' : '')
                .replace('{3}', item.text)
            );
        }
    }

    function getSelectedIds() {
        var selected = that._list.find('input[type="checkbox"]:checked');
        return selected.toArray().map(function (x) { return x.value; });
    }

    function countSelected() {
        var selected = that._list.find('input[type="checkbox"]:checked');
        return selected.size();
    }
}

// [module] radiolistModule
function radiolistModule(elem) {
    var that = this; // reference

    var protected = iListModule.call(that, elem); // base

    // override methods
    this.getItems = getItems;
    this.setSelected = setSelected;
    this.getSelected = getSelected;
    this.isPopulated = isPopulated;
    this.bindItems = bindItems;
    this.getSelectedIds = getSelectedIds;
    this.countSelected = countSelected;

    // initialization
    (function (protected) {
        that._elem.addClass('radiolist-module');

        var action = that._elem.attr('data-action');
        if (action) protected.bindAction('click', '> ul > li input', action);

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
    function getItems() {
        // [{ value, text, checked } ... ]
        var items = that._list.find('li');
        return items.toArray().map(function (x) {
            var input = $(x).find('input:radio');
            var label = $(x).find('label');
            return {
                value: input.val(),
                text: label.text().trim(),
                checked: input.attr('checked') ? true : false
            }
        });
    }

    function setSelected(data) {
        if (!(data instanceof Array))
            return;

        that.clearSelected();

        for (i in data) {
            var input = that._list.find('input:radio[value="' + data[i] + '"]')[0];
            if (input) input.checked = true
        }
    }

    function getSelected() {
        return that.getSelectedIds();
    }

    function isPopulated() {
        return that.countSelected() > 0;
    }

    function bindItems(data) {
        // data [{ value, text, checked, group } ... ]

        that.clearItems();

        if (!(data instanceof Array))
            return;

        // TODO name
        var template =
            '<li>' +
            '<label class="radio-container" title="{4}">{1}' +
            '<input value="{0}" type="radio" name="{3}" {2} />' +
            '<span class="checkmark"></span>' +
            '</label>' +
            '</li>';

        for (i in data) {
            var item = data[i];
            that._list.append(template
                .replace('{0}', item.value)
                .replace('{1}', item.text)
                .replace('{2}', item.checked ? 'checked' : '')
                .replace('{3}', item.group || 'default')
                .replace('{4}', item.text)
            );
        }
    }

    function getSelectedIds() {
        var selected = that._list.find('input[type="radio"]:checked');
        return selected.toArray().map(function (x) { return x.value; });
    }

    function countSelected() {
        var selected = that._list.find('input[type="radio"]:checked');
        return selected.size();
    }
}

// [module] checklistAutoCompleteModule
function checklistAutoCompleteModule(elem) {
    var that = this; // reference

    var protected = iListModule.call(that, elem); // base

    // properties
    this._phraseInput = null;
    this._remoteContent = false;

    // public methods
    this.getSearchPhrase = getSearchPhrase;
    this.searchItems = searchItems;

    // override methods
    this.getItems = getItems;
    this.setSelected = setSelected;
    this.getSelected = getSelected;
    this.isPopulated = isPopulated;
    this.bindItems = bindItems;
    this.getSelectedIds = getSelectedIds;
    this.countSelected = countSelected;

    // initialization
    (function (protected) {
        that._elem.addClass('checklist-autocomplete-module');

        that._phraseInput = that._elem.find('input[type="text"]:eq(0)');

        var provider = that._elem.attr('data-provider');
        if (provider) {
            that._remoteContent = true;
            protected.bindDataProviderUsingAutocomplete('keyup', that._phraseInput, provider);
        }
        else {
            that._remoteContent = false;
            that._elem.on('keyup', that._phraseInput, that.searchItems);
        }

        var action = that._elem.attr('data-action');
        if (action) protected.bindAction('click', '> ul > li input', action);

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
    function getItems() {
        // [{ value, text, checked } ... ]
        var items = that._list.find('li');
        return items.toArray().map(function (x) {
            var input = $(x).find('input:checkbox');
            var label = $(x).find('label');
            return {
                value: input.val(),
                text: label.text().trim(),
                checked: input.attr('checked') ? true : false
            }
        });
    }

    function setSelected(data) {
        if (!(data instanceof Array))
            return;

        that.clearSelected();

        for (i in data) {
            var input = that._list.find('input:checkbox[value="' + data[i] + '"]')[0];
            if (input) input.checked = true
        }
    }

    function getSelected() {
        return that.getSelectedIds();
    }

    function isPopulated() {
        return that.countSelected() > 0;
    }

    function bindItems(data) {
        // data [{ value, text, checked } ... ]

        that.clearItems();

        if (!(data instanceof Array))
            return;

        var template =
            '<li>' +
            '<label class="checkbox-container">{1}' +
            '<input value="{0}" type="checkbox" {2} />' +
            '<span class="checkmark"></span>' +
            '</label>' +
            '</li>';

        for (i in data) {
            var item = data[i];
            that._list.append(template
                .replace('{0}', item.value)
                .replace('{1}', item.text)
                .replace('{2}', item.checked ? 'checked' : '')
            );
        }
    }

    function searchItems() {
        var phrase = that.getSearchPhrase().trim();
        var phraseCount = phrase.length;

        // empty search value - clear items and don't trigger the event!
        if (that._EmptyIfNoPhrase && phraseCount == 0) {
            that.hideItems();
            return;
        }

        that.showItems();
        var matches = that._list.find('li > label').toArray().filter(function (x) { !($(x).text().toLowerCase().includes(phrase.toLowerCase())); });
        matches.map(function (x) { $(x).closest('li').hide(); });
    }

    function getSelectedIds() {
        var selected = that._list.find('input[type="checkbox"]:checked');
        return selected.toArray().map(function (x) { return x.value; });
    }

    function countSelected() {
        var selected = that._list.find('input[type="checkbox"]:checked');
        return selected.size();
    }

    function getSearchPhrase() {
        return that._phraseInput.val().trim();
    }
}

// [module] checklistAddToListModule
function checklistAddToListModule(elem) {
    var that = this; // reference

    var protected = iListModule.call(that, elem); // base

    // properties
    this._phraseInput = null;
    this._selectedList = null;

    // public methods
    this.getSearchPhrase = getSearchPhrase;
    this.clearSearchPhrase = clearSearchPhrase;
    this.addToList = addToList;
    this.removeFromList = removeFromList;

    // override methods
    this.getItems = getItems;
    this.setSelected = setSelected;
    this.getSelected = getSelected;
    this.isPopulated = isPopulated;
    this.bindItems = bindItems;
    this.getSelectedIds = getSelectedIds;
    this.countSelected = countSelected;
    this.searchByValue = searchByValue;

    // initialization
    (function (protected) {
        that._elem.addClass('checklist-addToList-module');

        // 2nd ul (_autoCompleteList) is not exists - add it
        var autoCompleteDiv = that._elem.find('div');
        var ulExists = autoCompleteDiv.find('ul').length > 0;
        that._autoCompleteList = ulExists ? autoCompleteDiv.find('ul:first') : $('<ul />');
        autoCompleteDiv.append(that._autoCompleteList);

        that.initScrollbar(that._autoCompleteList);

        that._phraseInput = that._elem.find('input[type="text"]:eq(0)');

        var provider = that._elem.attr('data-provider');
        if (provider) protected.bindDataProviderUsingAutocomplete('keyup', that._phraseInput, provider, that._autoCompleteList);

        var action = that._elem.attr('data-action');

        protected.bindAction('click', '> ul > li input', function (m, x) {
            that.removeFromList(x);

            // user action
            if (action) eval(action)(that, this);
        });

        that._autoCompleteList.on('click', 'li', function () {
            that.addToList({ text: $(this).text(), value: $(this).find('input').val() });
            that._autoCompleteList.hide();

            // user action
            if (action) eval(action)(that, this);
        });

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
    function getItems() {
        // [{ value, text, checked } ... ]
        var items = that._list.find('li');
        return items.toArray().map(function (x) {
            var input = $(x).find('input:checkbox');
            var label = $(x).find('label');
            return {
                value: input.val(),
                text: label.text().trim(),
                checked: input.attr('checked') ? true : false
            }
        });
    }

    function setSelected(data) {
        if (!(data instanceof Array))
            return;

        that.clearSelected();

        for (i in data) {
            var input = that._list.find('input:checkbox[value="' + data[i] + '"]')[0];
            if (input) input.checked = true
        }
    }

    function getSelected() {
        return that.getSelectedIds();
    }

    function isPopulated() {
        return that.countSelected() > 0;
    }

    function addToList(item) {
        // { text, value }

        var existsItem = that.searchByValue(item.value);
        if (existsItem) return;

        var template =
            '<li>' +
            '<label class="checkbox-container">{1}' +
            '<input value="{0}" type="checkbox" checked />' +
            '<span class="checkmark"></span>' +
            '</label>' +
            '</li>';

        that._list.append(template
            .replace('{0}', item.value)
            .replace('{1}', item.text)
        );
    }

    function removeFromList(item) {
        // item: checkbox
        if (item.checked) return;
        $(item).closest('li').remove();
    }

    function getSearchPhrase() {
        return that._phraseInput.val().trim();
    }

    function clearSearchPhrase() {
        that._phraseInput.val('');
        that._phraseInput.trigger('keyup');
    }

    function bindItems(data) {
        // data [ { text, value } ... ]

        that.clearItems(that._autoCompleteList);

        if (!(data instanceof Array))
            return;

        for (i in data) {
            let item = data[i];
            that._autoCompleteList.append('<li>' + item.text + '<input type="hidden" value="' + item.value + '" /></li>');
        }
    }

    function getSelectedIds() {
        var selected = that._list.find('input[type="checkbox"]:checked');
        return selected.toArray().map(function (x) { return x.value; });
    }

    function countSelected() {
        var selected = that._list.find('input[type="checkbox"]:checked');
        return selected.size();
    }

    function searchByValue(value) {
        var selector = that._list.find('input[type="checkbox"][value="' + value + '"]');
        return selector.size() == 0 ? null : selector.first();
    }
}

/******* Manager *******
************************/

// TODO should i move that into the manager ?!

// query expansion
jQuery.fn.extend({
    loadMAModules: function () {
        var arr = [];

        $(this).find('[data-module]').each(function (i, x) {
            var moduleType = $(x).attr('data-module').toLowerCase();
            switch (moduleType) {
                case 'numeric-range': arr.push(new numericRangeModule(x));
                    break;
                case 'autocomplete': arr.push(new autoCompleteModule(x));
                    break;
                case 'checklist': arr.push(new checklistModule(x));
                    break;
                case 'checklist-autocomplete': arr.push(new checklistAutoCompleteModule(x));
                    break;
                case 'radiolist': arr.push(new radiolistModule(x));
                    break;
                case 'checklist-addtolist': arr.push(new checklistAddToListModule(x));
                    break;
                case 'freesearch': arr.push(new freeSearchModule(x));
                    break;
            }
        });

        return arr;
    },
    findMAModule: function (arrModules) {
        // get by DOM reference

        for (i in arrModules)
            if (arrModules[i].equal(this[0]))
                return arrModules[i];
        return null;
    },
    collapseAll: function (arrModules) {

        $(this).each(function (i, x) {
            var elem = $(x).first();

            var arr = [];

            elem.find('[data-module]').each(function (i, x) {
                var module = $(x).findMAModule(arrModules);
                if (!module) return;
                arr.push(module);
            });

            // console.log(arr.length);
            var isAnyonePopulated = false;
            for (i in arr)
                if (arr[i] instanceof iModule)
                    if (arr[i].isPopulated()) {
                        isAnyonePopulated = true;
                        break;
                    }

            // console.log(totalSelected);

            if (isAnyonePopulated)
                elem.removeClass('collapsed');
            else
                elem.addClass('collapsed');

            // for (i in arr) arr[i].hide();
        });

    }
});

// --------------------------------

// helper
var modulesManagerHelper = function () {
    return {
        // query 2 dictionary
        // { key, value }
        parseParams: function () {
            var result = [];

            var query = document.location.search.replace('?', '');
            if (query.trim() == 0)
                return result;

            query = decodeURIComponent(query);

            var arr = query.split('&');
            for (i in arr) {
                var param = arr[i].split('=');
                result[param[0]] = param[1];
            }

            return result;
        },

        // dictionary 2 query
        // { key, value }
        createQuery: function (dic) {
            var res = this.dic2arr(dic).reduce(function (s, x) {
                return s.concat('&', x.key, '=', x.value.join('_'));
            }, '');

            // replace first & to ? mark
            if (res.indexOf('&') == 0)
                res = '?'.concat(res.slice(1));

            return res;
        },
        dic2arr: function (dic) {
            var arr = [];

            for (i in dic)
                arr.push({ key: i, value: dic[i] });

            return arr;
        }
    }
}

// --------------------------------

// modules manager
var modulesManagerService = function () {
    var that = this; // reference

    this._modules = [];
    this._helper = null;

    // initialization
    (function () {
        that._helper = new modulesManagerHelper();
    })();

    return {
        modules: function () {
            return that._modules
        },
        count: function () {
            return that._modules.length;
        },
        loadQuery: function () {
            let dicPrms = that._helper.parseParams();
            console.log(dicPrms);

            for (i in that._modules) {
                let current = that._modules[i];
                if (!current._queryName) continue;

                // get raw query value (before splitting it)
                let raw = dicPrms[current._queryName];
                if (!raw) continue;

                // get values collection
                let values = raw.split('_');
                current.setSelected(values);
            }

        },
        setQuery: function (extraQuery) {
            let dicPrms = {};

            for (i in that._modules) {
                let current = that._modules[i];
                if (!current._queryName || !current.isPopulated()) continue;

                dicPrms[current._queryName] = current.getSelected();
            }

            let query = that._helper.createQuery(dicPrms);

            if (extraQuery)
                query = query.concat('&', extraQuery);

            if (query.indexOf('?') == -1 && query.indexOf('&') > -1)
                query = query.replace('&', '?');

            console.log(query);

            var newURI = ''.concat(document.location.protocol, '//', window.location.host, window.location.pathname, query)
            window.history.replaceState({ urlPath: newURI }, "", newURI);
        },
        loadModules: function () {
            // modules
            that._modules = $(document).loadMAModules();
        },
        loadCollapsableTitles: function () {
            // collapsable titles
            let selector = $('.collapsable-title');

            // onload - ignore those with data-ignore-collapse attribute
            selector.not('[data-ignore-collapse]').collapseAll(that._modules);

            let header = selector.find('h4:first');
            header.click(function () {
                $(this).parent().toggleClass('collapsed');
            });
        },
        loadALL: function () {
            this.loadModules();
            this.loadQuery();
            this.loadCollapsableTitles();
            this.registerEvents();
        },
        registerEvents: function () {
            return;
            // any click on the page should clear the autocomplete values state
            $('body').click(function (x) {
                for (i in that._modules) {
                    let current = that._modules[i];
                    if (current instanceof autoCompleteModule || current instanceof checklistAddToListModule) {
                        current.clearSearchPhrase();
                        x.stopPropagation();
                    }
                }
            });
        }
    };
}
