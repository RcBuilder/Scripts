var module = angular.module('my-app', []);

// --- Controllers ------------------

module.controller('homeController', function ($scope, serializer, common, launcher, oauth) {

    // { id, title, startDate, dueDate, callbackUrl, iconURL, scoreType, scorePossible, location }
    $scope.assignments = [];
    $scope.jsonResult = '[]';
    
    // select inputs
    $scope.options_toolId = ['Openlearning', 'Simnet', 'Learnsmart'];
    $scope.options_lmsId = ['Blackboard', 'Openlearning'];
    $scope.options_scoreType = ['Points', 'Percentage', 'Text', 'Other'];

    $scope.toolId = $scope.options_toolId[0];
    $scope.lmsId = $scope.options_lmsId[0];
    $scope.scoreType = $scope.options_scoreType[0];

    // methods
    $scope.Add = function () {
        var assignment;

        // fix dates 
        var startDate = $scope.newAssignment.startDate;
        if (startDate)
            startDate = new Date(startDate);

        var dueDate = $scope.newAssignment.dueDate;
        if (dueDate)
            dueDate = new Date(dueDate);

        if ($scope.chkGradable) {
            assignment = new Assignment({
                id: $scope.newAssignment.id,
                title: $scope.newAssignment.title,
                callbackUrl: $scope.newAssignment.callbackUrl,
                startDate: startDate,
                dueDate: dueDate,
                iconURL: $scope.newAssignment.iconURL,
                location: $scope.newAssignment.location,
                scoreType: $scope.scoreType,
                scorePossible: $scope.newAssignment.scorePossible                
            });
        }
        else {
            assignment = new LinkItem({
                id: $scope.newAssignment.id,
                title: $scope.newAssignment.title,
                callbackUrl: $scope.newAssignment.callbackUrl,
                startDate: startDate,
                dueDate: dueDate,
                iconURL: $scope.newAssignment.iconURL,
                location: $scope.newAssignment.location,
            });
        }

        if (!assignment) return;

        $scope.assignments.push(assignment);
        $scope.Generate();
    }

    $scope.Remove = function (index) {
        $scope.assignments.splice(index, 1);
        $scope.Generate();
    }

    $scope.Generate = function () {        
        var graph = launcher.toGraphArray($scope.assignments);
        $scope.jsonResult = serializer.serialize(graph, $scope.chkPrettyJson);
    }

    $scope.Launch = function () {
        var postData = {};

        var data = new CISData($scope.returnURL, $scope.customerId, $scope.lmsId, $scope.toolId, $scope.recordId, $scope.contextId);

        var contentItems = launcher.toContentItems($scope.assignments);
        
        postData['lti_version'] = 'LTI-1p0';
        postData['lti_message_type'] = 'ContentItemSelection';
        postData['lti_msg'] = '';
        postData['lti_errormsg'] = '';

        postData['data'] = serializer.encodedJson(data);
        postData['content_items'] = serializer.encodedJson(contentItems);

        var host = $scope.host || 'http://mhaairs-aws-qa.tegrity.com'; // 'http://localhost/MHCampus'
        var launchURL = host.concat('/LTIHandlers/LTIContentItem.ashx');

        // generate an oauth signature
        var signature = oauth.generate(launchURL, postData, 'FEA488CB1A4844F7B6899DA8770126E3');
        postData['oauth_signature'] = signature;        

        launcher.launch(launchURL, postData);
    }
});

// --- Filters ------------------

module.filter('identity', function () {
    return function (value) {
        return '#'.concat(value);
    };
});

module.filter('date', function () {
    return function (value) {
        var year = value.getFullYear().toString();
        var month = (value.getMonth() + 1).toString();
        var day = value.getDate().toString();

        if (month.length == 1) month = '0' + month; // return yyyyMMdd
        if (day.length == 1) day = '0' + day;

        return year + '-' + month + '-' + day;
    };
});

// --- Tools ------------------

// https://github.com/bettiolo/oauth-signature-js
module.factory('oauth', function () {
    return {
        // postData { key: value, key: value ... }
        generate: function (url, postData, consumerSecret) {
            // add oauth params as post data
            postData['oauth_version'] = '1.0';
            postData['oauth_consumer_key'] = 'simnet';
            postData['oauth_signature_method'] = 'HMAC-SHA1';
            postData['oauth_token'] = '1234';
            postData['oauth_timestamp'] = Math.floor((new Date).getTime() / 1e3);
            postData['oauth_nonce'] = Math.floor(Math.random() * 1e9).toString();

            var signature = oauthSignature.generate('POST', url, postData, consumerSecret, null, { encodeSignature: false });

            console.log(new oauthSignature.SignatureBaseString('POST', url, postData).generate());
            console.log(signature);

            return signature;
        }
    }
});

