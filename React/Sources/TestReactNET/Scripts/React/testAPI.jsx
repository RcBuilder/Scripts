class TestAPI extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            customers: []
        }
    }

    // post-render
    componentDidMount() {
        const service = 'https://www.w3schools.com/js/customers_mysql.php';
        fetch(service)
            .then(data => data.json())
            .then(json => this.setState({
                customers: json
            }));
    }

    render() {                
        return (
            <ul>
                {this.state.customers.map(x => <li>{x.Name}</li>)}
            </ul>
        )
    }
}

class TestAPIAsync extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            customers: []
        }
    }

    // post-render
    async componentDidMount() {
        const service = 'https://www.w3schools.com/js/customers_mysql.php';
        let data = await fetch(service)
        let json = await data.json();

        this.setState({
            customers: json
        });
    }

    render() {
        return (
            <ul>
                {this.state.customers.map(x => <li>{x.Name}</li>)}
            </ul>
        )
    }
}