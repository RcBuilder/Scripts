class CelebritiesService implements ICelebritiesService {
    private static server: string = 'http://localhost:1166/';
    private static endpoints: Map<string, string> = new Map([
        ['get', 'celebrities'],
        ['reload', 'celebrities/reload'],        
        ['remove', 'celebrities/{id}'],
    ]);
   
    constructor() { }

    private parse(x: any): CelebrityCard {
        let date = x.birthDate;
        if (date) date = new Date(x.birthDate);
        return new CelebrityCard(x.id, x.name, x.type, x.desc, date, x.profile, x.knownArtwork);
    }

    async get(): Promise<CelebrityCard[]> {
        let options = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        }

        try {            
            let response: any = await fetch(CelebritiesService.server.concat(CelebritiesService.endpoints.get('get')), options);
            let json = await response.json();
            return json.map(this.parse);
        }
        catch (e) {
            console.error(e);
            return null;
        }
    }

    async reload(): Promise<CelebrityCard[]> {
        let options = {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            },
        }

        try {
            let response: any = await fetch(CelebritiesService.server.concat(CelebritiesService.endpoints.get('reload')), options);
            let json = await response.json();
            return json.map(this.parse);
        }
        catch (e) {
            console.error(e);
            return null;
        }
    }

    async remove(id: string): Promise<boolean> {
        let options = {
            method: 'DELETE',         
        }

        try {
            let response = await fetch(CelebritiesService.server.concat(CelebritiesService.endpoints.get('remove')).replace('{id}', id), options);
            return response.json() as Promise<boolean>;       
        }
        catch (e) {
            console.error(e);
            return false;
        }
    }
}