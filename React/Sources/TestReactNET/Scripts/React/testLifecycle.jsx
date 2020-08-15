class TestMounting extends React.Component {
    constructor(props) {
        super(props);
        this.state = {};
        
        console.log('[TestMounting] constructor');
    }

    static getDerivedStateFromProps(props, state) {
        console.log('[TestMounting] pre-render');        
    }

    componentDidMount() {                
        console.log('[TestMounting] post-render');
    } 
    
    render() {
        console.log('[TestMounting] render');
        
        return (
            <div>
                <h1>Hello Lifecycle</h1>
                <p>see console output</p>                
            </div>
        )
    }
}

class TestUpdating extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            someValue: 1
        };
        
        console.log('[TestUpdating] onstructor');
    }

    static getDerivedStateFromProps(props, state) {
        console.log('[TestUpdating] pre-render');
    }

    shouldComponentUpdate() {
        console.log('[TestUpdating] should perform the update');
        return true;
    }

    getSnapshotBeforeUpdate(prevProps, prevState) {
        console.log('[TestUpdating] get the state before the change');
    }

    componentDidUpdate() {
        console.log('[TestUpdating] post-render');
    } 

    doSomeChange = e => {
        console.log('[TestUpdating] executing doSomeChange method..');
        this.setState({ someValue: 2 });
    }

    render() {
        console.log('[TestUpdating] render');

        return (
            <div>
                <h1>Hello Lifecycle</h1>
                <p>see console output</p>
                <p><button onClick={this.doSomeChange}>clickMe</button></p>
            </div>
        )
    }
}

class TestUnmounting extends React.Component {
    constructor(props) {
        super(props);
        this.state = {
            render: true
        };

        console.log('[TestUnmounting] constructor');
    }

    removeComponent = e => {
        console.log('[TestUnmounting] executing removeComponent method..');
        this.setState({ render: false });
    }
    
    render() {
        console.log('[TestUnmounting] render');

        let childComponent = this.state.render ? <ChildComponent /> : null;

        return (
            <div>
                {childComponent}
                <p>see console output</p>
                <p><button onClick={this.removeComponent}>clickMe</button></p>
            </div>
        )
    }
}

class ChildComponent extends React.Component {
    constructor(props) {
        super(props);
        console.log('[ChildComponent] constructor');
    }

    componentWillUnmount() {
        console.log('[ChildComponent] pre-remove')
    }

    render() {
        console.log('[ChildComponent] render');
        return <h1>Hello Lifecycle</h1>;
    }
}