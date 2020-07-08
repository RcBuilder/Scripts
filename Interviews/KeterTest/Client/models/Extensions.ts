﻿Array.prototype.findByProperty = function (propName: string, value: any): Item {
    return this.find(x => x[propName] === value) || null;
}

Array.prototype.findIndexByProperty = function (propName: string, value: any): number {
    return this.findIndex(x => x[propName] === value); // -1 = not exists
}

Array.prototype.removeByProperty = function (propName: string, value: any): boolean {
    let index = this.findIndex(x => x[propName] === value);
    if (index == -1) return false;
    this.splice(index, 1);
    return true;
}

Date.prototype.dateFormat = function () {
    try {
        let year = this.getFullYear().toString();
        let month = (this.getMonth() + 1).toString();
        let day = this.getDate().toString();

        if (month.length == 1) month = '0' + month;
        if (day.length == 1) day = '0' + day;

        return year + '-' + month + '-' + day;
    }
    catch{ return ''; }
}