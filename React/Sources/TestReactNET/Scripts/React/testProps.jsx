class TestProps1 extends React.Component {    
    render() {           
        return (
            <>
                <p>props = {JSON.stringify(this.props)}</p>
                <p>p1 = {this.props.p1}</p>                
            </>
        )
    }
}

class TestProps2 extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <>
                <p>props = {JSON.stringify(this.props)}</p>
                <p>p1 = {this.props.p1}</p>
            </>
        )
    }
}

class TestProps3 extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        return (
            <>
                <p>props = {JSON.stringify(this.props)}</p>
                <p>p1 = {this.props.p1}</p>
            </>
        )
    }
}

class TestProps4 extends React.Component {
    constructor({ p1 }) {
        super();
        this.state = {
            o_p1: p1
        }
    }

    render() {
        return (
            <>
                <p>props = {JSON.stringify(this.props)}</p>
                <p>p1 = {this.props.p1}</p>
                <p>state p1 = {this.state.o_p1}</p>
            </>
        )
    }
}