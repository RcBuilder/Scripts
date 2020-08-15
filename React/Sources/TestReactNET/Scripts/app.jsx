class TestReact extends React.Component {
    render() {
        return (
            <div>
                <h1>Hello React</h1>
                <p>{this.props.name}</p>   
                <TestComp />
            </div>
        )
    }
}