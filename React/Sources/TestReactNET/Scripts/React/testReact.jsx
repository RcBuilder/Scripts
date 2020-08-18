class TestReact extends React.Component {
    render() {
        return (
            <div>
                <h1>Hello React</h1>
                <p>props: {this.props.name}</p>                   
            </div>
        )
    }
}

class TestReact2 extends React.Component {
    render() {
        return (
            <div>
                <h1>Hello React 2</h1>
                <TestComponent1 />
            </div>
        )
    }
}

class TestReact3 extends React.Component {
    renderContent = () => {
        return <h1>Hello React 4</h1>;
    }

    render() {
        let dynamicContent = [];
        dynamicContent.push(<p>dynamic 1</p>);
        dynamicContent.push(<p>dynamic 2</p>);        

        return (
            <>
                {this.renderContent()}
                {dynamicContent}                
            </>
        )
    }
}