$(document).ready(async function () {
    // Init choices
    var genreChoices = new Choices('#genresSelectList', { removeItemButton: true });
    var countryChoices = new Choices('#countriesSelectList', { removeItemButton: true, duplicateItemsAllowed: false, searchResultLimit: 10 });
    var directorChoices = new Choices('#directorsSelectList', { removeItemButton: true });
    var actorChoices = new Choices('#actorsSelectList', { removeItemButton: true });

    // Set choices change event handlers
    $(genreChoices.passedElement.element).on('change', handleChoiceChange(genreChoices))
    $(countryChoices.passedElement.element).on('change', handleChoiceChange(countryChoices))
    $(directorChoices.passedElement.element).on('change', handleChoiceChange(directorChoices))
    $(actorChoices.passedElement.element).on('change', handleChoiceChange(actorChoices))

    // Set choices search event handlers
    $(genreChoices.passedElement.element).on({
        'search': async function (event) {
            handleChoiceSearch(event, genreChoices, Urls.Genre.GetAll)
        }
    })

    $(countryChoices.passedElement.element).on({
        'search': async function (event) {
            handleChoiceSearch(event, countryChoices, Urls.Country.GetAll)
        }
    })

    $(directorChoices.passedElement.element).on({
        'search': async function (event) {
            handleChoiceSearch(event, directorChoices, Urls.Artist.GetAll)
        }
    })

    $(actorChoices.passedElement.element).on({
        'search': async function (event) {
            handleChoiceSearch(event, actorChoices, Urls.Artist.GetAll)
        }
    })
})

async function handleChoiceSearch(event, choices, dataUrl) {
    choices.clearChoices()
    var search = event.detail.value

    if (search.length > 2) {
        await choices.setChoices(async () => {
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

                let newChoices = result.data.map(function (data) {
                    return { label: data.Name, value: data.Id };
                })
                newChoices = newChoices.filter(item => !choices.getValue(true).includes(item.value))

                return newChoices;
            } catch (err) {
                console.error(err);
            }
        });
        $(choices.input.element).focus()
    }
}

function handleChoiceChange(choices) {
    choices.clearChoices()
}