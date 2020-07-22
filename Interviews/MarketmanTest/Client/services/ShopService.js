var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class ShopService {
    constructor() {
    }
    getItems(pageNum, pageSize = 0 /* 0 = ALL */) {
        return __awaiter(this, void 0, void 0, function* () {
            let options = {
                method: 'GET',
            };
            try {
                let response = yield fetch(ShopService.server.concat(ShopService.endpoints.get('getItems')), options);
                return (yield response.json()).map(x => new Item(x.code, x.name, x.desc, new Date(x.saleDate), x.image));
            }
            catch (e) {
                console.error(e);
                return null;
            }
        });
    }
    addItem(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let options = {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(item)
            };
            try {
                let response = yield fetch(ShopService.server.concat(ShopService.endpoints.get('addItem')), options);
                return response.json();
            }
            catch (e) {
                console.error(e);
                return null;
            }
        });
    }
    updateItem(item) {
        return __awaiter(this, void 0, void 0, function* () {
            let options = {
                method: 'PUT',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(item)
            };
            try {
                let response = yield fetch(ShopService.server.concat(ShopService.endpoints.get('updateItem')), options);
                return response.json();
            }
            catch (e) {
                console.error(e);
                return null;
            }
        });
    }
    removeItem(code) {
        return __awaiter(this, void 0, void 0, function* () {
            let options = {
                method: 'DELETE',
            };
            try {
                let response = yield fetch(ShopService.server.concat(ShopService.endpoints.get('removeItem')).replace('{code}', String(code)), options);
                return response.json();
            }
            catch (e) {
                console.error(e);
                return false;
            }
        });
    }
}
ShopService.server = 'http://localhost:1166/';
ShopService.endpoints = new Map([
    ['getItems', 'store/items'],
    ['addItem', 'store/item'],
    ['updateItem', 'store/item'],
    ['removeItem', 'store/item/{code}'],
]);