module.factory('serializer', function () {
    return {
        serialize: function (obj, pretty) {
            pretty = pretty || false; // true for a pretty json view instead of a single line
            return angular.toJson(obj, pretty);
        },
        encodedJson: function (obj) {            
            return encodeURIComponent(JSON.stringify(obj));
        }
    }
});

module.factory('launcher', function () {
    return {
        // postData { key: value, key: value ... }
        launch: function (launchURL, postData) {
            var form = angular.element('<form method="POST", action="{0}" target="_blank">'.replace('{0}', launchURL));

            for (var p in postData) {                
                var input = angular.element('<input type="hidden" name="{0}" value="{1}" />'.replace('{0}', p).replace('{1}', postData[p]));
                form.append(input);
            }

            angular.element('body').append(form);
            form.submit();
            angular.element('body').remove(form);
        },
        toContentItems: function (assignments) {
            return {
                '@context': 'http://purl.imsglobal.org/ctx/lti/v1/ContentItem',
                '@graph': this.toGraphArray(assignments)
            };
        },
        toGraphArray: function (assignments) {
            var graph = [];
            for (var i = 0; i < assignments.length; i++)
                graph.push(this.toGraphItem(assignments[i]));
            return graph;
        },
        toGraphItem: function (assignment) {
            var graphItem = {
                '@id': assignment.id,
                '@type': 'LtiLinkItem',
                mediaType: 'application/vnd.ims.lti.v1.ltilink',
                url: '',
                title: assignment.title,
                text: assignment.title,
                'mhe:callback_url': assignment.callbackUrl,
                custom: {},
                icon: {
                    URL: ''
                },
                placementAdvice: {
                    presentationDocumentTarget: 'window', // EMBED, FRAME, IFRAME, WINDOW, POPUP, OVERLAY
                    windowTarget: 'anLTIApp',
                    'mhe:path': assignment.location
                },
                available: {
                    startDatetime: assignment.startDate,  //'2016-11-07T00:00:00Z'
                },
                submission: {
                    startDatetime: assignment.startDate,  //'2016-11-07T00:00:00Z'
                    endDatetime: assignment.dueDate
                }
            };

            // not exists for NON-Gradable items
            if (assignment.isGradable()) {
                graphItem.lineItem = {
                    '@type': "LineItem",
                    label: '',
                    reportingMethod: assignment.scoreType, // 'res:totalScore'
                    assignedActivity: {
                        id: '',
                        activity_id: ''
                    },
                    scoreConstraints: {
                        '@type': 'NumericLimits',
                        totalMaximum: assignment.scorePossible
                    }
                }
            };

            return graphItem;
        }
    }
});

module.factory('common', function () {
    return {
        replace: function (input, oldValue, newValue) {
            return input.split(oldValue).join(newValue)
        }
    }
});

// --- Entities ------------------

function CISData(returnURL, customerId, lmsId, toolId, recordId, contextId) {
    this.originalData = '';
    this.returnURL = returnURL || '';
    this.customerId = customerId || '';
    this.lms = lmsId || '';
    this.toolId = toolId || '';
    this.originalRecordId = recordId || '';
    this.contextId = contextId || '';
}

function LinkItem(params) {
    var nextWeek = new Date();
    nextWeek.setDate(nextWeek.getDate() + 7);

    var defaults = { startDate: new Date(), dueDate: nextWeek };

    this.id = params.id || '';
    this.title = params.title || '';
    this.callbackUrl = params.callbackUrl || '';
    this.startDate = params.startDate || defaults.startDate;
    this.dueDate = params.dueDate || defaults.dueDate;
    this.location = params.location || '';
}
LinkItem.prototype.isGradable = function () {
    return this.scoreType != undefined;
}

function Assignment(params) {
    var defaults = { scoreType: '', scorePossible: 100 };

    LinkItem.call(this, params); // call base constractor (Assignment object is the caller - this)
    this.scoreType = params.scoreType || defaults.scoreType;
    this.scorePossible = params.scorePossible || defaults.scorePossible;
}
Assignment.prototype = new LinkItem({});

/*
var l = new LinkItem({});
var a = new Assignment({});
    
alert(l.isGradable()); // false
alert(a.isGradable()); // true


<select ng-options="opt as opt.text for opt in [{ id: 4, text: '444' }, { id: 2, text: '222' }, { id: 1, text: '111' }] track by opt.id" ng-model="ddlTest2" />                        
<select ng-options="opt as opt for opt in ['aaa','bbb','ccc']" ng-model="ddlTest" />     
*/