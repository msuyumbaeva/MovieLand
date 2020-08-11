async function postData(url, data) {
    try {
        await $.ajax({
            type: "POST",
            url,
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            cache: false,
            timeout: 600000
        });

    } catch (err) {
        console.error(err);
    }
}

async function getData(url) {
    try {
        const result = await $.ajax({
            type: "GET",
            url,
            cache: false,
            timeout: 600000
        });
        return result;

    } catch (err) {
        console.error(err);
    }
}
