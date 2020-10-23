class HomeRouter extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            module: <Home />
        }
    }

    changeView = e => {        
        let selected;
        switch (e.target.getAttribute('component')) {
            default:
            case 'Home': selected = <Home />;
                break;
            case 'ContactUs': selected = <ContactUs />;
                break;
            case 'AboutUs': selected = <AboutUs />;
                break;
        }

        this.setState({
            module: selected
        });
    }

    render() {
        return (
            <>                
                <HomeNAV changeView={this.changeView} />
                {this.state.module}
            </> 
        )
    }
}

class HomeNAV extends React.Component {
    render() {
        return (
            <>
                <button onClick={this.props.changeView} component="Home">Home</button>
                <button onClick={this.props.changeView} component="ContactUs">ContactUs</button>
                <button onClick={this.props.changeView} component="AboutUs">AboutUs</button>
            </>
        )
    }
}

class Home extends React.Component {
    render() {
        return <h1>Home</h1>
    }
}

class ContactUs extends React.Component {
    render() {
        return <h1>ContactUs</h1>
    }
}

class AboutUs extends React.Component {
    render() {
        return <h1>AboutUs</h1>
    }
}