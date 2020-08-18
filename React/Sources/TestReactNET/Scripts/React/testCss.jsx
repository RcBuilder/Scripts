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

class TestCss4 extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            classA: 'classA'
        };
    }

    getClassB = () => {
        return 'classB';
    }

    render() {
        let classC = 'classC';

        return (
            <>
                <h1 className={`${this.state.classA} ${this.getClassB()} ${classC}`}>Hello Css 4</h1>
                <p>multiple classes</p>
            </>
        )
    }
}

class TestCss5 extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            stylesA: {
                color: '#fff'
            }
        };
    }

    getStylesB = () => {
        return {
            backgroundColor: 'mediumvioletred'
        };
    }

    render() {
        let stylesC = {
            fontStyle: 'italic'
        };
        
        return (
            <>
                <h1 style={{ ...this.state.stylesA, ...this.getStylesB(), ...stylesC }}>Hello Css 5</h1>
                <p>multiple styles</p>
            </>
        )
    }
}