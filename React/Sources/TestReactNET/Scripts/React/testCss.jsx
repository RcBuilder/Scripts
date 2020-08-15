class TestCss1 extends React.Component {
    render() {
        return (
            <div>
                <h1 style={{ color: this.props.headerColor }}>Hello Css 1</h1>
                <p>props: {this.props.headerColor}</p>                                   
            </div>
        )
    }
}

class TestCss2 extends React.Component {
    render() {
        const headerStyles = {
            color: this.props.headerColor,
            textDecoration: 'underline'
        };

        return (
            <div>
                <h1 style={headerStyles}>Hello Css 2</h1>
                <p>props: {this.props.headerColor}</p>
            </div>
        )
    }
}

class TestCss3 extends React.Component {
    render() {
        return (
            <div>
                <h1 className={this.props.headerClass}>Hello Css 3</h1>
                <p>props: {this.props.headerClass}</p>
            </div>
        )
    }
}