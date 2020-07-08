Array.prototype.findByProperty = function (propName, value) {
    return this.find(x => x[propName] === value) || null;
};
Array.prototype.findIndexByProperty = function (propName, value) {
    return this.findIndex(x => x[propName] === value); // -1 = not exists
};
Array.prototype.removeByProperty = function (propName, value) {
    let index = this.findIndex(x => x[propName] === value);
    if (index == -1)
        return false;
    this.splice(index, 1);
    return true;
};
Date.prototype.dateFormat = function () {
    try {
        let year = this.getFullYear().toString();
        let month = (this.getMonth() + 1).toString();
        let day = this.getDate().toString();
        if (month.length == 1)
            month = '0' + month;
        if (day.length == 1)
            day = '0' + day;
        return year + '-' + month + '-' + day;
    }
    catch (_a) {
        return '';
    }
};
