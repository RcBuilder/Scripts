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
                    &nbsp;&nbsp;
                    <button onMouseOver={this.fun2}>hoverMe</button>
                </p>
            </div>
        )
    }
}

class TestEvents2 extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            value: ''
        };
    }
    
    fun1 = e => {
        let selection = document.getSelection();
        this.setState({
            value: selection.toString()
        });
    }

    render() {
        return (
            <div>
                <h5>{this.state.value}</h5>                
                <p onCopy={this.fun1}>
                    Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.
                </p>
            </div>
        )
    }
}