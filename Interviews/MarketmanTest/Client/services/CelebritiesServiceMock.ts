class CelebritiesServiceMock implements ICelebritiesService {
    constructor() { }

    protected cards: CelebrityCard[] = [
        new CelebrityCard('1', 'Artist-1', 'Actor', 'bla bla bla', new Date('1970-07-22'), 'https://picsum.photos/140/209?random=1', 'Some Movie-1'),
        new CelebrityCard('2', 'Artist-2', 'Actor', 'bla bla bla', new Date('1947-01-09'), 'https://picsum.photos/140/209?random=1', 'Some Movie-2'),
        new CelebrityCard('3', 'Artist-3', 'Actor', 'bla bla bla', new Date('1982-11-11'), 'https://picsum.photos/140/209?random=1', 'Some Movie-3'),
    ];    

    protected original = this.cards.slice(0);

    async get(): Promise<CelebrityCard[]> {
        return this.cards;
    }

    async reload(): Promise<CelebrityCard[]> {
        this.cards = this.original;
        return this.cards;
    }

    async remove(id: string): Promise<boolean> {
        this.cards = this.cards.filter(x => x.id != id);
        return true;
    }
}