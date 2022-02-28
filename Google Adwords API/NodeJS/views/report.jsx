const React = require('react');

module.exports = (props) => {
    console.log(props.items.length);
    let items = props.items.slice(1).map(x => {
        return(
            <tr>
                <td>{x.campaignId}</td>
                <td>{x.impressions}</td>
                <td>{x.clicks}</td>
                <td>{x.cost}</td>
            </tr>)
    });
    
    return (
        <div>
            <h1>{props.name}</h1>
            <table border="1" cellPadding="10">
                <tr>
                    <th>CampaignId</th>
                    <th>Impressions</th>
                    <th>Clicks</th>
                    <th>Cost</th>
                </tr>
                {items}
            </table>            
        </div>
    );
}