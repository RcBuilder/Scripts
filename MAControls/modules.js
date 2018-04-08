
/******* Modules *******
************************/

function iModule(elem) {
    var that = this; // reference

    // properties
    this._elem = $(elem);
    this._queryName = null;

    // public methods
    this.equal = equal;
    this.hide = hide;
    this.isPopulated = isPopulated;

    // initialization
    (function () {
        that._queryName = that._elem.attr('data-query-name') || null;
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
        return {
            min: that._leftBox.val(),
            max: that._rightBox.val()
        };
    };

    // protected
    return {
        bindAction: function (eventName, selector, action) {
            that._elem.on(eventName, selector, x => eval(action)(that));
        }
    };
}

numericRangeModule.prototype = Object.create(iRangeModule.prototype);

function iListModule(elem) {
    var that = this; // reference

    iModule.call(that, elem); // base

    // properties
    this._list = null;
    this._fullIfNoPhrase = false;

    // public methods
    this.clearItems = clearItems;
    this.hideItems = hideItems;
    this.showItems = showItems;
    this.clearSelected = clearSelected;
    this.bindItems = bindItems;
    this.getSelectedIds = getSelectedIds;
    this.countSelected = countSelected;

    // initialization
    (function () {
        var ulExists = that._elem.find('ul').length > 0;
        that._list = ulExists ? that._elem.find('ul:first') : $('<ul />');
        that._list.addClass('module-scrollbar');

        var listHeight = that._elem.attr('data-list-height');
        if (listHeight) that._list.css('height', listHeight);

        that._elem.append(that._list);

        var fullIfNoPhrase = that._elem.attr('data-full-list-if-no-phrase') || '0';
        that._fullIfNoPhrase = fullIfNoPhrase && fullIfNoPhrase == '1';
    })();

    // methods
    function clearItems() {
        that._list.empty();
    }

    function hideItems() {
        that._list.find('li').hide();
    }

    function showItems() {
        that._list.find('li').show();
    }

    function clearSelected() {
        that._list.find('input').removeAttr('checked');
    }

    // abstract methods
    function bindItems(data) { throw new Error("Not implemented."); }
    function getSelectedIds() { throw new Error("Not implemented."); };
    function countSelected() { throw new Error("Not implemented."); }

    // protected
    return {
        bindAction: function (eventName, selector, action) {
            that._elem.on(eventName, selector, x => {
                eval(action)(that);
                x.stopPropagation();
            });
        },
        bindDataProvider: function (eventName, selector, provider) {
            that._elem.on(eventName, selector, x => {
                that._list.show();

                // empty search value - clear items and don't trigger the event!
                if (!that._fullIfNoPhrase && that.getSearchPhrase().trim() == '') {
                    that.clearItems();
                    return;
                }

                eval(provider)(that);
                x.stopPropagation();
            });
        },
        initCompleted: function () {
            var initProvider = that._elem.attr('data-init-provider');
            if (initProvider)
                eval(initProvider)(that);
        }
    }
}

// [ISSUE] DO NOT use new iListModule -> issue with instanceof clause
autoCompleteModule.prototype = Object.create(iListModule.prototype);
checklistModule.prototype = Object.create(iListModule.prototype);
checklistAutoCompleteModule.prototype = Object.create(iListModule.prototype);
radiolistModule.prototype = Object.create(iListModule.prototype);


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

        var action = that._elem.attr('data-action');
        if (action) protected.bindAction('change', 'input[type="number"]', action);
    })(protected);

    // methods
    function setSelected(data) {
        if (!(data instanceof Array))
            return;

        that._leftBox.val(data[0] || '');
        that._rightBox.val(data[1] || '');
    }

    function getSelected() {
        var range = that.getRange();
        return [range.min, range.max];
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
    this.setSelected = setSelected;
    this.getSelected = getSelected;
    this.isPopulated = isPopulated;
    this.bindItems = bindItems;

    // public methods
    this.getSearchPhrase = getSearchPhrase;

    // initialization
    (function (protected) {
        that._elem.addClass('autocomplete-module');

        that._phraseInput = that._elem.find('input[type="text"]:eq(0)');

        var provider = that._elem.attr('data-provider');
        if (provider) protected.bindDataProvider('keyup', that._phraseInput, provider);

        that._list.on('click', 'li', function () {
            that._phraseInput.val($(this).text());
            that._list.hide();
        });

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
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
}

// [module] checklistModule
function checklistModule(elem) {
    var that = this; // reference

    var protected = iListModule.call(that, elem); // base

    // override methods
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
        if (action) protected.bindAction('click', 'ul > li input', action);

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
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

    function getSelectedIds() {
        var selected = that._list.find('input[type="checkbox"]:checked');
        return selected.toArray().map(x => x.value);
    }

    function countSelected() {
        var selected = that._list.find('input[type="checkbox"]:checked');
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
            protected.bindDataProvider('keyup', that._phraseInput, provider);
        }
        else {
            that._remoteContent = false;
            that._elem.on('keyup', that._phraseInput, that.searchItems);
        }

        var action = that._elem.attr('data-action');
        if (action) protected.bindAction('click', 'ul > li input', action);

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
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
        var phrase = that.getSearchPhrase();

        // empty search value - clear items and don't trigger the event!
        if (!that._fullIfNoPhrase && phrase == '') {
            that.hideItems();
            return;
        }

        that.showItems();
        var matches = that._list.find('li > label').toArray().filter(x => !($(x).text().includes(phrase)));
        matches.map(x => { $(x).closest('li').hide(); });
    }

    function getSelectedIds() {
        var selected = that._list.find('input[type="checkbox"]:checked');
        return selected.toArray().map(x => x.value);
    }

    function countSelected() {
        var selected = that._list.find('input[type="checkbox"]:checked');
        return selected.size();
    }

    function getSearchPhrase() {
        return that._phraseInput.val().trim();
    }
}

// [module] radiolistModule
function radiolistModule(elem) {
    var that = this; // reference

    var protected = iListModule.call(that, elem); // base

    // override methods
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
        if (action) protected.bindAction('click', 'ul > li input', action);

        protected.initCompleted(); // after fully loaded!
    })(protected);

    // methods
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
            '<label class="radio-container">{1}' +
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
            );
        }
    }

    function getSelectedIds() {
        var selected = that._list.find('input[type="radio"]:checked');
        return selected.toArray().map(x => x.value);
    }

    function countSelected() {
        var selected = that._list.find('input[type="radio"]:checked');
        return selected.size();
    }
}

/******* Manager *******
************************/

// TODO should i move that into the manager ?!

// query expansion
jQuery.fn.extend({
    loadMAModules: function () {
        var arr = [];

        $(this).find('[data-module]').each((i, x) => {
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

        $(this).each((i, x) => {
            var elem = $(x).first();

            var arr = [];

            elem.find('[data-module]').each((i, x) => {
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
            var res = this.dic2arr(dic).reduce((s, x) => {
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
};

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
        setQuery: function () {
            let dicPrms = {};

            for (i in that._modules) {
                let current = that._modules[i];
                if (!current._queryName || !current.isPopulated()) continue;

                dicPrms[current._queryName] = current.getSelected();
            }

            let query = that._helper.createQuery(dicPrms);

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
            selector.collapseAll(that._modules);

            let header = selector.find('h4:first');
            header.click(function () {
                $(this).parent().toggleClass('collapsed');
            });
        },
        loadALL: function () {
            this.loadModules();
            this.loadQuery();
            this.loadCollapsableTitles();
        }
    };
}

