// https://github.com/ChrisAlvares/node-adwords
// https://developers.google.com/adwords/api/docs/reference/v201809/CampaignService
// https://developers.google.com/adwords/api/docs/appendix/selectorfields
// https://developers.google.com/adwords/api/docs/guides/reporting
// https://developers.google.com/adwords/api/docs/appendix/reports
// https://developers.google.com/adwords/api/docs/guides/awql

// > npm install --save node-adwords
const AdwordsUser = require('node-adwords').AdwordsUser;
const AdwordsConstants = require('node-adwords').AdwordsConstants;
const AdwordsAuth = require('node-adwords').AdwordsAuth;
const AdwordsReport = require('node-adwords').AdwordsReport;

const config = require('../config.json');

let auth = new AdwordsAuth({
    client_id: config.client_id,
    client_secret: config.client_secret
}, config.token_response_url);

let parseErrorStatusCode = error => {
    if (error.message == 'No refresh token is set.')
        return 401;
    return 400;
}

module.exports = {    
    generateAuthenticationUrl: () => {
        return auth.generateAuthenticationUrl();
    },
    refreshAccessToken: refreshToken => {
        return new Promise((resolve, reject) => {
            try {                
                auth.refreshAccessToken(refreshToken, (error, tokens) => {                    
                    if (error)
                        reject({
                            statusCode: 400,
                            message: error.message
                        });

                    resolve(tokens.access_token);
                });
            }
            catch (ex) {
                reject({
                    statusCode: 400,
                    message: ex.message
                });
            }
        });
    },
    getAccessTokenFromAuthorizationCodeAsync: code => {
        return new Promise((resolve, reject) => {
            try {
                // tokens: { access_token, refresh_token, scope, token_type, expiry_date }
                auth.getAccessTokenFromAuthorizationCode(code, (error, tokens) => {
                    if (error)
                        reject({
                            statusCode: 400,
                            message: error.message
                        }); 
                    
                    resolve({
                        access_token: tokens.access_token,
                        refresh_token: tokens.refresh_token
                    });
                });
            }
            catch (ex) {
                reject({
                    statusCode: 400,
                    message: ex.message
                });
            }
        });
    },
    getAccounts: token => {
        return new Promise((resolve, reject) => {
            try {

                let user = new AdwordsUser({
                    developerToken: config.developerToken,
                    client_id: config.client_id,
                    client_secret: config.client_secret,
                    userAgent: config.userAgent,                    
                    access_token: token
                });

                let customerService = user.getService('CustomerService', 'v201809');
                customerService.getCustomers({}, (error, result) => {                    
                    if (error) {
                        reject({
                            statusCode: parseErrorStatusCode(error),
                            message: error.message
                        });
                        return;
                    }
                    
                    let customers = result.map(x => {
                        return {
                            id: x.customerId,
                            name: x.descriptiveName,
                            isMcc: x.canManageClients
                        }
                    });    
                    
                    resolve(customers);
                });
            }
            catch (ex) {
                reject({
                    statusCode: 400,
                    message: ex.message
                });
            }
        });        
    },
    getSubAccounts: (token, account) => {
        return new Promise((resolve, reject) => {
            try {

                let user = new AdwordsUser({
                    developerToken: config.developerToken,
                    client_id: config.client_id,
                    client_secret: config.client_secret,
                    userAgent: config.userAgent,
                    clientCustomerId: account,  // specific account
                    access_token: token
                });

                /* 
                    SELECT  TOP(50) Id, Name 
                    FROM    ManagedCustomers
                    WHERE   CustomerId = <Id>
                    ORDER BY Name ASC
                */
                let managedCustomerService = user.getService('ManagedCustomerService', 'v201809');
                let selector = {
                    fields: ['CustomerId', 'Name', 'CanManageClients'],
                    ordering: [{
                        field: 'Name',
                        sortOrder: 'ASCENDING'
                    }]
                }
                let params = { serviceSelector: selector };
                managedCustomerService.get(params, (error, result) => {

                    if (error) {
                        reject({
                            statusCode: parseErrorStatusCode(error),
                            message: error.message
                        });
                        return;
                    }

                    console.log(`found ${result.totalNumEntries} sub-accounts`);  // { totalNumEntries: int, entries: [ANY]}
                    resolve({
                        totalSubAccounts: result.totalNumEntries || 0,
                        subAccounts: (result.entries || []).map(c => {
                            return {
                                id: c.customerId,
                                name: c.name,
                                isMcc: c.canManageClients
                            }
                        })
                    });
                });
            }
            catch (ex) {
                reject({
                    statusCode: 400,
                    message: ex.message
                });
            }
        });
    },
    getCampaigns: (token, account) => {
        return new Promise((resolve, reject) => {
            try {

                let user = new AdwordsUser({
                    developerToken: config.developerToken,
                    client_id: config.client_id,
                    client_secret: config.client_secret,
                    userAgent: config.userAgent,
                    clientCustomerId: account,  // specific account
                    access_token: token
                });
                
                /* 
                    SELECT  TOP(50) Id, Name 
                    FROM    Campaigns
                    WHERE   Status IN ('ENABLED')
                    ORDER BY Name ASC
                */
                let campaignService = user.getService('CampaignService', 'v201809');
                let selector = {
                    fields: ['Id', 'Name'],                                        
                    predicates: [{
                        field: 'Status',
                        operator: 'IN',
                        values: ['ENABLED']
                    }],      
                    ordering: [{
                        field: 'Name',
                        sortOrder: 'ASCENDING'
                    }],
                    paging: {
                        startIndex: 0,
                        numberResults: 50
                    }                    
                }
                let params = { serviceSelector: selector };
                campaignService.get(params, (error, result) => {
                                        
                    if (error) {
                        reject({
                            statusCode: parseErrorStatusCode(error),
                            message: error.message
                        });
                        return;
                    }

                    console.log(`found ${result.totalNumEntries} campaigns`);  // { totalNumEntries: int, entries: [ANY]}
                    resolve({
                        totalCampaigns: result.totalNumEntries || 0,
                        campaigns: (result.entries || []).map(c => {
                            return {
                                id: c.id,
                                name: c.name
                            }
                        })
                    });
                });
            }
            catch (ex) {
                reject({
                    statusCode: 400,
                    message: ex.message
                });
            }
        });
    },
    getCampaignsUsingAWQL: (token, account) => {
        return new Promise((resolve, reject) => {
            try {

                let user = new AdwordsUser({
                    developerToken: config.developerToken,
                    client_id: config.client_id,
                    client_secret: config.client_secret,
                    userAgent: config.userAgent,
                    clientCustomerId: account,  // specific account
                    access_token: token
                });

                /* 
                    SELECT  TOP(50) Id, Name 
                    FROM    Campaigns
                    WHERE   Status IN ('ENABLED')
                    ORDER BY Name ASC
                */
                let campaignService = user.getService('CampaignService', 'v201809');
                let query = 'SELECT Id, Name WHERE Status IN ["ENABLED"] ORDER BY Name DESC LIMIT 0,50';
                let params = { query: query };
                                
                campaignService.query(params, (error, result) => {

                    if (error) {
                        reject({
                            statusCode: parseErrorStatusCode(error),
                            message: error.message
                        });
                        return;
                    }

                    console.log(`found ${result.totalNumEntries} campaigns`);  // { totalNumEntries: int, entries: [ANY]}
                    resolve({
                        totalCampaigns: result.totalNumEntries || 0,
                        campaigns: (result.entries || []).map(c => {
                            return {
                                id: c.id,
                                name: c.name
                            }
                        })
                    });
                });
            }
            catch (ex) {
                reject({
                    statusCode: 400,
                    message: ex.message
                });
            }
        });
    },
    getCampaignsWithStats: (token, account, dateFrom, dateTo) => {
        return new Promise((resolve, reject) => {
            try {

                let userReport = new AdwordsReport({
                    developerToken: config.developerToken,
                    client_id: config.client_id,
                    client_secret: config.client_secret,
                    userAgent: config.userAgent,
                    clientCustomerId: account,  // specific account
                    access_token: token
                });
                
                /* 
                    SELECT  CampaignId, CampaignName, Impressions, Clicks, Cost
                    FROM    CAMPAIGN_PERFORMANCE_REPORT
                    WHERE   Status IN ('ENABLED')                    
                    ORDER BY Name ASC
                */
                let params = {
                    reportName: 'CampaignsWithStats',
                    reportType: 'CAMPAIGN_PERFORMANCE_REPORT',
                    fields: [
                        'CampaignId',
                        'CampaignName',
                        'Impressions',
                        'Clicks',
                        'Cost',
                        'Conversions',
                        'Ctr',
                        'InvalidClicks',
                        'AverageCpc',
                        'AverageCpe',
                        'AverageCpm',
                        'VideoViewRate',
                        'CostPerConversion',
                        'ConversionRate',
                        'ConversionValue',
                        'ValuePerConversion',
                        'Labels',
                        'SearchImpressionShare',
                        'SearchRankLostImpressionShare',
                        'SearchBudgetLostImpressionShare',
                        'ImpressionReach',
                        'AdvertisingChannelType'                        
                    ],
                    filters: [
                        {
                            field: 'CampaignStatus',
                            operator: 'IN',
                            values: ['ENABLED']
                        }
                    ],                    
                    format: 'CSV',
                    additionalHeaders: {
                        skipReportHeader: true,
                        skipReportSummary: true
                    }
                };

                /*
                    TODAY, YESTERDAY, LAST_7_DAYS, LAST_WEEK, LAST_BUSINESS_WEEK, THIS_MONTH, LAST_MONTH, LAST_14_DAYS, LAST_30_DAYS, THIS_WEEK_SUN_TODAY, THIS_WEEK_MON_TODAY, LAST_WEEK_SUN_SAT
                    -
                    ALL_TIME
                    CUSTOM_DATE (requires startDate + endDate)
                */
                if (!dateFrom || !dateTo) {
                    params.dateRangeType = 'ALL_TIME';
                }
                else {
                    params.dateRangeType = 'CUSTOM_DATE';
                    params.startDate = new Date(dateFrom);
                    params.endDate = new Date(dateTo);
                }

                console.log(`params: ${JSON.stringify(params || {})}`);
                userReport.getReport('v201809', params, (error, report) => {                    
                    if (error) {
                        reject({
                            statusCode: parseErrorStatusCode(error),
                            message: error.message
                        });
                        return;
                    }

                    // parse csv
                    let rows = report.split('\n');
                    let campaigns = (rows || []).filter(row => row != '').map(row => {
                        let cols = row.split(',');

                        var index = 0;
                        return {
                            id: cols[index++],
                            name: cols[index++],
                            metrics: {
                                impressions: cols[index++],
                                clicks: cols[index++],
                                cost: cols[index++],
                                conversions: cols[index++],
                                ctr: cols[index++],
                                invalidClicks: cols[index++],
                                averageCpc: cols[index++],
                                averageCpe: cols[index++],
                                averageCpm: cols[index++],
                                videoViewRate: cols[index++],
                                costPerConversion: cols[index++],
                                conversionRate: cols[index++],
                                conversionValue: cols[index++],
                                valuePerConversion: cols[index++],
                                labels: cols[index++],
                                searchImpressionShare: cols[index++],
                                searchRankLostImpressionShare: cols[index++],
                                searchBudgetLostImpressionShare: cols[index++],
                                impressionReach: cols[index++],
                                advertisingChannelType: cols[index++],
                            }
                        }
                    });

                    resolve({ campaigns: campaigns });
                });
            }
            catch (ex) {                
                reject({
                    statusCode: 400,
                    message: ex.message
                });
            }
        });
    },
    getCampaignsWithStatsUsingAWQL: (token, account, dateFrom, dateTo) => {
        return new Promise((resolve, reject) => {
            try {
                let userReport = new AdwordsReport({
                    developerToken: config.developerToken,
                    client_id: config.client_id,
                    client_secret: config.client_secret,
                    userAgent: config.userAgent,
                    clientCustomerId: account,  // specific account
                    access_token: token
                });

                /*            
                    TODAY, YESTERDAY, LAST_7_DAYS, LAST_WEEK, LAST_BUSINESS_WEEK, THIS_MONTH, LAST_MONTH, LAST_14_DAYS, LAST_30_DAYS, THIS_WEEK_SUN_TODAY, THIS_WEEK_MON_TODAY, LAST_WEEK_SUN_SAT
                    -
                    ALL_TIME -> remove the DURING clause
                    CUSTOM_DATE -> yyyyMMdd,yyyyMMdd (e.g: 20200501, 20200531)
                */
                let dateRange = ''; // default (leave empty for ALL_TIME)
                if (dateFrom && dateTo)
                    dateRange = `DURING ${dateFrom},${dateTo}`

                let params = {
                    query: `SELECT  CampaignId, CampaignName, Impressions, Clicks, Cost
                            FROM    CAMPAIGN_PERFORMANCE_REPORT 
                            WHERE   CampaignStatus IN [ENABLED]
                            ${dateRange}`,  // DURING clause 
                    format: 'CSV'
                };

                ///console.log(params.query);

                userReport.getReport('v201809', params, (error, report) => {                    
                    if (error) {
                        reject({
                            statusCode: parseErrorStatusCode(error),
                            message: error.message
                        });
                        return;
                    }

                    let rows = report.split('\n');
                    let campaigns = (rows || []).map(row => {
                        let cols = row.split(',');

                        return {
                            id: cols[0],
                            name: cols[1],
                            metrics: {
                                impressions: cols[2],
                                clicks: cols[3],
                                cost: cols[4]
                            }
                        }
                    });

                    resolve({ campaigns: campaigns });
                });
            }
            catch (ex) {                
                reject({
                    statusCode: 400,
                    message: ex.message
                });
            }
        });
    },      
};