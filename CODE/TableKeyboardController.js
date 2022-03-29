function TableKeyboardController(tableNode, onEnter, selectedClass = 'selectedRow') {
    // properties
    let oTable = tableNode;
    let currentRow;

    // private methods
    let RegisterEvents = () => {
        oTable.addEventListener('click', (e) => {
            oTable.focus();
        });

        oTable.addEventListener('focusout', (e) => {            
            if (currentRow != null) {
                currentRow.classList.remove(selectedClass);
                /// currentRow = null;
            }
        });

        oTable.addEventListener('keydown', (e) => {

            /*
                37 = Arrow LEFT
                38 = Arrow UP
                39 = Arrow RIGHT
                40 = Arrow DOWN
                13 = ENTER
            */
            var keyCode = (e.keyCode > 0) ? e.keyCode : e.charCode;
            // console.log(keyCode);

            switch (keyCode) {
                case 38: ChooseRow('up');
                    break;
                case 40: ChooseRow('down');
                    break;
                case 13:
                    if (onEnter) onEnter.call(null, currentRow);
                    break;
            }
        });
    }
    let UnRegisterEvents = () => {
        oTable.removeEventListener('keydown');
    }
    let ChooseRow = (direction = 'down') => {
        let rows = oTable.querySelectorAll('tbody > tr');
        (rows || []).forEach((x) => x.classList.remove(selectedClass));

        currentRow = currentRow ? direction == 'down' ? currentRow.nextElementSibling : currentRow.previousElementSibling : null;
        if (!currentRow) // reset - take 1st row
            currentRow = oTable.querySelector('tbody > tr');

        currentRow.classList.add(selectedClass);
    }

    // public methods            
    this.Start = () => {
        if (!oTable) return;
        RegisterEvents();
    }
    this.Stop = () => {
        if (!oTable) return;
        UnRegisterEvents();
    }

    // init
    (() => {
        if (!oTable) return;
        console.log('Init TableKeyboardController');
        oTable.setAttribute('tabindex', Math.ceil(Math.random() * 100));
    })();
}