
import { DAL } from './DAL.js';

async function main() {
    DAL.connect();

    let numberOfCatsToCreate: number = 8;
    await DAL.createCats(numberOfCatsToCreate);
    console.log(`${numberOfCatsToCreate} cats have created`);

    let catsCount = await DAL.countCats();
    console.log(`cats: ${catsCount}`);

    let cats = await DAL.getCats();
    console.log(`${cats.length} cats were found`);
    cats.forEach((c) => { console.log(c._id); });

    let catName: string = 'Kitty';
    let catsByName = await DAL.getCatsByName(catName);
    console.log(`${catsByName.length} cats naming ${catName} were found`)
    catsByName.forEach((c) => { console.log(c._id); });

    let color: string = 'Green';
    let catsByCollar = await DAL.getCatsByCollar(color);
    console.log(`${catsByCollar.length} cats with ${color} collar were found`)
    catsByCollar.forEach((c) => { console.log(c._id); });    

    let specificCat = await DAL.getCatById('5d7a40ee775b9242e0d5cec9');
    console.log(`#${specificCat._id} > ${specificCat.name}`);

    let updateResult = await DAL.updateCatName('5d7a40ee775b9242e0d5cec9', 'Tommy');
    console.log(`update result: ${updateResult}`);

    let deleteResult = await DAL.deleteCat('5d7a40ee775b9242e0d5cecc');
    console.log(`delete result: ${deleteResult}`);
}

main();
