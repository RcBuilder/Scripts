interface Date {
    dateFormat(): string;
}

interface ICelebrityCard {
    id: string,
    name: string,
    type: string,
    desc: string,
    birthDate: Date,
    profile: string,
    knownArtwork: string,
    gender: string
};

interface ICelebritiesService {
    get(): Promise<ICelebrityCard[]>,
    reload(): Promise<CelebrityCard[]>,
    remove(id: string): Promise<boolean>,        
};