class TestModel extends React.Component {
    constructor(props) {
        super(props);        
    }
   
    render() {
        return (
            <>
                <p>{this.props.id}</p>
                <p>{this.props.name}</p>
                <p>{this.props.price}</p>
                <p>{this.props.expiry}</p>
            </>
        )
    }
}

class TestModel2 extends React.Component {
    constructor(props) {
        super(props);
    }

    checkModel = async e => {
        this.props.name = 'short';

        var result = await fetch('CheckModel', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json' // application/json, application/x-www-form-urlencoded
            },
            body: JSON.stringify(this.props)
        }).then(result => result.json());
        console.log(result);
    }

    render() {
        return (
            <p>
                <button onClick={this.checkModel}>clickMe</button>
            </p>
        )
    }
}