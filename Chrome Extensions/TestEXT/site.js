
var popup = null;

document.querySelector('#btn').addEventListener('click', () => {
    fetch('http://127.0.0.1:3334/test4').then(response => response.json()).then(data => {
        console.log(data.authURL);
        popup = window.open(data.authURL, "_blank", "height=400, width=550, status=yes, toolbar=no, menubar=no, location=no,addressbar=no");
    });
});

// chrome.runtime.sendMessage('kodallhpklbdekhpfnnaghbnijkgcahh', message);
chrome.runtime.onMessageExternal.addListener((request, sender, sendResponse) => {
    switch (request.name) {
        case 'token':
            let token = request.value;
            document.querySelector('#token').innerHTML = token;
            popup.close();
            break;
    }
});
