// return only products contains 'Samsung' phrase
function isSamsung_filter() {
    return function (products) {
        var filtered = [];
        for (var i = 0; i < products.length; i++) {
            var product = products[i];
            if (product.name.toLowerCase().indexOf('samsung') != -1)
                filtered.push(product);
        }
        return filtered;

    };
};

// return only products Starts With the specified value
function productStartsWith_filter() {
    return function (products, param) {
        if (param == undefined || param == '') return products;

        var filtered = [];
        for (var i = 0; i < products.length; i++) {
            var product = products[i];
            if (product.name.toLowerCase().indexOf(param) == 0)
                filtered.push(product);
        }
        return filtered;

    };
};

// return only products Starts With the specified value
function productNameLengthRange_filter() {
    return function (products, param1, param2) {
        // undefined = first execution, empty = no selected value
        if (param1 == undefined || param2 == undefined) return products; 

        var filtered = [];
        for (var i = 0; i < products.length; i++) {
            var product = products[i];
            if (product.name.length >= param1 && product.name.length <= param2)
                filtered.push(product);
        }
        return filtered;

    };
};