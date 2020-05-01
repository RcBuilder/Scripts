const puppeteer = require('puppeteer');

takeScreenshot = async (url) => {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();
    await page.goto(url);
    await page.screenshot({ path: 'example.png' });

    await browser.close();
};

saveAsPDF = async (url, pdfName) => {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();
    await page.goto(url, { waitUntil: 'networkidle2' });
    await page.pdf({ path: pdfName, format: 'A4' });

    await browser.close();
};

executeScript = async (url, scriptToExecute) => {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();
    await page.goto(url);

    // page.evaluate(<fun>)
    let scriptResult = await page.evaluate(scriptToExecute);    
    await browser.close();
    return scriptResult;
};

getHTML = async (url) => {
    const browser = await puppeteer.launch();
    const page = await browser.newPage();
    await page.goto(url);

    let html = await page.content();
    await browser.close();
    return html;
};

getByXPath = async (url, xpath, headless = true) => {    
    const browser = await puppeteer.launch({ headless });
    const page = await browser.newPage();
    await page.goto(url);

    // page.evaluate(<fun>, <args>)
    let nodes = await page.evaluate((xpath) => {
        let iter = document.evaluate(xpath, document, null, XPathResult.ANY_TYPE, null);

        let nodes = [];
        let current = {};
        while ((current = iter.iterateNext()) != null)
            nodes.push({
                tag: current.tagName,
                text: current.textContent
            });
        return nodes;
    }, xpath);    
    
    await browser.close();
    return nodes;
};

(async () => {
    //takeScreenshot('https://example.com');
    //saveAsPDF('https://news.ycombinator.com', 'ny.pdf');

    /*
    let scriptResult = await executeScript('https://example.com', () => {
        return {
            width: document.documentElement.clientWidth,
            height: document.documentElement.clientHeight
        };
    });
    console.log(scriptResult);
    */

    /*
    let html = await getHTML('https://example.com');
    console.log(html);
    */

    /*
    let nodes = await getByXPath('https://example.com', '//h1');
    console.log(nodes);
    */

    let nodes = await getByXPath('https://www.trendminer.com/leveragingintegration-webinar/', '//title', true);
    console.log(nodes);
})();