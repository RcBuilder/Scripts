class TestReact2 extends React.Component {
    render() {
        return (
            <div>
                <h1>Hello React 2</h1>
                <p>{this.props.name}</p>
            </div>
        )
    }
}

ReactDOM.render(<TestReact2 name="Roby" />, document.getElementById('root'));