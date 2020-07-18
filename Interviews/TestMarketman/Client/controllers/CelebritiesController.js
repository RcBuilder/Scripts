var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    function adopt(value) { return value instanceof P ? value : new P(function (resolve) { resolve(value); }); }
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : adopt(result.value).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
class CelebritiesController {
    constructor(celebritiesService, document) {
        this.celebritiesService = celebritiesService;
        this.document = document;
        this._cardTemplate = '';
        this.init();
    }
    init() {
        this._canvas = this.document.querySelector('#content');
        this.registerEvents();
        this.load();
    }
    load() {
        return __awaiter(this, void 0, void 0, function* () {
            this._cards = yield this.getFromDB();
            this.drawCards();
        });
    }
    drawCards() {
        this._canvas.innerHTML = '';
        this._cards.forEach(card => {
            let node = document.createElement('div');
            node.innerHTML = `<div class="card">
	            <h3 class="truncate">${card.name}</h3>		                
	            <p class="truncate">${card.type} | ${card.birthDate ? card.birthDate.dateFormat() : 'NoDate'} | ${card.knownArtwork}</p>                  
                <p>
                    <image class="img-thumbnail" src="${card.profile}" /> 
                    ${card.desc}
                </p>                
                <p>
                    <button class="btn btn-danger btn-sm" data-id="${card.id}" data-role="delete">Delete</button>                    
                </p>
            </div>`;
            this._canvas.appendChild(node);
        });
    }
    registerEvents() {
        return __awaiter(this, void 0, void 0, function* () {
            // monitor DOM new elements 
            this._canvas.addEventListener('DOMNodeInserted', (e) => {
                e.target.querySelector('[data-role="delete"]').addEventListener('click', (eInner) => __awaiter(this, void 0, void 0, function* () {
                    let selected = eInner.target.attributes['data-id'].value;
                    yield this.remove(selected);
                    yield this.load();
                }));
            }, false);
            this.document.querySelector('[data-role="reload-imdb"]').addEventListener('click', (e) => __awaiter(this, void 0, void 0, function* () {
                yield this.reload();
            }));
            this.document.querySelector('[data-role="reload-server"]').addEventListener('click', (e) => __awaiter(this, void 0, void 0, function* () {
                yield this.load();
            }));
        });
    }
    getFromDB() {
        return __awaiter(this, void 0, void 0, function* () {
            let cards = yield this.celebritiesService.get();
            return cards;
        });
    }
    remove(id) {
        return __awaiter(this, void 0, void 0, function* () {
            let success = yield this.celebritiesService.remove(id);
        });
    }
    reload() {
        return __awaiter(this, void 0, void 0, function* () {
            this._cards = yield this.celebritiesService.reload();
            this.drawCards();
        });
    }
}
