class TestEvents extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            value: ''
        };

        this.fun2 = this.fun2.bind(this);  // bind method (for regular functions)
    }

    // arrow function - no binding is required! 'this' refers to the component.
    fun1 = e => {
        this.setState({
            value: Math.floor(Math.random() * 1000)
        });
    }  

    // regular function - binding is required!
    fun2(e) {
        this.setState({
            value: Math.floor(Math.random() * 1000)
        });
    }  

    render() {
        return (
            <div>
                <h5>{this.state.value}</h5>
                <p>
                    <button onClick={this.fun1}>clickMe</button>
                    &nbsp;&nbsp;
                    <button onClick={this.fun2}>clickMe</button>
                </p>
            </div>
        )
    }
}