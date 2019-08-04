var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator.throw(value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments)).next());
    });
};
var module = angular.module('my-app', []);
module.controller('homeController', function ($scope, contactsFactory) {
    $scope.contacts = [];
    $scope.isPopupMode = false;
    $scope.newContact = new Contact();
    $scope.contactProfiles = [
        'alex jonathan.jpg', 'janeth carton.jpg', 'john-smith.jpg', 'michael zimber.jpg'
    ];
    (function () {
        contactsFactory.getContacts(function (result) {
            $scope.contacts = result;
        });
    })();
    $scope.openContactPopup = function () {
        $scope.isPopupMode = true;
    };
    $scope.closeContactPopup = function () {
        $scope.isPopupMode = false;
    };
    $scope.saveContact = function () {
        if (!validateContact($scope.newContact))
            return;
        contactsFactory.saveContact($scope.newContact, function (result) {
            $scope.contacts = result;
            saveCompleted();
        }, function (error) {
            console.log('[ERROR] ' + error);
            saveCompleted();
        });
    };
    function saveCompleted() {
        $scope.closeContactPopup();
        $scope.newContact = new Contact();
        $scope.$digest();
    }
    $scope.addContact = function (index) {
        var contact = new Contact();
        contact.id = $scope.contacts.reduce(function (a, x) { return (a < x.id) ? x.id : a; }, 0) + 1;
        $scope.newContact = contact;
        $scope.openContactPopup();
    };
    $scope.editContact = function (index) {
        $scope.newContact = $scope.contacts[index];
        $scope.openContactPopup();
    };
    $scope.deleteContact = function (id) {
        contactsFactory.deleteContact(id, function (result) {
            $scope.contacts = result;
        });
    };
    function validateContact(contact) {
        var regex = new RegExp('^[0-9]+$');
        if (contact.image.trim() == '')
            contact.image = $scope.contactProfiles[0];
        if (contact.name.trim() == '' || contact.phone.trim() == '')
            return false;
        if (!regex.test(contact.phone))
            return false;
        return true;
    }
});
var Contact = (function () {
    function Contact(name, image, address, twitterLine1, twitterLine2, phone, position, id) {
        if (name === void 0) { name = ''; }
        if (image === void 0) { image = ''; }
        if (address === void 0) { address = ''; }
        if (twitterLine1 === void 0) { twitterLine1 = ''; }
        if (twitterLine2 === void 0) { twitterLine2 = ''; }
        if (phone === void 0) { phone = ''; }
        if (position === void 0) { position = ''; }
        if (id === void 0) { id = 0; }
        this.name = name;
        this.image = image;
        this.address = address;
        this.twitterLine1 = twitterLine1;
        this.twitterLine2 = twitterLine2;
        this.phone = phone;
        this.position = position;
        this.id = id;
    }
    return Contact;
}());
module.factory('contactsFactory', function ($http) {
    var googleAPI = 'https://maps.googleapis.com/maps/api/geocode/json?address={0}&key=AIzaSyDKvvBgAkSCugEbXckutuAFuqPzthsCnJ8';
    var contacts = [];
    (function () {
        var temp = JSON.parse(localStorage.getItem("contacts"));
        if (!temp || temp.length == 0) {
            contacts.push(new Contact('John Doe1', 'alex jonathan.jpg', 'alenbi tel aviv, israel', 'bla bla bla', 'bla bla bla', '054-5614020', 'developer', 1));
            contacts.push(new Contact('John Doe2', 'janeth carton.jpg', 'alenbi tel aviv, israel', 'bla bla bla', 'bla bla bla', '054-5614020', 'developer', 2));
            contacts.push(new Contact('John Doe3', 'john-smith.jpg', 'alenbi tel aviv, israel', 'bla bla bla', 'bla bla bla', '054-5614020', 'developer', 3));
            contacts.push(new Contact('John Doe4', 'michael zimber.jpg', 'alenbi tel aviv, israel', 'bla bla bla', 'bla bla bla', '054-5614020', 'developer', 4));
            Save();
        }
        else
            contacts = temp;
    })();
    contacts.findIndex = function (id) {
        if (this.length == 0)
            return -1;
        for (i in this)
            if (this[i].id == id)
                return i;
        return -1;
    };
    function Save() {
        localStorage.setItem("contacts", JSON.stringify(contacts));
    }
    return {
        getContacts: function (onSuccess, onFailure) {
            try {
                onSuccess(contacts);
            }
            catch (e) {
                onFailure(e);
            }
        },
        saveContact: function (contact, onSuccess, onFailure) {
            return __awaiter(this, void 0, void 0, function* () {
                try {
                    try {
                        var response = yield $http.get(googleAPI.replace('{0}', contact.address));
                        if (response.status == 'OK')
                            contact.coordinates = response.results.geometry.location;
                    }
                    catch (e) {
                        console.log(e);
                    }
                    var index = contacts.findIndex(contact.id);
                    if (index == -1)
                        contacts.push(contact);
                    else
                        contacts[index] = contact;
                    Save();
                    onSuccess(contacts);
                }
                catch (e) {
                    onFailure(e);
                }
            });
        },
        deleteContact: function (id, onSuccess, onFailure) {
            try {
                var index = contacts.findIndex(id);
                contacts.splice(index, 1);
                Save();
                onSuccess(contacts);
            }
            catch (e) {
                onFailure(e);
            }
        },
    };
});
//# sourceMappingURL=app.js.map