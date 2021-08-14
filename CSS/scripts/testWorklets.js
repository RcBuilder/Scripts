// Worklet definition
class TestWorklet1 {
    // expose properties
    static get inputProperties() {
        return ['--circle-color'];
    }

    paint(context, geometry, properties) {        
        let x = geometry.width / 2;
        let y = geometry.height / 2;
        let radius = 100;

        context.arc(x, y, radius, 0, 2 * Math.PI, false);
        context.fillStyle = properties.get('--circle-color').toString();        
        context.fill();
    }
}

class TestWorklet2 {
    constructor() {
        this.colors = ['green', 'blue', 'red', 'purple', 'pink', 'yellow', 'silver'];
    }    
    paint(context, geometry, properties) {
        let selected = this.colors[Math.floor(Math.random() * this.colors.length)];        
        context.fillStyle = selected;
        context.fillRect(0, 0, geometry.width, geometry.height);        
    }
}

// Worklet registration
if (typeof registerPaint !== 'undefined') {
    registerPaint('testWorklet1', TestWorklet1);
    registerPaint('testWorklet2', TestWorklet2);
}