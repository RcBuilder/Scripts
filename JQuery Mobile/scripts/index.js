
$(window).on('scrollstart', function (e) {
    console.log('[scrollstart] scroll start');
});

$(window).on('scrollstop', function (e) {
    console.log('[scrollstop] scroll stop');
});

$(window).on('swiperight', function (e) {
    console.log('[swiperight] swiping right');
});

$(window).on('vclick', function (e) {
    console.log('[vclick] click');
});

// PAGES
$(window).on("pagecontainerbeforechange", function (e, data) {
    var state = '';

    if (typeof (data.toPage) == 'string')
        state = 'before load';
    else if (typeof (data.toPage) == 'object')
        state = 'load completed';

    console.log('[pagebeforechange] ' + 'state: ' + state + ', destination: ' + data.absUrl);
});

$(window).on('pagebeforecreate', function (e) {
    console.log('[pagebeforecreate] creating page ' + e.target.id);
});

$(window).on('pagecontainerbeforehide', function (e, data) {
    console.log('[pagebeforehide] leaving page ' + data.prevPage[0].id);
});

$(window).on('pagecontainerbeforeshow', function (e, data) {
    console.log('[pagebeforeshow] pre show page ' + data.toPage[0].id);
});

$(window).on("pagecontainerchange", function (e, data) {
    console.log('[pagechange] changing page ' + data.toPage[0].id);
});

$(window).on('pagecreate', function (e) {
    console.log('[pagecreate] page ' + e.target.id + ' created');
});

$(window).on('pagecontainerhide', function (e, data) {
    console.log('[pagehide] page ' + data.prevPage[0].id + ' left');
});

$(window).on('pagecontainershow', function (e, data) {
    console.log('[pageshow] showing page ' + data.toPage[0].id);
});

//-------------------------------------------------------------------------------------

$('#btnLoader1').click(function () {
    $.mobile.loading("show", {
        text: '',
        textVisible: false,
        theme: 'b',
        textonly: false,
        html: ''
    });
});

$('#btnLoader2').click(function () {
    $.mobile.loading("show", {
        text: 'Loading...',
        textVisible: true,
        theme: 'b',
        textonly: false,
        html: ''
    });
});