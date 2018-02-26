<%@ Page Language="C#" AutoEventWireup="true" %>

<!DOCTYPE html>

<html ng-app="my-app" lang="en" xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta charset="utf-8" />
    <title>CIS GENERATOR</title>
    <link rel="stylesheet" type="text/css" href="Styles/bootstrap.min.css"  />
    <link rel="stylesheet" type="text/css" href="Styles/cis_app_style.css" />
</head>
<body>
    <div ng-controller="homeController">
        <div id="main-wrapper">
            <div class="row">
                <div class="col-lg-2">
                    <h3>items</h3>
                                        
                    <div ng-include="'newAssignment.partial'"></div>
                    <div>
                        <input type="button" ng-click="Add()" value="Add" class="btn btn-primary" />                        
                    </div>                    
                </div>
                <div class="col-lg-2">
                    <h3>content items</h3>
                    <div>
                        <div class="colorGuide linkItem"></div>
                        linkItem
                        &nbsp;&nbsp;
                        <div class="colorGuide assignment"></div>
                        assignment
                    </div>
                    <ul>
                        <li ng-repeat="assignment in assignments">
                            <div ng-include="'assignment.partial'"></div>
                            <!--<ng-assignment value="assignment"></ng-assignment>-->
                        </li>
                    </ul>
                </div>
                <div class="col-lg-2">
                    <h3>launch</h3>                    
                    <div ng-include="'basicInput.partial'"></div>
                    <div>
                        <!-- ng-show="assignments.length > 0" -->
                        <input type="button" ng-click="Launch()" value="Launch" class="btn btn-primary" />
                    </div>
                </div>
                <div class="col-lg-6">
                    <h3>content items (json)</h3>
                    <div ng-include="'result.partial'"></div>
                    <div>
                        <input type="checkbox" ng-model="chkPrettyJson" />
                        pretty
                    </div>                    
                </div>
            </div>
        </div>       
    </div>

    <script type="text/ng-template" id="result.partial">
        <div class="result-wrapper">
            <textarea ng-model="$parent.jsonResult" class="form-control" />
        </div>
    </script>

    <script type="text/ng-template" id="assignment.partial">
        <div class="assignment-wrapper">
            <div ng-class="(assignment.isGradable()) ? 'assignment' : 'linkItem'">                
                <div>
                    {{ assignment.id | identity }}
                </div>
                <div>
                    {{ assignment.title }}
                </div>
                <div>
                    start date: {{ assignment.startDate | date }}
                </div>
                <div>
                    due date: {{ assignment.dueDate | date }}
                </div>
                <div>
                    callback: {{ assignment.callbackUrl }}
                </div>
                <div>
                   icon: {{ assignment.iconURL }}
                </div>
                <div>
                   location: {{ assignment.location }}
                </div>

                <div ng-show="assignment.isGradable()">
                    <div>
                        score type: {{ assignment.scoreType }}
                    </div>
                    <div>
                        score possible: {{ assignment.scorePossible }}
                    </div>
                </div>
                
                <input type="button" ng-click="Remove($index)" value="x" class="btn btn-xs btn-default btn-close" />                
            </div>
        </div>
    </script>

    <script type="text/ng-template" id="basicInput.partial">
        <div class="new-assignment-wrapper">
            <div>
                <input type="text" ng-model="$parent.host" placeholder="host" class="form-control" />
            </div>
            <div>
                <input type="text" ng-model="$parent.customerId" placeholder="customer Id" class="form-control" />
            </div>
            <div>
                <select ng-model="$parent.toolId" class="form-control" ng-options="opt as opt for opt in options_toolId" />       
            </div>
            <div>
                <select ng-model="$parent.lmsId" class="form-control" ng-options="opt as opt for opt in options_lmsId" />                   
                </select>                
            </div>
            <div>
                <input type="text" ng-model="$parent.returnURL" placeholder="return URL" class="form-control" />
            </div>
            <div>
                <input type="text" ng-model="$parent.recordId" placeholder="record Id" class="form-control" />
            </div>
            <div>
                <input type="text" ng-model="$parent.contextId" placeholder="context Id" class="form-control" />
            </div>
        </div>
    </script>

    <script type="text/ng-template" id="newAssignment.partial">
        <div class="new-assignment-wrapper">
            <div>                
                <input type="text" ng-model="$parent.newAssignment.id" placeholder="id" class="form-control" />
            </div>
            <div>
                <input type="text" ng-model="$parent.newAssignment.title" placeholder="title" class="form-control" />
            </div>            
            <div>
                <input type="text" ng-model="$parent.newAssignment.startDate" placeholder="start Date yyyy-MM-dd" class="form-control" />
            </div>
            <div>
                <input type="text" ng-model="$parent.newAssignment.dueDate" placeholder="due Date yyyy-MM-dd" class="form-control" />
            </div>
            <div>
                <input type="text" ng-model="$parent.newAssignment.callbackUrl" placeholder="callback URL" class="form-control" />
            </div>
            <div>
                <input type="text" ng-model="$parent.newAssignment.iconURL" placeholder="icon URL" class="form-control" />
            </div>
            <div>
                <input type="text" ng-model="$parent.newAssignment.location" placeholder="location" class="form-control" />
            </div>

            <div>                
                <input type="checkbox" ng-model="$parent.chkGradable" />
                gradable
            </div>
            <div ng-show="chkGradable">
                <div>
                    <select ng-model="$parent.scoreType" class="form-control" ng-options="opt as opt for opt in options_scoreType" />
                </div>
                <div>
                    <input type="text" ng-model="$parent.newAssignment.scorePossible" placeholder="score Possible" class="form-control" />
                </div>
            </div>
        </div>
    </script>

    <script src="//code.jquery.com/jquery-1.10.2.js"></script>
    
    <script src="Scripts/hmac-sha1.js"></script>    
    <script src="Scripts/enc-base64-min.js"></script>
    <script src="Scripts/url.min.js"></script>
    <script src="Scripts/oauth-signature.js"></script>

    <script type="text/javascript" src="https://code.angularjs.org/1.4.9/angular.min.js"></script>
    <script type="text/javascript" src="Scripts/cis_app.js"></script>
</body>
</html>
