function users_factory() {
    var users = [
			{ name: 'RON', age: 39 },
			{ name: 'LITAL', age: 26 },
			{ name: 'EREZ', age: 35 },
			{ name: 'MAAYAN', age: 34 },
			{ name: 'RICKI', age: 28 },
            { name: 'ROBY', age: 35 },
			{ name: 'AVI', age: 33 },
            { name: 'OFIR', age: 41 },
			{ name: 'SHIRLY', age: 36 }            
		];

	    var factory = {};
	    factory.get_users = function () {
	        return users;
	    };
	    factory.add_user = function (user) {
	        users.push(user);
	    };
		return factory;
}