class Wrapper1 extends React.Component {
    constructor(props) {
        super(props);
    }

    render() {
        const box = {
            backgroundColor: '#666',
            color: '#fff',
            padding: '12px',
            border: 'solid 1px #000',
            marginBottom: '10px'
        };

        return (
            <div style={box}>
                {this.props.children}
            </div>
        )
    }
}

class TestWrapper1 extends React.Component {

    render() {
        return (
            <Wrapper1>
                <p>Paragraph 1</p>
                <p>Paragraph 2</p>
                <p>Paragraph 3</p>
            </Wrapper1>    
        )
    }
}
