async function removeChoicesItemHandler(event, removedArray ) {
    var itemValue = event.detail.value
    removedArray.push(itemValue)
}

async function addChoicesItemHandler(event, removedArray) {
    var itemValue = event.detail.value
    var ind = removedArray.indexOf(itemValue)
    if (ind >= 0)
        removedArray.splice(ind, 1)
}

async function handleChoiceSearch(event, choices, dataUrl) {
    choices.clearChoices()
    var search = event.detail.value

    if (search.length > 1) {
        await choices.setChoices(async () => {
            const data = await searchChoicesData(dataUrl, search)
            let newChoices = data.data.map(function (data) {
                return { label: data.Name, value: data.Id };
            })
            newChoices = newChoices.filter(item => !choices.getValue(true).includes(item.value))

            return newChoices;
        });
        $(choices.input.element).focus()
    }
}

function handleChoiceChange(choices) {
    choices.clearChoices()
}

async function getChoicesData(dataUrl) {
    try {
        const result = await $.ajax({
            type: "GET",
            url: dataUrl,
            contentType: 'application/json; charset=utf-8',
            cache: false,
            timeout: 600000
        });
        return result;
    } catch (err) {
        console.error(err);
    }
}

async function searchChoicesData(dataUrl, search) {
    try {
        const data = {
            columns: [
                { data: "Id", name: "", searchable: false, orderable: false, search: { value: "", regex: false } },
                { data: "Name", name: "Name", searchable: true, orderable: true, search: { value: "", regex: false } }
            ],
            draw: 1,
            length: 0,
            order: [{ column: 1, dir: "asc" }],
            search: { value: search, regex: false },
            start: 0
        }

        const result = await $.ajax({
            type: "POST",
            url: dataUrl,
            data: JSON.stringify(data),
            contentType: 'application/json; charset=utf-8',
            cache: false,
            timeout: 600000
        });

        return result;
    } catch (err) {
        console.error(err);
    }
}

class ChoicesClient {
    constructor(selector) {
        this.item = new Choices(selector, { removeItemButton: true, duplicateItemsAllowed: false, searchResultLimit: 10 });
        this.removes = []
    }

    handleSearch(dataUrl) {
        $(this.item.passedElement.element).on({
            'search': async (event) => {
                handleChoiceSearch(event, this.item, dataUrl)
            }
        })
    }

    handleChange() {
        $(this.item.passedElement.element).on('change', handleChoiceChange(this.item))
    }

    handleAddItem() {
        $(this.item.passedElement.element).on({
            'addItem': async (event) => {
                addChoicesItemHandler(event, this.removes)
            }
        })
    }

    handleRemoveItem() {
        $(this.item.passedElement.element).on({
            'removeItem': async (event) => {
                removeChoicesItemHandler(event, this.removes)
            }
        })
    }

    async initChoices(dataUrl) {
        await this.item.setChoices(async () => {
            const data = await getChoicesData(dataUrl)
            const newChoices = data.map(function (data) {
                return { label: data.Name, value: data.Id, selected: true };
            })
            return newChoices;
        });
    }
}