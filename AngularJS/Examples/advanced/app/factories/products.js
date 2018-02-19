function products_factory() {
    var products = { "products": [
	    { "id": 1, "name": "Samsung 4 Phone", "price": 930.00, "colors": [ "red", "white" ] },
	    { "id": 2, "name": "Samsung 3 Phone", "price": 690.00, "colors": [ "green", "white", "black" ] },
	    { "id": 3, "name": "Charger", "price": 40.50, "colors": [ "black", "blue" ] },
        { "id": 4, "name": "Case", "price": 1.99, "colors": [ "black", "purple", "pink" ] },
        { "id": 5, "name": "Charger 2 modes", "price": 70.99, "colors": [ "black" ] },
        { "id": 6, "name": "Samsung 1 Phone", "price": 265.80, "colors": [ "green", "white", "blue", "brown" ] }
    ]};
    
    var factory = {};

    factory.get_products = function (onSuccess, onError) {
        try{ onSuccess(products); }
        catch(e) { }
    };

    return factory;
}