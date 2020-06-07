const React = require('react');

module.exports = (props) => {    
    let items = props.items.map(c => {
        return <p>{c.name}</p>
    });

    return (
        <div>
            <h1>{props.name}</h1>
            {items}
        </div>
    );
}