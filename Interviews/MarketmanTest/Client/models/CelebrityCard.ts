class CelebrityCard implements ICelebrityCard {
    constructor(public id: string, public name: string, public type: string, public desc: string, public birthDate: Date, public profile: string, public knownArtwork: string, public gender: string) { }
}