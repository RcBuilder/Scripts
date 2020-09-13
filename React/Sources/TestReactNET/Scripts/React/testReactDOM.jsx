class TestReactDOM extends React.Component {
    render() {
        return (
            <div>
                <h1>Hello React DOM</h1>
                <p>props: {this.props.name}</p>
            </div>
        )
    }
}

ReactDOM.render(<TestReactDOM name="Roby" />, document.getElementById('root'));