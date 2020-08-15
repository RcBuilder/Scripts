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