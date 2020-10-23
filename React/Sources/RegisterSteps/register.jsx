class Register extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            module: <Step1 postChanges={this.postChanges} changeView={this.changeView} />
        }
    }

    postChanges = async (postURI, payload) => {  
        console.log(`
            POST ${postURI}
            ${JSON.stringify(payload)}
        `);

        let response = await fetch(postURI, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        }).then(result => result.json());

        console.log(response);
        return response;
    }

    changeView = name => {        
        let selected;
        switch (name) {
            default:
            case 'Step1': selected = <Step1 postChanges={this.postChanges} changeView={this.changeView} />;
                break;
            case 'Step2': selected = <Step2 postChanges={this.postChanges} changeView={this.changeView} />;
                break;
            case 'Step3': selected = <Step3 postChanges={this.postChanges} changeView={this.changeView} />;
                break;
        }

        this.setState({
            module: selected
        });
    }

    render() {
        return (
            <>                
                {this.state.module}
            </> 
        )
    }
}

class StepBase extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            nextStep: '', 
            validationSummary: [] 
        }

        this.form = React.createRef();
    }    

    form2Json = form => {
        let elements = form.current.elements;
        let filtered = [].filter.call(elements, e => e.name && e.name != '');

        return [].reduce.call(filtered, (acc, x) => {
            acc[x.name] = x.value;
            return acc;
        }, {});          
    };

    saveForm = async (e) => {
        e.preventDefault();

        let postURI = this.form.current.getAttribute('action');
        if (postURI == '') return;

        let payload = this.form2Json(this.form);
        let response = await this.props.postChanges(postURI, payload);
        if (response.Status == 'OK') {
            this.props.changeView(this.state.nextStep);
            return;
        }

        this.state2ValidationSummary(response.State);
    };

    state2ValidationSummary = state => {        
        let validationSummary = [];
        (state || []).forEach(field => {
            field.errors.forEach(error => {
                validationSummary.push(error);
            });
        });

        this.setState({ validationSummary });
    };
}

class Step1 extends StepBase {
    constructor(props) {
        super(props);
        this.state.nextStep = 'Step2';              
    }
    
    render() {
        return (
            <>           
                <h1>Step1</h1>
                <p>{this.state.nextStep}</p>

                <form ref={this.form} onSubmit={this.saveForm} action="Forms/RegistrationStep1">
                    <input type="text" name="input1" /><br />                    
                    <input type="text" name="input2" /><br /> 
                    <button type="submit">שמור</button>
                </form>

                {this.state.validationSummary.map(x => <p>{x}</p>)}
            </>
        )
    }
}

class Step2 extends StepBase {
    constructor(props) {
        super(props);
        this.state.nextStep = 'Step3';        
    }

    render() {
        return (
            <>
                <h1>Step2</h1>
                <p>{this.state.nextStep}</p>

                <form ref={this.form} onSubmit={this.saveForm} action="Forms/RegistrationStep2">
                    <input type="text" name="input3" /><br />  
                    <button type="submit">שמור</button>
                </form> 

                {this.state.validationSummary.map(x => <p>{x}</p>)}
            </>
        )
    }
}

class Step3 extends StepBase {
    constructor(props) {
        super(props);
        this.state.nextStep = 'Done';        
    }

    render() {
        return (
            <>
                <h1>Step3</h1>
                <p>{this.state.nextStep}</p>

                <form ref={this.form} onSubmit={this.saveForm} action="Forms/RegistrationStep3">
                    <input type="text" name="input4" /><br />  
                    <button type="submit">שמור</button>
                </form>

                {this.state.validationSummary.map(x => <p>{x}</p>)}
            </>
        )
    }
}