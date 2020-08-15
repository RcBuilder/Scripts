import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import './index.css';
import './index.scss';
/// import ComponentA from './ComponentA.js';

class Test extends Component {
    constructor(props) {
        super(props);
        this.state = {
            p1: 'value-1', 
            p2: 'value-2'
        };       
    }
    
    changeP1 = e => {
        this.state.p1 = `value-${Math.floor(Math.random() * 1000)}`;
        this.setState(this.state);
    }
    
    render() {
        return (<div>
            <h1 className="blue">Hello World</h1>
            <h6 className="title">Hello World</h6>
            <p>{this.state.p1}</p>
            <p>{this.state.p2}</p>
            <p>{this.props.p3}</p>
            <p><button onClick={this.changeP1}>change-p1</button></p>
            <TestForm />
        </div>)
    }
}

class TestForm extends Component {
    constructor(props) {
        super(props);
        this.state = {
            input1: '',
            input2: ''
        };
    }

    setInput1 = e => {
        this.setState({
            input1: e.target.value
        });
    }

    setInput2 = e => {
        this.setState({
            input2: e.target.value
        });
    }

    submitForm = e => {
        e.preventDefault();
        // code here...
    }

    render() {
        return (
            <form onSubmit={this.submitForm}>
                <h1>Test Form</h1>
                <input type="text" placeholder="input-1" onChange={this.setInput1} />
                &nbsp;&nbsp;<span>{this.state.input1}</span>
                <br /><br />
                <input type="text" placeholder="input-2" onChange={this.setInput2} />
                &nbsp;&nbsp;<span>{this.state.input2}</span>                
                <br /><br />
                <textarea value="Some Description..." />
                <br /><br />
                <select value="2">
                    <option value="1">Value1</option>
                    <option value="2">Value2</option>
                    <option value="3">Value3</option>
                </select>
                <br /><br />
                <input type="submit" />                
            </form>
        )
    }
}

ReactDOM.render(<Test />, document.getElementById('root'));