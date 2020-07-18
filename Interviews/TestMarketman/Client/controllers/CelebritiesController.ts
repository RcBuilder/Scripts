class CelebritiesController {
    protected _cards: CelebrityCard[];
    protected _cardTemplate: string = '';
    protected _canvas: HTMLElement;   

    constructor(protected celebritiesService: ICelebritiesService, protected document: HTMLDocument) {
        this.init();
    }

    protected init(): void {
        this._canvas = this.document.querySelector('#content');
        this.registerEvents();
        this.load();        
    }

    protected async load(): Promise<void> {
        this._cards = await this.getFromDB();        
        this.drawCards();
    }

    protected drawCards(): void {
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

    protected async registerEvents(): Promise<void> {        
        // monitor DOM new elements 
        this._canvas.addEventListener('DOMNodeInserted', (e: any) => {
            e.target.querySelector('[data-role="delete"]').addEventListener('click', async (eInner: any) => {
                let selected = eInner.target.attributes['data-id'].value;
                await this.remove(selected);
                await this.load();
            });
        }, false)

        this.document.querySelector('[data-role="reload-imdb"]').addEventListener('click', async (e: any) => {
            await this.reload();
        });

        this.document.querySelector('[data-role="reload-server"]').addEventListener('click', async (e: any) => {
            await this.load();
        });
    }

    protected async getFromDB(): Promise<CelebrityCard[]> {
        let cards: CelebrityCard[] = await this.celebritiesService.get();        
        return cards;
    }

    protected async remove(id: string): Promise<void> {        
        let success = await this.celebritiesService.remove(id);        
    }

    protected async reload(): Promise<void> {
        this._cards = await this.celebritiesService.reload();
        this.drawCards();
    } 
}