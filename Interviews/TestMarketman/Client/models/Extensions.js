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
