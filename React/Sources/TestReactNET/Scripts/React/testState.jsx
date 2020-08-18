class TestState extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            p1: 'value-1',
            p2: 'value-2'
        };        
    }

    changeP1 = e => {
        // option1 to update a state
        this.state.p1 = `value-${Math.floor(Math.random() * 1000)}`;
        this.setState(this.state);
    }

    changeP2 = e => {  
        // option2 to update a state
        this.setState({
            p2: `value-${Math.floor(Math.random() * 1000)}`
        });
    }

    render() {
        return (
            <div>
                <p>{this.state.p1}</p>
                <p>{this.state.p2}</p>
                <p>
                    <button onClick={this.changeP1}>change-p1</button>
                    &nbsp;&nbsp;
                    <button onClick={this.changeP2}>change-p2</button>
                </p>
            </div>
        )
    }
}
