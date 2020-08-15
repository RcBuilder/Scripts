class TestReact3 extends React.Component {
    render() {
        return (
            <div>
                <h1>Hello React 3</h1>
                <p>props: {this.props.name}</p>
            </div>
        )
    }
}

ReactDOM.render(<TestReact3 name="Roby" />, document.getElementById('root'));