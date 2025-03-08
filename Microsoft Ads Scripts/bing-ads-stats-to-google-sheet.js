function main() {
  var payload = {"channel": "bing", "reports": []};
  var date = new Date();
  date.setDate(date.getDate() - 1);
  var yesterday = date.toISOString().split('T')[0];

  var rows = AdsApp.campaigns().forDateRange('YESTERDAY').withCondition('Impressions > 0').get();
  while (rows.hasNext()) {
    var campain = rows.next()
    var row = campain.getStats();
    payload["reports"].push([yesterday, campain.getName(), row.getClicks(), row.getImpressions(), row.getCost(), row.getConversions(), 'Search']);
  }


    // push to spreadsheet
    // Follow these instructions to get the credentials https://docs.microsoft.com/en-us/advertising/scripts/examples/authenticating-with-google-services#option1
    const credentials = {
        accessToken: 'access-token',
        clientId: 'oauth-client-id',
        clientSecret: 'oauth-client-secret',
        refreshToken: 'refresh-token'
    };
    var spreadsheetId = 'spreadsheet-id' // can be found from spreadsheet url
    var sheetsApi = GoogleApis.createSheetsService(credentials);
    var updateResponse = sheetsApi.spreadsheets.values.append(
      { spreadsheetId: spreadsheetId, valueInputOption: 'USER_ENTERED', insertDataOption: 'INSERT_ROWS', range: 'A:Z' },
      { range: `A:Z`, values: payload["reports"] }
    );
    Logger.log(`Added ${payload["reports"].length} rows.`);
}

// Code taken from https://docs.microsoft.com/en-us/advertising/scripts/examples/calling-google-services
var GoogleApis;
(function (GoogleApis) {
  function createSheetsService(credentials) {
    return createService("https://sheets.googleapis.com/$discovery/rest?version=v4", credentials);
  }
  GoogleApis.createSheetsService = createSheetsService;

  // Creation logic based on https://developers.google.com/discovery/v1/using#usage-simple
  function createService(url, credentials) {
    var content = UrlFetchApp.fetch(url).getContentText();
    var discovery = JSON.parse(content);
    var baseUrl = discovery['rootUrl'] + discovery['servicePath'];
    var accessToken = getAccessToken(credentials);
    var service = build(discovery, {}, baseUrl, accessToken);
    return service;
  }

  function Response(result, body, status) {
    this.result = result;
    this.body = body;
    this.status = status;
  }
  Response.prototype.toString = function () {
    return this.body;
  }

  function build(discovery, collection, baseUrl, accessToken) {
    for (var name in discovery.resources) {
      var resource = discovery.resources[name];
      collection[name] = build(resource, {}, baseUrl, accessToken);
    }
    for (var name in discovery.methods) {
      var method = discovery.methods[name];
      collection[name] = createNewMethod(method, baseUrl, accessToken);
    }
    return collection;
  }

  function createNewMethod(method, baseUrl, accessToken) {
    return (urlParams, body) => {
      var urlPath = method.path;
      var queryArguments = [];
      for (var name in urlParams) {
        var paramConfg = method.parameters[name];
        if (!paramConfg) {
          throw `Unexpected url parameter ${name}`;
        }
        switch (paramConfg.location) {
          case 'path':
            urlPath = urlPath.replace('{' + name + '}', urlParams[name]);
            break;
          case 'query':
            queryArguments.push(`${name}=${urlParams[name]}`);
            break;
          default:
            throw `Unknown location ${paramConfg.location} for url parameter ${name}`;
        }
      }
      var url = baseUrl + urlPath;
      if (queryArguments.length > 0) {
        url += '?' + queryArguments.join('&');
      }
      var httpResponse = UrlFetchApp.fetch(url, { contentType: 'application/json', method: method.httpMethod, payload: JSON.stringify(body), headers: { Authorization: `Bearer ${accessToken}` }, muteHttpExceptions: true });
      var responseContent = httpResponse.getContentText();
      var responseCode = httpResponse.getResponseCode();
      var parsedResult;
      try {
        parsedResult = JSON.parse(responseContent);
      } catch (e) {
        parsedResult = false;
      }
      var response = new Response(parsedResult, responseContent, responseCode);
      if (responseCode >= 200 && responseCode <= 299) {
        return response;
      }
      throw response;
    }
  }

  function getAccessToken(credentials) {
    if (credentials.accessToken) {
      return credentials.accessToken;
    }
    var tokenResponse = UrlFetchApp.fetch('https://www.googleapis.com/oauth2/v4/token', { method: 'post', contentType: 'application/x-www-form-urlencoded', muteHttpExceptions: true, payload: { client_id: credentials.clientId, client_secret: credentials.clientSecret, refresh_token: credentials.refreshToken, grant_type: 'refresh_token' } });    
    var responseCode = tokenResponse.getResponseCode(); 
    var responseText = tokenResponse.getContentText(); 
    if (responseCode >= 200 && responseCode <= 299) {
      var accessToken = JSON.parse(responseText)['access_token'];
      return accessToken;
    }    
    throw responseText;  
  }
})(GoogleApis || (GoogleApis = {}));