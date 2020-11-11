class Subscription extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            module: <SubscriptionDetails {...this.props} />
        }
    }

    changeView = e => {
        let selected;
        switch (e.target.getAttribute('component')) {
            default:
            case 'SubscriptionDetails': selected = <SubscriptionDetails {...this.props} />;
                break;
            case 'SubscriptionWakeupSettings': selected = <SubscriptionWakeupSettings {...this.props} />;
                break;     
            case 'SubscriptionChangePassword': selected = <SubscriptionChangePassword {...this.props} />;
                break;     
        }

        this.setState({
            module: selected
        });
    }

    render() {
        return (
            <>
                <SubscriptionNAV changeView={this.changeView} />
                {this.state.module}                
            </>
        )
    }
}

class SubscriptionNAV extends React.Component {
    render() {
        return (
            <>
                <button onClick={this.props.changeView} component="SubscriptionDetails">החשבון שלי</button>
                <button onClick={this.props.changeView} component="SubscriptionWakeupSettings">הגדרות השקמה</button>  
                <button onClick={this.props.changeView} component="SubscriptionChangePassword">עדכון סיסמה</button>  
                <button>יציאה</button>                
            </>
        )
    }
}

class SubscriptionTabBase extends React.Component {
    constructor(props) {
        super(props);

        this.state = {            
            validationSummary: []
        }

        this.form = React.createRef();
    }

    form2Json = form => {
        let elements = form.current.elements;
        let filtered = [].filter.call(elements, e => e.name && e.name != '');

        return [].reduce.call(filtered, (acc, x) => {
            // handle radio and checkbox - collect only checked
            if (x.tagName.toLowerCase() == 'input' && (x.getAttribute('type') == 'checkbox' || x.getAttribute('type') == 'radio') && x.checked == false)
                return acc;
            // fix checkbox 'ON' value
            if (x.tagName.toLowerCase() == 'input' && x.getAttribute('type') == 'checkbox' && x.value == 'on')
                x.value = true;

            acc[x.name] = x.value;
            return acc;
        }, {});
    };

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

    saveForm = async (e) => {
        e.preventDefault();

        let postURI = this.form.current.getAttribute('action');
        if (postURI == '') return;        

        let payload = this.form2Json(this.form);
        let response = await this.postChanges(postURI, payload);
        if (response.Status == 'OK') {
            alert('OK');
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

class SubscriptionDetails extends SubscriptionTabBase {
    render() {
        return (
            <>
                <h1>{this.props.fullName}</h1>
                <form ref={this.form} onSubmit={this.saveForm} action="Api/Test">
                    <h3>שם פרטי</h3>
                    <p>
                        <input type="text" maxLength="50" placeholder="שם פרטי" name="firstName" id="firstName" value={this.props.firstName} />
                    </p>
                    <h3>שם משפחה</h3>
                    <p>
                        <input type="text" maxLength="50" placeholder="שם משפחה" name="lastName" id="lastName" value={this.props.lastName} />
                    </p>
                    <h3>מספר טלפון לקבלת השירות</h3>
                    <p>
                        <input type="text" maxLength="30" placeholder="טלפון לקבלת השירות" name="phone" id="phone" value={this.props.phone} />
                    </p>
                    <h3>אימייל</h3>
                    <p>
                        <input type="email" maxLength="30" placeholder="אימייל" name="email" id="email" value={this.props.email} disabled />
                    </p>                    

                    <input type="hidden" value={this.props.id} />
                    <button type="submit">שמור</button>
                </form> 

                {this.state.validationSummary.map(x => <p>{x}</p>)}
            </>
        )
    }
}

class SubscriptionChangePassword extends SubscriptionTabBase {
    render() {
        return (
            <>
                <form ref={this.form} onSubmit={this.saveForm} action="Api/Test">
                    <h3>סיסמה</h3>
                    <p>
                        <input type="password" maxLength="30" placeholder="סיסמה" name="password" id="password" value={this.props.password} />
                    </p>

                    <button type="submit">שמור</button>
                </form>

                {this.state.validationSummary.map(x => <p>{x}</p>)}
            </>
        )
    }
}

class SubscriptionWakeupSettings extends SubscriptionTabBase {
    render() {
        return (
            <>                
                <form ref={this.form} onSubmit={this.saveForm} action="Api/Test">
                    <h3>שעת השקמה</h3>
                    <p>
                        <input type="time" name="wakeupTime" id="wakeupTime" value={this.props.wakeupTime} />
                    </p>
                    <h3>איך לפנות</h3>
                    <p>
                        <input type="radio" name="gender" value="0" checked={this.props.gender == 0} />זכר
                        <input type="radio" name="gender" value="1" checked={this.props.gender == 1} />נקבה
                    </p>
                    <h3>שיחת בוקר עם קול</h3>
                    <p>
                        <input type="radio" name="voiceGender" value="0" checked={this.props.voiceGender == 0} />ג'רמי
                        <input type="radio" name="voiceGender" value="1" checked={this.props.voiceGender == 1} />לוסי
                    </p>
                    <h3>ימים לקבלת השיחות</h3>
                    <p>
                        <input type="radio" name="wakeupState" value="0" checked={this.props.wakeupState == 0} />ימים א-ש
                        <input type="radio" name="wakeupState" value="1" checked={this.props.wakeupState == 1} />ימים א-ה
                    </p>
                    <h3>מצב השירות</h3>
                    <p>
                        <input type="checkbox" name="isServiceActive" checked={this.props.isServiceActive} />פעיל                        
                    </p>
                    
                    <ul>
                        <li>הודעה אחרונה נשלחה בתאריך {this.props.lastReceivedMessage}</li>
                        <li>השירות פעיל עד {this.props.validUntil}</li>
                    </ul>
                
                    <button type="submit">שמור</button>
                </form> 

                {this.state.validationSummary.map(x => <p>{x}</p>)}
            </>
        )
    }
}