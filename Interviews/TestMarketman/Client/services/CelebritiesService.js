var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class CelebritiesService {
    constructor() {
    }
    parse(x) {
        let date = x.birthDate;
        if (date)
            date = new Date(x.birthDate);
        return new CelebrityCard(x.id, x.name, x.type, x.desc, date, x.profile, x.knownArtwork);
    }
    get() {
        return __awaiter(this, void 0, void 0, function* () {
            let options = {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                },
            };
            try {
                let response = yield fetch(CelebritiesService.server.concat(CelebritiesService.endpoints.get('get')), options);
                let json = yield response.json();
                return json.map(this.parse);
            }
            catch (e) {
                console.error(e);
                return null;
            }
        });
    }
    reload() {
        return __awaiter(this, void 0, void 0, function* () {
            let options = {
                method: 'GET',
                headers: {
                    'Content-Type': 'application/json'
                },
            };
            try {
                let response = yield fetch(CelebritiesService.server.concat(CelebritiesService.endpoints.get('reload')), options);
                let json = yield response.json();
                return json.map(this.parse);
            }
            catch (e) {
                console.error(e);
                return null;
            }
        });
    }
    remove(id) {
        return __awaiter(this, void 0, void 0, function* () {
            let options = {
                method: 'DELETE',
            };
            try {
                let response = yield fetch(CelebritiesService.server.concat(CelebritiesService.endpoints.get('remove')).replace('{id}', id), options);
                return response.json();
            }
            catch (e) {
                console.error(e);
                return false;
            }
        });
    }
}
CelebritiesService.server = 'http://localhost:1166/';
CelebritiesService.endpoints = new Map([
    ['get', 'celebrities'],
    ['reload', 'celebrities/reload'],
    ['remove', 'celebrities/{id}'],
]);
