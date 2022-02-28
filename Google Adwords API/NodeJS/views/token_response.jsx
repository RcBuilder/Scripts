const React = require('react');

module.exports = (props) => {    
    let jsCode = `
        let message = {
            name: 'token',
            value: {
                token: '${props.token}',
                refresh_token: '${props.refresh_token}'
            }
        };

        if(chrome.runtime)
            chrome.runtime.sendMessage('${props.extensionId}', message);
        if(window.opener)
            window.opener.postMessage(message);
    `;
    return (
        <body>
            <h6>{props.token}</h6>
            <script dangerouslySetInnerHTML={{ __html: jsCode }}></script>
        </body>        
    );
}