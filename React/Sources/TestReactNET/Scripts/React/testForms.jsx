class TestForm1 extends React.Component {
    constructor() {
        super();
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
                <h1>Test Form 1</h1>
                <input type="text" placeholder="input-1" onChange={this.setInput1} />
                &nbsp;&nbsp;<span>{this.state.input1}</span>
                <br /><br />
                <input type="text" placeholder="input-2" onChange={this.setInput2} />
                &nbsp;&nbsp;<span>{this.state.input2}</span>
                <br /><br />
                <p><input type="submit" value="SUBMIT" /></p>
            </form>
        )
    }
}

class TestForm2 extends React.Component {
    constructor() {
        super();
        this.state = {
            input1: '',
            input2: ''
        };
    }

    setInput = e => {
        this.setState({
            [e.target.name]: e.target.value
        });
    }

    render() {
        return (
            <form>
                <h1>Test Form 2</h1>
                <input type="text" placeholder="input-1" onChange={this.setInput} name="input1" />
                &nbsp;&nbsp;<span>{this.state.input1}</span>
                <br /><br />
                <input type="text" placeholder="input-2" onChange={this.setInput} name="input2" />
                &nbsp;&nbsp;<span>{this.state.input2}</span>              
                <br /><br />
            </form>
        )
    }
}

class TestForm3 extends React.Component {
    constructor() {
        super();
        this.state = {
            selectedValue: '2',
            description: 'bla bla bla...'
        };
    }

    setSelectedValue = e => {
        this.setState({
            selectedValue: e.target.value
        });
    }

    render() {        
        return (
            <form>
                <h1>Test Form 3</h1>
                <select value={this.state.selectedValue} onChange={this.setSelectedValue}>
                    <option value="1">Value1</option>
                    <option value="2">Value2</option>
                    <option value="3">Value3</option>
                </select>
                <br /><br />
                select value is {this.state.selectedValue}                
                <br /><br />
                <textarea value={this.state.description} />
                <br /><br />
            </form>
        )
    }
}

class TestForm4 extends React.Component {
    constructor() {
        super();
        this.form = React.createRef();        
    }

    submitForm = e => {
        e.preventDefault();
        console.log(this.form.current);  // reference to the current form!
        // code here...
    }

    render() {
        return (
            <form ref={this.form} onSubmit={this.submitForm}>
                <h1>Test Form 4</h1>
                <p><input type="submit" value="SUBMIT" /></p>
            </form>
        )
    }
}

class TestForm5 extends React.Component {
    constructor() {
        super();
        this.state = {
            users: [
                { name: 'User-A' },
                { name: 'User-B' }
            ]
        }
    }

    addUser = (user) => {        
        let newUsers = [...this.state.users, user];
        this.setState({
            users: newUsers
        });
    }

    render() {
        return (
            <>
                <h1>Test Form 5</h1>
                {this.state.users.map(u => <div>{u.name}</div>)}
                <TestForm5Child fnAddUser={this.addUser} />
                <p>use a child component as a form to add new user, update the parent state from the child component</p>
            </>
        )
    }
}

class TestForm5Child extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            name: ''
        };
    }

    setName = e => {
        this.setState({
            name: e.target.value
        });
    }

    submitForm = e => {
        e.preventDefault();
        this.props.fnAddUser({ name: this.state.name });
    }

    render() {
        return (
            <form onSubmit={this.submitForm}>
                <h5>new user</h5>
                <p>
                    <input type="text" placeholder="user-name" onChange={this.setName} />
                    <input type="submit" value="SUBMIT" />
                </p>
            </form>
        )
    }
}

class TestForm5ChildNoForm extends React.Component {       
    addUser = e => {
        this.props.fnAddUser(this.userName.current.value);  // call the parent fn
    }

    render() {
        this.userName = React.createRef();  // create ref to the input

        return (
            <>
                <input ref={this.userName}  type="text" placeholder="user-name"  />
                <button onClick={this.addUser}>add</button>
            </>
        )
    }
}
